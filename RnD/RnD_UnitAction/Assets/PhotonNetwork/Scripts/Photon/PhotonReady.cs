using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonReady : MonoBehaviour
{
    PhotonView pv;

    bool localReady = false;
    int readyPlayers;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    public void Ready()
    {
        localReady = true;
        pv.RPC("AddReady", PhotonTargets.MasterClient);
    }
    [PunRPC] public void ReadyCancel()
    {
        if (localReady)
        {
            localReady = false;
            pv.RPC("RemoveReady", PhotonTargets.MasterClient);
        }
    }
    public void ReadyReset()
    {
        pv.RPC("ReadyCancel", PhotonTargets.All);
        readyPlayers = 0;
    }

    [PunRPC] void AddReady() => readyPlayers++;
    [PunRPC] void RemoveReady() => readyPlayers--;

    public IEnumerator WaitForReady()
    {
        yield return new WaitUntil(() => readyPlayers == PhotonNetwork.room.PlayerCount);
    }

    void OnLeftRoom() => ReadyCancel();
}
