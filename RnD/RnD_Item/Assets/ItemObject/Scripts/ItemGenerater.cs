using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerater : MonoBehaviour
{
    public ItemObject item;


    public void GenerateItem()
    {
        Instantiate(item, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);

        
    }
}
