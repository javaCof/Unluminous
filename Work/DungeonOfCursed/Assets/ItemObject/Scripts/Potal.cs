using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potal : MonoBehaviour, IPoolObject
{
    private MapGenerator map;

    private void Awake()
    {
        map = FindObjectOfType<MapGenerator>();
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

    public void OnPoolCreate(int id) { }
    public void OnPoolDisable() { }
    public void OnPoolEnable(Vector3 pos, Quaternion rot) { }
}
