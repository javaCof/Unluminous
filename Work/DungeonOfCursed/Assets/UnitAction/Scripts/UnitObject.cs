using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitObject : MonoBehaviour, IPoolObject
{
    [HideInInspector] public bool isDead;
    [HideInInspector] public int id;
    [HideInInspector] public UnitStatInfo stat;
    public float curHP;
    [HideInInspector] public int roomNum;

    protected void SetStat()
    {
        stat = new UnitStatInfo();
        stat.HP = FirebaseManager.units[id].hp;
        stat.ATK = FirebaseManager.units[id].atk;
        stat.DEF = FirebaseManager.units[id].def;
        stat.SPD = FirebaseManager.units[id].spd;
    }

    public abstract void Attack();
    public abstract void OnHit(float dmg);
    protected abstract IEnumerator Dead(float delay);

    public abstract void OnPoolCreate(int id);
    public abstract void OnPoolEnable(Vector3 pos, Quaternion rot);
    public abstract void OnPoolDisable();
}
