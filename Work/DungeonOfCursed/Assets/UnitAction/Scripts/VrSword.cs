using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrSword : MonoBehaviour
{
    Collider target;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag=="Enemy")
        {
            //�� ������ �������� �ִ� �Լ�
            target = collision.collider;

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
