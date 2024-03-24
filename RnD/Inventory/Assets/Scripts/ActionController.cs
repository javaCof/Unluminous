using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionController : MonoBehaviour
{
    private bool canPickup = false;
     
    [SerializeField]
    private Inventory theInventory;

    private RaycastHit hitInfo;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckItem();
            CanPickUp();
        }
    }

    private void CheckItem()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitInfo, 15.0f, 1 << LayerMask.NameToLayer("Item")))
        {
            if (hitInfo.transform.tag == "Item")
            {
                Debug.Log("획득가능");
                ItemInfo();
            }
        }
    }

    private void ItemInfo()
    {
        canPickup = true;    
    }
    private void CanPickUp()
    {
        if (canPickup)
        {
            Debug.Log("획득");
            theInventory.AcquireItem(hitInfo.transform.GetComponent<ItemPickUp>().item);
        }
    }
}
