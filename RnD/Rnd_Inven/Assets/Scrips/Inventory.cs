using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public bool CanAddItem() => true;

    public void AddItem(InventoryItem item) { }
    public void RemoveItem(InventoryItem item) { }
}
