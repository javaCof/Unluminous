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
        if (col.gameObject.tag == "Player")
        {
            Debug.Log("�÷��̾� ����!");
            //Destroy(col.gameObject);

        }
    }

   
    
}
