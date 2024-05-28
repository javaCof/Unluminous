using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IPoolObject
{
    public bool isLocked = false;
    public List<Item> items;

    public float upSpeed;

    Item targetItem;

    public Animator anim;
    private MapGenerator map;
    private PhotonView pv;

    private bool isOpened = false;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        map = FindObjectOfType<MapGenerator>();
        pv = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (targetItem != null&& targetItem.transform.position.y <= 1.5f)
        {
            targetItem.transform.Translate(Time.deltaTime * (new Vector3(0, 0, -upSpeed)));
        }
    }

    public void Open()
    {
        if (isOpened) return;

        if (PhotonNetwork.inRoom) pv.RPC("Open_All", PhotonTargets.All);
        else Open_All();

        bool isItem = Random.Range(0, 2) == 0;
        int id = isItem ? Random.Range((int)DB_INFO.ITEM_BEGIN, (int)DB_INFO.ITEM_NEXT) :
            Random.Range((int)DB_INFO.EQUIP_BEGIN, (int)DB_INFO.EQUIP_NEXT);
        Item item = map.GenerateObject(id, transform.position, Quaternion.identity).GetComponent<Item>();
        item.amount = (id == (int)DB_INFO.ITEM_GOLD) ? Random.Range(1, 4) : 1;
        targetItem = item;
    }

    [ContextMenu("Gen Item")]
    [PunRPC]
    void Open_All()
    {
        isOpened = true;
        anim.SetBool("open", true);
    }

    [PunRPC]
    public void OnPoolCreate(int id)
    {
        if (PhotonNetwork.inRoom)
        {
            if (PhotonNetwork.isMasterClient) pv.RPC("OnPoolCreate", PhotonTargets.Others, id);
            transform.parent = map.poolPos;
            gameObject.SetActive(false);
        }
    }
    [PunRPC]
    public void OnPoolEnable(Vector3 pos, Quaternion rot)
    {
        if (PhotonNetwork.inRoom)
        {
            if (PhotonNetwork.isMasterClient) pv.RPC("OnPoolEnable", PhotonTargets.Others, pos, rot);
            transform.parent = map.objectPos;
            transform.position = pos;
            transform.rotation = rot;
            gameObject.SetActive(true);
        }

        isOpened = false;
        anim.SetBool("open", false);
    }
    [PunRPC]
    public void OnPoolDisable()
    {
        if (PhotonNetwork.inRoom)
        {
            if (PhotonNetwork.isMasterClient) pv.RPC("OnPoolDisable", PhotonTargets.Others);
            transform.parent = map.poolPos;
            gameObject.SetActive(false);
        }
    }
}
