using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitObject : MonoBehaviour, IPoolObject
{
    [HideInInspector] public bool isDead;
    [HideInInspector] public int id;
    [HideInInspector] public UnitStatInfo stat;
    [HideInInspector] public float curHP;
    public int roomNum;

    protected void SetStat()
    {
        stat = new UnitStatInfo();
        stat.HP = FirebaseManager.units[id].hp;
        stat.ATK = FirebaseManager.units[id].atk;
        stat.DEF = FirebaseManager.units[id].def;
        stat.SPD = FirebaseManager.units[id].spd;
    }

    public abstract void AttackAction();

    public abstract void OnPoolCreate(int id);
    public abstract void OnPoolEnable(Vector3 pos, Quaternion rot);
    public abstract void OnPoolDisable();
}
