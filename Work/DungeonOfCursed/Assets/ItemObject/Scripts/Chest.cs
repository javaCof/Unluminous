using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


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

        Debug.Log("»óÀÚ");
    }

    [ContextMenu("Gen Item")]
    [PunRPC]
    void Open_All()
    {
        isOpened = true;
        anim.SetTrigger("Open");

        targetItem =Instantiate(items[0], gameObject.transform);
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
