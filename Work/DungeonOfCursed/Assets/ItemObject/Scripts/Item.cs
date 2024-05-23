using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IPoolObject
{
    [HideInInspector] public int id;
    [HideInInspector] public int amount;

    private Inventory _inventory;

    private MapGenerator map;
    private ItemData itemData;

    private void Awake()
    {
        map = FindObjectOfType<MapGenerator>();
        itemData = this.gameObject.GetComponent<ItemData>();
        _inventory = FindObjectOfType<Inventory>();
    }

    public void Pickup()
    {
        if(itemData != null)
        {
            itemData.ID = this.id;

            if(itemData is CountableItemData)
            {
                Debug.Log(itemData.Name + "È¹µæ");
                _inventory.Add(itemData, this.amount);
            }
            else
                _inventory.Add(itemData);
        }

        RemoveObject();
    }
    [PunRPC] void RemoveObject()
    {
        map.RemoveObject(gameObject, id);
    }

    [PunRPC] public void OnPoolCreate(int id)
    {
        this.id = id;
    }
    [PunRPC] public void OnPoolEnable(Vector3 pos, Quaternion rot) { }
    [PunRPC] public void OnPoolDisable() { }
}
