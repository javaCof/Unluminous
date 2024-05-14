using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item_Portion_", menuName = "Inventory System/Item Data/Portion", order = 3)]
public class PortionItemData : CountableItemData
{
    public float Value => _value;
    [SerializeField] private float _value;
    public override Item2 CreateItem()
    {
        return new PortionItem(this);
    }
}
