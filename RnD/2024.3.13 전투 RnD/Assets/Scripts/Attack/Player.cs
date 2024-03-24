using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Weapon weapon;
    Animator anim;

    private void Awake()
    {
        weapon = GetComponent<Weapon>();
        anim = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("Attack");
            Debug.Log("어택 모션");
        }
    }
}
