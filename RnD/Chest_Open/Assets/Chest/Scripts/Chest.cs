using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Chest : MonoBehaviour
{
    Animator anim;

    public UnityEvent ev;

    private void Awake()
    {
        anim = GetComponent<Animator>();

        if (ev!=null)
        {
            ev.Invoke();
        }
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
