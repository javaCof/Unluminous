using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData
{
    public int ID { get; private set; }
    public int Price { get; private set; }
    public string Name { get; private set; }
    public string Tooltip { get; private set; }
    public Sprite IconSprite { get; private set; }

    //[SerializeField] private GameObject _dropItemPrefab;

    public ItemData(int id)
    {
        ID = id;
        Price = FirebaseManager.items[id].price;
        Name = FirebaseManager.items[id].name;
        Tooltip = FirebaseManager.items[id].dec;
    }

    public virtual Item2 CreateItem() { return new Item2(this); }
}