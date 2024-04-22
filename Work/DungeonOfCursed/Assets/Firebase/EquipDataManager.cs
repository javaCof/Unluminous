using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipDataManager : MonoBehaviour
{
    public class Equip
    {
        public int id;
        public int set;
        public float hp;
        public float atk;
        public float def;
        public float speed;
        public int price;
        public string dec;

        public Equip(int id, int set, float hp, float atk, float def, float speed, int price, string dec)
        {
            this.id = id;
            this.set = set;
            this.hp = hp;
            this.atk = atk;
            this.def = def;
            this.speed = speed;
            this.price = price;
            this.dec = dec;
        }
    }


    public string equipName;
    public int id;
    public int set;
    public float hp;
    public float atk;
    public float def;
    public float speed;
    public int price;
    public string dec;
    
    [ContextMenu("헬멧 데이터 저장")]
    void SaveHelmetData()
    {
        var data = new Equip(id, set, hp, atk, def, speed, price, dec);
        string jsonData = JsonUtility.ToJson(data);
        FirebaseRef.databaseReference.Child("Equip").Child("Helmet").Child(equipName).SetRawJsonValueAsync(jsonData);

        Debug.Log(data);
    }

    [ContextMenu("하의 데이터 저장")]
    void SavePantsData()
    {
        var data = new Equip(id, set, hp, atk, def, speed, price, dec);
        string jsonData = JsonUtility.ToJson(data);
        FirebaseRef.databaseReference.Child("Equip").Child("Pants").Child(equipName).SetRawJsonValueAsync(jsonData);

        Debug.Log(data);
    }

    [ContextMenu("상의 데이터 저장")]
    void SaveArmorData()
    {
        var data = new Equip(id, set, hp, atk, def, speed, price, dec);
        string jsonData = JsonUtility.ToJson(data);
        FirebaseRef.databaseReference.Child("Equip").Child("Armor").Child(equipName).SetRawJsonValueAsync(jsonData);

        Debug.Log(data);
    }

    [ContextMenu("세트 데이터 저장")]
    void SaveEquipData()
    {
        var data = new Equip(id, set, hp, atk, def, speed, price, dec);
        string jsonData = JsonUtility.ToJson(data);
        FirebaseRef.databaseReference.Child("Equip").Child("SetEquip").Child(equipName).SetRawJsonValueAsync(jsonData);

        Debug.Log(data);
    }
}
