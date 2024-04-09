using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Chest : MonoBehaviour
{
    Animator anim;
    public UnityEvent OnOpen;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    public void Open()
    {
        anim.SetTrigger("Open");
        OnOpen.Invoke();
    }
}
