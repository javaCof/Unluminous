using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EquipmentItem : Item2
{
    public EquipmentItemData EquipmentData { get; private set; }
    public int Durability
    {
        get => _durability;
        set
        {
            if (value < 0) value = 0;
            if (value > EquipmentData.MaxDurability)
                value = EquipmentData.MaxDurability;

            _durability = value;
        }
    }
    private int _durability;

    public EquipmentItem(EquipmentItemData data) : base(data)
    {
        EquipmentData = data;
        Durability = data.MaxDurability;
    }
}