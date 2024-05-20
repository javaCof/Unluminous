using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortionItemData : CountableItemData
{
    public float Value => _value;
    [SerializeField] private float _value;
    public override Item2 CreateItem()
    {
        return new PortionItem(this);
    }

    public PortionItemData(int id) : base(id) { }
}
