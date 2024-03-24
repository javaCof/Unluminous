using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIInven : MonoBehaviour, IDropHandler
{
    public Inventory inven;
    public string uiItemTag;
    private Transform grid;

    private void Awake()
    {
        grid = GetComponentInChildren<GridLayoutGroup>().transform;
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject obj = eventData.pointerDrag;
        if (obj && obj.tag == uiItemTag && inven.CanAddItem())
        {
            UIInvenItem item = obj.GetComponent<UIInvenItem>();
            if (item.transferable)
            {
                item.inven.RemoveItem(item.item);
                inven.AddItem(item.item);
                item._parent = grid;
                item.inven = inven;
            }
        }
    }
}
