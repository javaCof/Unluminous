using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public GameObject readyBtn;
    public GameObject startBtn;

    PhotonReady pr;

    private void Awake()
    {
        pr = GetComponent<PhotonReady>();
    }

    IEnumerator Start()
    {
        PhotonNetwork.isMessageQueueRunning = true;

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
        
    }
}
