using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Item_Armor_", menuName = "Inventory System/Item Data/Armor", order = 2)]
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
