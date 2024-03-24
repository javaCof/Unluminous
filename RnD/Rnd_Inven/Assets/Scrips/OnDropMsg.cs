using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnDropMsg : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("on drop" + this);
    }
}
