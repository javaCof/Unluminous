using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonReady : MonoBehaviour
{
    PhotonView pv;
    int readyPlayers;
    bool isReady;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    public void Ready()
    {
        pv.RPC("AddReady", PhotonTargets.MasterClient);
    }

    [PunRPC]
    void AddReady()
    {
        if (++readyPlayers == PhotonNetwork.room.PlayerCount)
            pv.RPC("SetReady", PhotonTargets.All);
    }

    [PunRPC]
    void SetReady()
    {
        isReady = true;
    }

    public IEnumerator WaitForReady()
    {
        yield return new WaitUntil(() => isReady);
    }
}
