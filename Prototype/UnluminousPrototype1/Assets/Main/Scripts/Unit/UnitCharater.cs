using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitCharater : MonoBehaviour
{
    public bool isDead;

    public UnitStatInfo stat;
    public float curHP;

    public int roomNum;

    public abstract void Hit(UnitCharater other);
}
