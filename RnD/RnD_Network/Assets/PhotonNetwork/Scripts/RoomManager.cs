using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    public GameObject readyBtn;
    public GameObject readyCancelBtn;

    public string nextScene;

    PhotonView pv;
    PhotonReady pr;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        pr = GetComponent<PhotonReady>();
    }
    IEnumerator Start()
    {
        PhotonNetwork.isMessageQueueRunning = true;

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
        PhotonNetwork.LoadLevelAsync(nextScene);
    }
}
