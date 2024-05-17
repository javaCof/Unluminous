using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountableItemData : ItemData
{
    public int MaxAmount => _maxAmount;
    [SerializeField] private int _maxAmount = 99;

    //public CountableItemData(int id) : base(id) {}
}
