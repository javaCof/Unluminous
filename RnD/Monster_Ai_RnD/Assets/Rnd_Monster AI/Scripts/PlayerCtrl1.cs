using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl1 : MonoBehaviour
{
    CharacterController controller;
    Vector3 moveDir;
    public float moveSpeed=5;
    public float gravity=1;

    
    
    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        moveDir = new Vector3(Input.GetAxis("Horizontal"), -1* (gravity * Time.deltaTime), Input.GetAxis("Vertical")) ;

        moveDir *= moveSpeed;


        controller.Move(moveDir * Time.deltaTime) ;


    }
}