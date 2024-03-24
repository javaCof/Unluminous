using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
 
    
    

    private void Awake()
    {
       
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag== "Enemy")
        {
            Debug.Log("몬스터에 닿음");
            Destroy(col.gameObject);
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
