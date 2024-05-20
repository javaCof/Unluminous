using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EquipmentItemData : ItemData
{
    public int MaxDurability => _maxDurability;

    [SerializeField] private int _maxDurability = 100;

    public EquipmentItemData(int id) : base(id) { }

    public override Item2 CreateItem()
    {
        return new EquipmentItem(this);
    }
}
