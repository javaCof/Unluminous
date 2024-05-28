using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : MonoBehaviour
{
    public int ID { get; set; }
    public int Price { get; set; }
    public string Name { get; set; }
    public string Tooltip { get; set; }
    public string Icon { get; set; }
    public string Res { get; set; }

    public virtual Item2 CreateItem() { return new Item2(this); }
}