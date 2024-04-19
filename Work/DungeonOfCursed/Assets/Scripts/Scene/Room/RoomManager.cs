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

    private GameManager game;
    private PhotonView pv;
    private PhotonReady pr;

    private void Awake()
    {
        game = FindObjectOfType<GameManager>();
        pv = GetComponent<PhotonView>();
        pr = GetComponent<PhotonReady>();
    }
    IEnumerator Start()
    {
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
        roomReadyText.text = "" + pr.GetReadyPlayerCount() + "/" + PhotonNetwork.room.PlayerCount;
    }

    public void OnReady()
    {
        readyBtn.SetActive(false);
        readyCancelBtn.SetActive(true);
        pr.Ready();
    }
    public void OnReadyCancel()
    {
        readyBtn.SetActive(true);
        readyCancelBtn.SetActive(false);
        pr.ReadyCancel();
    }

    [PunRPC] void LoadStage()
    {
        StartCoroutine(_LoadStage());
    }
    IEnumerator _LoadStage()
    {
        PhotonNetwork.isMessageQueueRunning = false;

        yield return game.StartLoading();
        yield return game.ChangeScene("RoomScene", nextScene);
    }
}
