using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountableItemData : ItemData
{
    public int MaxAmount = 99;
    public CountableItemData(int id) : base(id) {
    }

    public override Item2 CreateItem()
    {
        return new CountableItem(this);
    }
}
