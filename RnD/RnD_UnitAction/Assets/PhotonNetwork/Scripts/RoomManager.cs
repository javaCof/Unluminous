using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    public GameObject readyBtn;
    public GameObject startBtn;

    public string NextScene;

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
        startBtn.SetActive(false);

        yield return StartCoroutine(pr.WaitForReady());

        startBtn.SetActive(PhotonNetwork.isMasterClient);
    }

    public void OnReady()
    {
        readyBtn.SetActive(false);
        pr.Ready();
    }

    public void OnStart()
    {
        pv.RPC("LoadStage", PhotonTargets.All);
    }

    [PunRPC]
    void LoadStage()
    {
        SceneManager.LoadSceneAsync(NextScene);
    }
}
