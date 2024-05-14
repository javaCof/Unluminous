using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrSword : MonoBehaviour
{
    VrPlayer vrPlayer;



    private void Awake()
    {
        vrPlayer = GetComponentInParent<VrPlayer>();
    }


    //private void OnTriggerEnter(Collider other)
    //{
    //    //vr플레이어에 있는 vr 어택 함수 호출
    //    vrPlayer.VrAttack(other);

    //}


    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Enemy":
                vrPlayer.VrAttackAction(collision);
                break;
            case "Chest":
                vrPlayer.VrOpenChest(collision);
                break;
            case "Trader":
                vrPlayer.VrTrade(collision);
                break;
            case "Item":
                vrPlayer.VrPickupItem(collision);
                break;
        }





    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
