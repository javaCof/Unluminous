using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trader : MonoBehaviour, IPoolObject
{
    private MapGenerator map;
    private PhotonView pv;

    public List<Item> randomItemSlot1;
    public List<Item> randomItemSlot2;
    public List<Item> randomItemSlot3;

    private int[] itemIdxs = new int[3];

    private void Awake()
    {
        map = FindObjectOfType<MapGenerator>();
        pv = GetComponent<PhotonView>();
    }
    private void Reset()
    {
        itemIdxs[0] = Random.Range(0, randomItemSlot1.Count);
        itemIdxs[1] = Random.Range(0, randomItemSlot2.Count);
        itemIdxs[2] = Random.Range(0, randomItemSlot3.Count);

        if (PhotonNetwork.inRoom) pv.RPC("ResetItems", PhotonTargets.Others, itemIdxs);
    }
    [PunRPC] void ResetItems(int[] idxs)
    {
        itemIdxs[0] = idxs[0];
        itemIdxs[1] = idxs[1];
        itemIdxs[2] = idxs[2];
    }

    public void Trade()
    {
        if (PhotonNetwork.inRoom)
            pv.RPC("Trade_Master", PhotonTargets.MasterClient);
        else Trade_Master();
    }
    [PunRPC] void Trade_Master()
    {
        //show item list
    }

    [PunRPC] public void OnPoolCreate(int id)
    {
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

            if (PhotonNetwork.isMasterClient) Reset();
        }
        else Reset();
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
