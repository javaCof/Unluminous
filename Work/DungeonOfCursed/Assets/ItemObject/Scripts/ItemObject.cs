using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour, IPoolObject
{
    public int id;
    public int amount;

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
            if (!PhotonNetwork.inRoom || oth.GetComponent<PhotonView>().isMine)
            {
                //Inventory inven = oth.GetComponent<Inventory>().AddItem(id, amount);

                pv.RPC("RemoveObject", PhotonTargets.MasterClient);
            }
        }
    }

    [PunRPC] void RemoveObject()
    {
        map.RemoveObject(gameObject, id);
    }

    [PunRPC] public void OnPoolCreate(int id)
    {
        this.id = id;

        if (PhotonNetwork.inRoom)
        {
            if (PhotonNetwork.isMasterClient) pv.RPC("OnPoolCreate", PhotonTargets.Others, id);
            transform.parent = map.poolPos;
            gameObject.SetActive(false);
        }
    }
    [PunRPC] public void OnPoolEnable(Vector3 pos, Quaternion rot)
    {
        if (PhotonNetwork.inRoom)
        {
            if (PhotonNetwork.isMasterClient) pv.RPC("OnPoolEnable", PhotonTargets.Others, pos, rot);
            transform.parent = map.objectPos;
            transform.position = pos;
            transform.rotation = rot;
            gameObject.SetActive(true);
        }
    }
    [PunRPC] public void OnPoolDisable()
    {
        if (PhotonNetwork.inRoom)
        {
            if (PhotonNetwork.isMasterClient) pv.RPC("OnPoolDisable", PhotonTargets.Others);
            transform.parent = map.poolPos;
            gameObject.SetActive(false);
        }
    }
}
