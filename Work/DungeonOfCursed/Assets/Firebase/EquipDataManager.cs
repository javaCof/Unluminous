using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;

public class EquipDataManager : MonoBehaviour
{
    public class Equip
    {
        public int id;
        public float hp;
        public float atk;
        public float def;
        public float speed;
        public int price;

        public Equip(int id, float hp, float atk, float def, float speed, int price)
        {
            this.id = id;
            this.hp = hp;
            this.atk = atk;
            this.def = def;
            this.speed = speed;
            this.price = price;
        }
    }

    private DatabaseReference databaseReference;

    public string equipName;
    public int id;
    public float hp;
    public float atk;
    public float def;
    public float speed;
    public int price;
    
    void Awake()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    [ContextMenu("헬멧 데이터 저장")]
    void SaveHelmetData()
    {
        var data = new Equip(id, hp, atk, def, speed, price);
        string jsonData = JsonUtility.ToJson(data);
        databaseReference.Child("Equip").Child("Helmet").Child(equipName).SetRawJsonValueAsync(jsonData);

        Debug.Log(data);
    }

    [ContextMenu("하의 데이터 저장")]
    void SavePantsData()
    {
        var data = new Equip(id, hp, atk, def, speed, price);
        string jsonData = JsonUtility.ToJson(data);
        databaseReference.Child("Equip").Child("Pants").Child(equipName).SetRawJsonValueAsync(jsonData);

        Debug.Log(data);
    }

    [ContextMenu("상의 데이터 저장")]
    void SaveEquipData()
    {
        var data = new Equip(id, hp, atk, def, speed, price);
        string jsonData = JsonUtility.ToJson(data);
        databaseReference.Child("Equip").Child("Armor").Child(equipName).SetRawJsonValueAsync(jsonData);

        Debug.Log(data);
    }
}
