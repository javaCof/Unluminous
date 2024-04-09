using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitCharater : MonoBehaviour
{
    public bool isDead;
    public UnitStatInfo stat;
    public float curHP;

    public string model_name;
    public string icon_name;

    public int roomNum;

    public abstract void Hit(UnitCharater other);
}
