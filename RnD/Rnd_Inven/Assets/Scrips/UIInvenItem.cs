using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIInvenItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Inventory inven;
    public InventoryItem item;
    public bool transferable;
    private Transform canvas;
    private Image img;
    public Transform _parent;

    private void Awake()
    {
        inven = GetComponentInParent<UIInven>().inven;
        canvas = GetComponentInParent<Canvas>().transform;
        img = GetComponent<Image>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        img.raycastTarget = false;
        _parent = transform.parent;
        transform.parent = canvas;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.parent = _parent;
        transform.localPosition = Vector3.zero;
        img.raycastTarget = true;
    }
}
