using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IPoolObject
{
    [HideInInspector] public int id;
    [HideInInspector] public int amount;
    //[HideInInspector] public int price;
    //[HideInInspector] public string name;
    //[HideInInspector] public string dec;
    //[HideInInspector] public string res;
    //[HideInInspector] public string icon;

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
        if (itemData != null)
        {
            Debug.Log("¾ÆÀÌÅÛ ÇÈ¾÷");

            itemData.ID = this.id;

            if (itemData is CountableItemData)
            {
                itemData.Name = FirebaseManager.items[this.id].name;
                itemData.Price = FirebaseManager.items[this.id].price;
                itemData.Tooltip = FirebaseManager.items[this.id].dec;
                itemData.Res = FirebaseManager.items[this.id].res;
                itemData.Icon = FirebaseManager.items[this.id].icon;

                Debug.Log(itemData.Name + "Ä«¿îÅÍºí È¹µæ");
                _inventory.Add(itemData, this.amount);
            }
            else
            {
                ArmorItemData elseItemData = itemData as ArmorItemData;
                elseItemData.Name = FirebaseManager.equips[this.id].name;
                elseItemData.Atk = FirebaseManager.equips[this.id].atk;
                elseItemData.Def = FirebaseManager.equips[this.id].def;
                elseItemData.Speed = FirebaseManager.equips[this.id].speed;
                elseItemData.Hp = FirebaseManager.equips[this.id].hp;
                elseItemData.Set = FirebaseManager.equips[this.id].set;

                Debug.Log(elseItemData.Name + "È¹µæ");
                _inventory.Add(elseItemData);
            }

        }
        else
            Debug.Log("¾ÆÀÌÅÛ ¾ø´Âµ¥");

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
