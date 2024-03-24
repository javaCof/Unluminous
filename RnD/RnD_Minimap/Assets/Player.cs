using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveForce = 2f;
    public float maxSpeed = 3f;

    private Rigidbody rig;

    private void Awake()
    {
        rig = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Move();
    }

    void Move()
    {
        if (rig.velocity.magnitude > maxSpeed) return;

        float inpH = Input.GetAxis("Horizontal");
        float inpV = Input.GetAxis("Vertical");

        Vector3 moveDir = new Vector3(0, 0, inpV).normalized;
        //moveDir = transform.TransformDirection(moveDir).normalized;

        //rig.AddForce(moveDir * moveForce);
        transform.Translate(moveDir * moveForce);

        transform.Rotate(0, inpH*0.4f, 0);
    }
}
