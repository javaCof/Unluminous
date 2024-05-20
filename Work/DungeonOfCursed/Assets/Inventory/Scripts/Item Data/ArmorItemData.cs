using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorItemData : EquipmentItemData
{
    public int Defence => _defence;

    [SerializeField] private int _defence = 1;
    public override Item2 CreateItem()
    {
        return new ArmorItem(this);
    }

    public ArmorItemData(int id) : base(id) { }
}
