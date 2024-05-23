using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorItemData : EquipmentItemData
{
    public int Set { get; set; }
    public float Atk { get; set; }
    public float Def { get; set; }
    public float Hp { get; set; }
    public float Speed { get; set; }


    public override Item2 CreateItem()
    {
        return new ArmorItem(this);
    }

    public ArmorItemData(int id) : base(id) {}
}
