using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Monster monctrl;



    private void Awake()
    {
        monctrl = transform.root.GetComponent<Monster>();
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player" && monctrl.state == Monster.State.attack)
        {
            Debug.Log("�÷��̾� ����!");
            //Destroy(col.gameObject);

        }
    }

   
    
}
