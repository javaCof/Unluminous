using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviour
{
    public GameObject readyBtn;
    public GameObject readyCancelBtn;
    public Text roomNameText;
    public Text roomReadyText;

    public string nextScene;

    private PhotonView pv;
    private PhotonReady pr;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        pr = GetComponent<PhotonReady>();
    }
    IEnumerator Start()
    {
        GameManager.Instance.curScene = gameObject.scene.name;

        roomNameText.text = PhotonNetwork.room.Name;
        roomReadyText.text = "0/" + PhotonNetwork.room.PlayerCount;

        yield return StartCoroutine(pr.WaitForReady());

        readyBtn.SetActive(false);
        readyCancelBtn.SetActive(false);

        if (PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.room.IsOpen = false;
            PhotonNetwork.room.IsVisible = false;

            pv.RPC("LoadStage", PhotonTargets.All);
        }
    }
    private void Update()
    {
        if (PhotonNetwork.inRoom)
        {
            roomReadyText.text = "" + pr.GetReadyPlayerCount() + "/" + PhotonNetwork.room.PlayerCount;
        }
    }

    public void OnReady()
    {
        SoundManager.instance.PlaySfx("menu");
        readyBtn.SetActive(false);
        readyCancelBtn.SetActive(true);
        pr.Ready();
    }
    public void OnReadyCancel()
    {
        SoundManager.instance.PlaySfx("menu");
        readyBtn.SetActive(true);
        readyCancelBtn.SetActive(false);
        pr.ReadyCancel();
    }

    public void BackToLobby()
    {
        SoundManager.instance.PlaySfx("menu");
        if (PhotonNetwork.inRoom) PhotonNetwork.LeaveRoom();
        GameManager.Instance.LoadingScene("LobbyScene");
    }

    [PunRPC] void LoadStage()
    {
        StartCoroutine(_LoadStage());
    }
    IEnumerator _LoadStage()
    {
        PhotonNetwork.isMessageQueueRunning = false;

        yield return GameManager.Instance.StartLoading();
        yield return GameManager.Instance.ChangeScene(GameManager.Instance.curScene, nextScene);
    }
}
