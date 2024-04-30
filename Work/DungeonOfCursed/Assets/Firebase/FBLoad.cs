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
    public class Item
    {
        static public int price;
        static public string dec;
        static public string name;
    }
    public class Equip
    {
        static public int set;
        static public float hp;
        static public float atk;
        static public float def;
        static public float speed;
        static public int price;
        static public string dec;
        static public string name;
    }

    static public int MonsterData(int id, int data, string name) => int.Parse(FirebaseManager.monster[id.ToString()][name].ToString());
    static public float MonsterData(int id, float data, string name) => float.Parse(FirebaseManager.monster[id.ToString()][name].ToString());
    static public bool MonsterData(int id, bool data, string name) => bool.Parse(FirebaseManager.monster[id.ToString()][name].ToString());
    static public string MonsterData(int id, string data, string name) => FirebaseManager.monster[id.ToString()][name].ToString();

    static public int ItemData(int id, int data, string name) => int.Parse(FirebaseManager.item[id.ToString()][name].ToString());
    static public float ItemData(int id, float data, string name) => float.Parse(FirebaseManager.item[id.ToString()][name].ToString());
    static public bool ItemData(int id, bool data, string name) => bool.Parse(FirebaseManager.item[id.ToString()][name].ToString());
    static public string ItemData(int id, string data, string name) => FirebaseManager.item[id.ToString()][name].ToString();


    private void Start()
    {
        float atk = MonsterData(10000, Monster.atk, nameof(Monster.atk));

        Debug.Log(atk);
    }
}
