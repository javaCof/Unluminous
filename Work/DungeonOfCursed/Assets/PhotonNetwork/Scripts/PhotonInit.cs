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

    [Header("NEXT SCENE")]
    public string nextScene;

    private GameManager game;

    private void Awake()
    {
        game = FindObjectOfType<GameManager>();

        if (!PhotonNetwork.connected)
        {
            if (PhotonNetwork.ConnectUsingSettings(version))
            {
                PhotonNetwork.logLevel = LogLevel;
                PhotonNetwork.playerName = "USER_" + Random.Range(1, 9999);
            }
            else game.ErrorGame(ERROR_CODE.PHOTON_CONNECT_ERROR);
        }
    }
    private void Start()
    {
        game.curScene = gameObject.scene.name;
        if (PhotonNetwork.insideLobby)
        {
            UpdateUserId();
            UpdateRoomName();
            StartCoroutine(game.EndLoading());
        }
    }

    public void JoinRandom()
    {
        UpdateUserId();
        PhotonNetwork.player.NickName = userIdInput.text;
        PhotonNetwork.JoinRandomRoom();
    }
    public void CreateRoom()
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
    public void JoinRoom(string roomName)
    {
        UpdateUserId();
        PhotonNetwork.player.NickName = userIdInput.text;
        PhotonNetwork.JoinRoom(roomName);
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

    public void BackToMenu()
    {
        if (PhotonNetwork.inRoom) PhotonNetwork.LeaveRoom();
        game.LoadingScene("MenuScene");
    }

    IEnumerator EnterRoom()
    {
        AsyncOperation ao = PhotonNetwork.LoadLevelAsync(nextScene);
        yield return ao;
    }

    /*---------------Photon OK---------------*/
    void OnJoinedLobby()
    {
        Debug.Log("PHOTON : Joined Lobby");
        UpdateUserId();
        UpdateRoomName();

        StartCoroutine(game.EndLoading());
    }
    void OnJoinedRoom()
    {
        Debug.Log("PHOTON : Enter Room");
        StartCoroutine(EnterRoom());
    }
    void OnReceivedRoomListUpdate()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("RoomItem"))
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

            RoomDataUI roomData = room.GetComponent<RoomDataUI>();
            roomData.roomName = _room.Name;
            roomData.connectPlayer = _room.PlayerCount;
            roomData.maxPlayers = _room.MaxPlayers;

            roomData.UpdateRoomData();
            room.GetComponent<Button>().onClick.AddListener(
                () => JoinRoom(roomData.roomName)
                );

            roomListGrid.constraintCount = ++rowCount;
            gridRect.sizeDelta += new Vector2(0, roomListGrid.cellSize.y + roomListGrid.spacing.y);
        }
    }

    /*---------------Photon Failed---------------*/
    void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        Debug.Log("PHOTON : Photon Connect Failed");
        game.ErrorGame(ERROR_CODE.PHOTON_CONNECT_ERROR);
    }
    void OnPhotonRandomJoinFailed(object[] codeAndMsg)
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
        game.ErrorGame(ERROR_CODE.PHOTON_CREATE_ROOM_ERROR);
    }
    void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {
        Debug.Log("PHOTON : Join Room Failed");
        game.ErrorGame(ERROR_CODE.PHOTON_JOIN_ROOM_ERROR);
    }
}
