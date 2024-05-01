using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potal : MonoBehaviour, IPoolObject
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
            if (!PhotonNetwork.inRoom)
                map.ResetLevel();
            else if (oth.GetComponent<PhotonView>().isMine)
                map.GetComponent<PhotonView>().RPC("ResetLevel", PhotonTargets.MasterClient);
        }
    }

    [PunRPC] public void OnPoolCreate()
    {
        if (pv.isMine)
            pv.RPC("OnPoolCreate", PhotonTargets.Others);

        transform.parent = map.poolPos;
        gameObject.SetActive(false);
    }
    [PunRPC] public void OnPoolEnable(Vector3 pos, Quaternion rot)
    {
        if (pv.isMine)
            pv.RPC("OnPoolEnable", PhotonTargets.Others, pos, rot);

        transform.parent = map.objectPos;
        transform.localPosition = pos;
        transform.localRotation = rot;
        gameObject.SetActive(true);
    }
    [PunRPC] public void OnPoolDisable()
    {
        if (pv.isMine)
            pv.RPC("OnPoolDisable", PhotonTargets.Others);

        transform.parent = map.poolPos;
        gameObject.SetActive(false);
    }
}
