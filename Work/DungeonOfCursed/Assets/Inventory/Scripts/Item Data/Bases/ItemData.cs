using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : ScriptableObject
{
    //public int ID { get; private set; }
    //public int Price { get; private set; }
    //public string Name { get; private set; }
    //public string Tooltip { get; private set; }

    public int ID => _id;
    public int Price => _price;
    public string Name => _name;
    public string Tooltip => _tooltip;
    public Sprite IconSprite => _iconSprite;

    [SerializeField] private Sprite _iconSprite;
    [SerializeField] private int _id;
    [SerializeField] private int _price;
    [SerializeField] private string _name;
    [SerializeField] private string _tooltip;

    //[SerializeField] private GameObject _dropItemPrefab;

    //public ItemData(int id)
    //{
    //    ID = id;
    //    Price = FirebaseManager.items[id].price;
    //    Name = FirebaseManager.items[id].name;
    //    Tooltip = FirebaseManager.items[id].dec;
    //}

    public virtual Item2 CreateItem() { return new Item2(this); }
}