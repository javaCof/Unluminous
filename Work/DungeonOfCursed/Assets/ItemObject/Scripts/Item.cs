using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IPoolObject
{
    public int id;
    public int amount;
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
                elseItemData.Price = FirebaseManager.equips[this.id].price;
                elseItemData.Tooltip = FirebaseManager.equips[this.id].dec;
                elseItemData.Res = FirebaseManager.equips[this.id].res;
                elseItemData.Icon = FirebaseManager.equips[this.id].icon;
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

    public ItemData SetItemData(ItemData itemData, int id)
    {
        ItemData lastItemData = null;
        itemData.ID = id;

        if (itemData is CountableItemData)
        {
            itemData.Name = FirebaseManager.items[id].name;
            itemData.Price = FirebaseManager.items[id].price;
            itemData.Tooltip = FirebaseManager.items[id].dec;
            itemData.Res = FirebaseManager.items[id].res;
            itemData.Icon = FirebaseManager.items[id].icon;

            lastItemData = itemData;
        }
        else
        {
            ArmorItemData elseItemData = itemData as ArmorItemData;
            elseItemData.Name = FirebaseManager.equips[id].name;
            elseItemData.Price = FirebaseManager.equips[id].price;
            elseItemData.Tooltip = FirebaseManager.equips[id].dec;
            elseItemData.Res = FirebaseManager.equips[id].res;
            elseItemData.Icon = FirebaseManager.equips[id].icon;
            elseItemData.Atk = FirebaseManager.equips[id].atk;
            elseItemData.Def = FirebaseManager.equips[id].def;
            elseItemData.Speed = FirebaseManager.equips[id].speed;
            elseItemData.Hp = FirebaseManager.equips[id].hp;
            elseItemData.Set = FirebaseManager.equips[id].set;

            lastItemData = elseItemData;
        }
        return lastItemData;
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
