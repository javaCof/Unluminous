using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potal : MonoBehaviour, IPhotonPoolObject
{
    private MapGenerator map;
    private PhotonView pv;

    private void Awake()
    {
        map = FindObjectOfType<MapGenerator>();
        pv = GetComponent<PhotonView>();
    }

    private void OnTriggerEnter(Collider oth)
    {
        if (oth.tag == "Player")
        {
            if (PhotonNetwork.inRoom)
                map.GetComponent<PhotonView>().RPC("ResetLevel", PhotonTargets.MasterClient);
            else map.ResetLevel();
        }
    }

    public void OnPoolCreate()
    {
        if (pv.isMine)
            pv.RPC("OnPotalCreate", PhotonTargets.All);
    }
    public void OnPoolEnable()
    {
        if (pv.isMine)
            pv.RPC("OnPotalEnable", PhotonTargets.All);
    }
    public void OnPoolDisable()
    {
        if (pv.isMine)
            pv.RPC("OnPotalDisable", PhotonTargets.All);
    }

    [PunRPC]
    void OnPotalCreate()
    {
        transform.parent = map.poolPos;
        gameObject.SetActive(false);
    }
    [PunRPC]
    void OnPotalEnable()
    {
        transform.parent = map.objectPos;
        gameObject.SetActive(true);
    }
    [PunRPC]
    void OnPotalDisable()
    {
        transform.parent = map.poolPos;
        gameObject.SetActive(false);
    }
}
