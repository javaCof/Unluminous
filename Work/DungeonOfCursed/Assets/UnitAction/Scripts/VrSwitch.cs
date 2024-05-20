using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrSwitch : MonoBehaviour
{
    public Transform actor;
    public Transform avatar;

    private PhotonView pv;

    private void Awake()
    {
        pv = actor.GetComponent<PhotonView>();
    }
    private void Start()
    {
        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            actor.gameObject.SetActive(true);
            avatar.gameObject.SetActive(false);
        }
        else
        {
            actor.gameObject.SetActive(false);
            avatar.gameObject.SetActive(true);
        }
    }
}
