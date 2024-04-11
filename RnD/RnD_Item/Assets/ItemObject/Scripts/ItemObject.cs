using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
   public float genSpeed=1f;
    Rigidbody rigBody;

    ItemInfo info;
    int amount;

    private void Awake()
    {
        rigBody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Vector3 tmp = rigBody.velocity;
        tmp.y = genSpeed;
        rigBody.velocity = tmp;
    }

    private void Update()
    {
        if (transform.position.y>=1.5f)
        {
            Vector3 tmp = rigBody.velocity;
            tmp.y = 0f;
            rigBody.velocity = tmp;
        }
    }
}
