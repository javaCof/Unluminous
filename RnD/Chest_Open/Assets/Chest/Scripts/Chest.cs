using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    Animator anim;

   

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    public void Open()
    {
        anim.SetTrigger("Open");
    }

    public void Close()
    {
        anim.SetTrigger("Close");
    }
}
