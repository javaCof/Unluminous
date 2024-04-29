using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public int id;
    public int amount;

    private MapGenerator map;
    private PhotonView pv;

    private void OnTriggerEnter(Collider oth)
    {
        if (oth.tag == "Player")
        {
            if (!PhotonNetwork.inRoom || oth.GetComponent<PhotonView>().isMine)
            {
                //Inventory inven = oth.GetComponent<Inventory>().AddItem(id, amount);
                map.RemoveObject(gameObject, id);
            }
        }
    }

    [PunRPC]
    public void OnPoolCreate()
    {
        if (pv.isMine)
            pv.RPC("OnPoolCreate", PhotonTargets.Others);

        transform.parent = map.poolPos;
        gameObject.SetActive(false);
    }
    [PunRPC]
    public void OnPoolEnable(Vector3 pos, Quaternion rot)
    {
        if (pv.isMine)
            pv.RPC("OnPoolEnable", PhotonTargets.Others, pos, rot);

        transform.parent = map.objectPos;
        transform.localPosition = pos;
        transform.localRotation = rot;
        gameObject.SetActive(true);
    }
    [PunRPC]
    public void OnPoolDisable()
    {
        if (pv.isMine)
            pv.RPC("OnPoolDisable", PhotonTargets.Others);

        transform.parent = map.poolPos;
        gameObject.SetActive(false);
    }
}
