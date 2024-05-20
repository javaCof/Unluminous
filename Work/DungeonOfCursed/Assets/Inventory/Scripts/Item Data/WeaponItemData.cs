using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Item_Weapon_", menuName = "Inventory System/Item Data/Weaopn", order = 1)]
public class WeaponItemData : EquipmentItemData
{
    /// <summary> 공격력 </summary>
    public int Damage => _damage;

    [SerializeField] private int _damage = 1;

    public override Item2 CreateItem()
    {
        return new WeaponItem(this);
    }

    public WeaponItemData(int id) : base(id) { }
}