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
    //    //vr�÷��̾ �ִ� vr ���� �Լ� ȣ��
    //    vrPlayer.VrAttack(other);
        
    //}


    private void OnCollisionEnter(Collision collision)
    {
        //vr�÷��̾ �ִ� vr ���� �Լ� ȣ��
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
