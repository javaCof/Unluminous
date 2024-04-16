using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonReady : MonoBehaviour
{
    PhotonView pv;

    bool localReady = false;
    bool allReady = false;
    int readyPlayers;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    public void Ready()
    {
        localReady = true;
        pv.RPC("AddPhotonReady", PhotonTargets.MasterClient);
    }
    [PunRPC] public void ReadyCancel()
    {
        if (localReady)
        {
            localReady = false;
            pv.RPC("RemovePhotonReady", PhotonTargets.MasterClient);
        }
    }
    public void ReadyReset()
    {
        allReady = false;
        pv.RPC("ReadyCancel", PhotonTargets.All);
        readyPlayers = 0;
    }

    [PunRPC] void AddPhotonReady()
    {
        if (++readyPlayers == PhotonNetwork.room.PlayerCount)
            pv.RPC("PhotonAllReady", PhotonTargets.All);
    }
    [PunRPC] void RemovePhotonReady() => readyPlayers--;
    [PunRPC] void PhotonAllReady() => allReady = true;

    public IEnumerator WaitForReady()
    {
        yield return new WaitUntil(() => allReady);
    }

    void OnLeftRoom() => ReadyCancel();
}
