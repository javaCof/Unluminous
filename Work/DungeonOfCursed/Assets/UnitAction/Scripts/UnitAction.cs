using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitAction : MonoBehaviour
{
    [HideInInspector] public bool isDead;
    [HideInInspector] public int id;
    [HideInInspector] public UnitStatInfo stat;
    [HideInInspector] public float curHP;
    [HideInInspector] public int roomNum;

    protected void SetStat()
    {
        //if (id == 200) return;

        //stat = new UnitStatInfo();
        //stat.HP = (float)FirebaseManager.monster[id.ToString()]["hp"];
        //stat.ATK = (float)FirebaseManager.monster[id.ToString()]["atk"];
        //stat.DEF = (float)FirebaseManager.monster[id.ToString()]["def"];
        //stat.SPD = (float)FirebaseManager.monster[id.ToString()]["spd"];

        stat = new UnitStatInfo();
        stat.HP = 100f;
        stat.ATK = 10f;
        stat.DEF = 10f;
        stat.SPD = 5f;
    }

    public abstract void AttackAction();
}
