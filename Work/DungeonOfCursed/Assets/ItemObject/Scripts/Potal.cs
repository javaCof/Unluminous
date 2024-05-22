using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potal : MonoBehaviour, IPoolObject
{
    public Transform icon;

    private MapGenerator map;

    private void Awake()
    {
        map = FindObjectOfType<MapGenerator>();
    }

    private void Update()
    {
        Player target = GameManager.Instance.player;
        if (target != null)
        {
            Vector3 pos = target.transform.position;
            pos.y = icon.position.y;
            icon.LookAt(pos);
        }
    }

    private void OnTriggerEnter(Collider oth)
    {
        if (oth.tag == "Player")
        {
            SoundManager.instance.PlaySfx("portal");
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
