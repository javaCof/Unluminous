using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrSword : MonoBehaviour
{
    VrPlayer vrPlayer;

    private void Awake()
    {
        vrPlayer = GameObject.Find("VrPlayer").GetComponent<VrPlayer>();
    }


    //private void OnTriggerEnter(Collider other)
    //{
    //    //vr플레이어에 있는 vr 어택 함수 호출
    //    vrPlayer.VrAttack(other);
        
    //}


    private void OnCollisionEnter(Collision collision)
    {
        //vr플레이어에 있는 vr 어택 함수 호출
        vrPlayer.VrAttackAction(collision);
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
