using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon1 : MonoBehaviour
{
 
    
    

    private void Awake()
    {
       
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag== "Player")
        {
            Debug.Log("플레이어 맞음!");
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
