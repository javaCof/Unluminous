using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public monsterCtrl monctrl;



    private void Awake()
    {
        monctrl = transform.root.GetComponent<monsterCtrl>();
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player" && monctrl.enemyMode == monsterCtrl.state.attack)
        {
            Debug.Log("플레이어 맞음!");
            //Destroy(col.gameObject);

        }
    }

   
    
}
