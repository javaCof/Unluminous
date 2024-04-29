using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchPad : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public Vector2 dTouchPoint;
    int touchId = -1;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (touchId == -1) touchId = eventData.pointerId;
    }
    public void OnDrag(PointerEventData eventData) { }
    public void OnEndDrag(PointerEventData eventData) => touchId = -1;

    private void Update()
    {
        if (touchId != -1)
            dTouchPoint = Input.touches[touchId].deltaPosition;
        else dTouchPoint = Vector2.zero;
    }
}
