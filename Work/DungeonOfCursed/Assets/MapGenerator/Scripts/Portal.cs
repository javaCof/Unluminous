using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public MapGenerator mapGen;

    private void Awake()
    {
        mapGen = GameObject.Find("MapGenerator").GetComponent<MapGenerator>();
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.tag =="Player")
        {
            Invoke("Teleport", 3);
            
        }
    }

    void Teleport()
    {
        mapGen.ResetLevel();
    }
}
