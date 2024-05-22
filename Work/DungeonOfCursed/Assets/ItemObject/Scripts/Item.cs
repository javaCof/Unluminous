using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IPoolObject
{
    public int id;
    public int amount;

    public Inventory _inventory;

    private MapGenerator map;
    private PhotonView pv;
    private ItemData itemData;

    private void Awake()
    {
        map = FindObjectOfType<MapGenerator>();
        pv = GetComponent<PhotonView>();
        itemData = this.gameObject.GetComponent<ItemData>();
    }

    public void Pickup()
    {
        if(itemData != null)
        {
            itemData.ID = this.id;

            if(itemData is CountableItemData)
            {
                Debug.Log(itemData.Name + "È¹µæ");
                _inventory.Add(itemData, this.amount);
            }
            else
                _inventory.Add(itemData);
        }

        if (PhotonNetwork.inRoom) pv.RPC("RemoveObject", PhotonTargets.MasterClient);
        else RemoveObject();
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
