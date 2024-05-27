using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trader : MonoBehaviour, IPoolObject
{
    private MapGenerator map;
    private PhotonView pv;
    private Trade_UI trade_UI;
    private Inventory _invetory;

    public List<Item> randomItemSlot1;
    public List<Item> randomItemSlot2;
    public List<Item> randomItemSlot3;

    public ItemData[] btnItem = new ItemData[3];

    private int[] itemIdxs = new int[3];

    private void Awake()
    {
        map = FindObjectOfType<MapGenerator>();
        pv = GetComponent<PhotonView>();
        trade_UI = FindObjectOfType<Trade_UI>();
        _invetory = FindAnyObjectByType<Inventory>();
    }

    void Start()
    {
       trade_UI.gameObject.SetActive(false);
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
        Debug.Log("트레이드");

        GameManager.Instance.player.controllable = false;
        GameManager.Instance.enablePopup = true;
        
        trade_UI.gameObject.SetActive(true);
        UpdateTradeUI();

        
    }

    void UpdateTradeUI()
    {
        for (int i = 0; i < itemIdxs.Length; i++)
        {
            ItemData itemData = null;
            Item item = null;
            switch (i)
            {
                case 0:
                    item = randomItemSlot1[itemIdxs[i]];
                    itemData = randomItemSlot1[itemIdxs[i]].GetComponent<ItemData>();
                    itemData = item.SetItemData(itemData, item.id);
                    btnItem[i] = itemData;
                    break;
                case 1:
                    item = randomItemSlot2[itemIdxs[i]];
                    itemData = randomItemSlot2[itemIdxs[i]].GetComponent<ItemData>();
                    itemData = item.SetItemData(itemData, item.id);
                    btnItem[i] = itemData;
                    break;
                case 2:
                    item = randomItemSlot3[itemIdxs[i]];
                    itemData = randomItemSlot3[itemIdxs[i]].GetComponent<ItemData>();
                    itemData = item.SetItemData(itemData, item.id);
                    btnItem[i] = itemData;
                    break;
            }

            if (item != null)
            {
                trade_UI.SetItemImg(Resources.Load<Sprite>(itemData.Icon), i);
                trade_UI.SetGold(itemData, i);
            }
        }

        for(int i =0; i < 3; i++)
         Debug.Log(btnItem[i].Name);
    }

    public void OnBtnItem(int i)
    {
        Debug.Log("on btn");
        _invetory.Add(btnItem[i]);
    }

    public void OnClose()
    {
        Debug.Log("닫기");
        trade_UI.gameObject.SetActive(false);
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
