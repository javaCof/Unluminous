using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PhotonInit : MonoBehaviour
{
    [Header("PHOTON SETTING")]
    public string version = "Ver 0.1.0";
    public PhotonLogLevel LogLevel = PhotonLogLevel.Full;

    [Header("ROOM/USER INFO")]
    public InputField userIdInput;
    public InputField roomNameInput;

    [Header("ROOM UI")]
    public GridLayoutGroup roomListGrid;
    public GameObject roomItem;

    private void Awake()
    {
        if (!PhotonNetwork.connected)
        {
            PhotonNetwork.ConnectUsingSettings(version);
            PhotonNetwork.logLevel = LogLevel;
            PhotonNetwork.playerName = "USER_" + Random.Range(1, 9999);
        }
    }

    void OnJoinedLobby()
    {
        Debug.Log("PHOTON : Joined Lobby");
        UpdateUserId();
        UpdateRoomName();
    }
    void OnJoinedRoom()
    {
        Debug.Log("PHOTON : Enter Room");
        StartCoroutine(LoadStage());
    }
    public void OnClickJoinRandom()
    {
        UpdateUserId();
        PhotonNetwork.player.NickName = userIdInput.text;
        PhotonNetwork.JoinRandomRoom();
    }
    public void OnClickCreateRoom()
    {
        UpdateUserId();
        UpdateRoomName();

        PhotonNetwork.player.NickName = userIdInput.text;

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 10;

        PhotonNetwork.CreateRoom(roomNameInput.text, roomOptions, TypedLobby.Default);
    }
    public void OnClickJoinRoom(string roomName) 
    {
        UpdateUserId();
        PhotonNetwork.player.NickName = userIdInput.text;
        PhotonNetwork.JoinRoom(roomName);
    }
    void OnReceivedRoomListUpdate()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("ROOM_ITEM"))
        {
            Destroy(obj);
        }

        int rowCount = 0;
        RectTransform gridRect = roomListGrid.GetComponent<RectTransform>();
        gridRect.sizeDelta = new Vector2(0, roomListGrid.padding.top);

        foreach (RoomInfo _room in PhotonNetwork.GetRoomList())
        {
            GameObject room = Instantiate(roomItem);
            room.transform.SetParent(gridRect, false);

            PhotonRoomData roomData = room.GetComponent<PhotonRoomData>();
            roomData.roomName = _room.Name;
            roomData.connectPlayer = _room.PlayerCount;
            roomData.maxPlayers = _room.MaxPlayers;

            roomData.UpdateRoomData();
            room.GetComponent<Button>().onClick.AddListener(
                ()=> OnClickJoinRoom(roomData.roomName)
                );

            roomListGrid.constraintCount = ++rowCount;
            gridRect.sizeDelta += new Vector2(0, roomListGrid.cellSize.y + roomListGrid.spacing.y);
        }
    }

    public void UpdateUserId()
    {
        string _userId = userIdInput.text;
        if (string.IsNullOrEmpty(_userId))
        {
            _userId = "USER_" + Random.Range(0, 999).ToString("000");
        }
        userIdInput.text = _userId;
    }
    public void UpdateRoomName()
    {
        string _roomName = roomNameInput.text;
        if (string.IsNullOrEmpty(_roomName))
        {
            _roomName = "ROOM_" + Random.Range(0, 999).ToString("000");
        }
        roomNameInput.text = _roomName;
    }

    IEnumerator LoadStage()
    {
        PhotonNetwork.isMessageQueueRunning = false;
        AsyncOperation ao = SceneManager.LoadSceneAsync("RoomScene");
        yield return ao;
    }

    /*---------------Photon Failed---------------*/
    void OnPhotonRandomJoinFailed()
    {
        Debug.Log("PHOTON : Random Join Failed");

        UpdateRoomName();

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 10;

        bool isSuccess = PhotonNetwork.CreateRoom(roomNameInput.text, roomOptions, TypedLobby.Default);

        Debug.Log("PHOTON : Create new Room : " + isSuccess);
    }
    void OnPhotonCreateRoomFailed(object[] codeAndMsg) 
    {
        Debug.Log("PHOTON : Create Room Failed");
    }
}
