using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchPad : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public Vector2 dTouchPoint;

    public void OnBeginDrag(PointerEventData eventData) { }
    public void OnDrag(PointerEventData eventData) 
    {
        dTouchPoint = eventData.delta;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        dTouchPoint = Vector2.zero;
    }
}
