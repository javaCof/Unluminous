using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitAction : MonoBehaviour
{
    [HideInInspector] public bool isDead;
    [HideInInspector] public int id;
    [HideInInspector] public UnitStatInfo stat;
    [HideInInspector] public float curHP;
    public int roomNum;

    protected void SetStat()
    {
        stat = new UnitStatInfo();

        if (id == 200) return;

        id = 10000;

        stat.HP = FirebaseManager.monsters[id].hp;
        stat.ATK = FirebaseManager.monsters[id].atk;
        stat.DEF = FirebaseManager.monsters[id].def;
        stat.SPD = FirebaseManager.monsters[id].spd;
    }

    public abstract void AttackAction();
}
