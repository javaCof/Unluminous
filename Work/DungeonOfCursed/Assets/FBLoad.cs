using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBLoad : MonoBehaviour
{
    public class Monster
    {
        static public float hp;
        static public float atk;
        static public float def;
        static public float spd;
        static public int type;
        static public string dec;
    }

    static public T MonsterData<T>(int id, T data, string name)
    {
        foreach (var g in FirebaseManager.monster)
        {
            Debug.Log(g.Key);
        }
        foreach (var r in FirebaseManager.monster[id.ToString()])
        Debug.Log(r.Value);



        return (T)FirebaseManager.monster[id.ToString()][name];
    }

    private void Start()
    {
        float atk = MonsterData(10000, Monster.atk, nameof(Monster.atk));

    }
}
