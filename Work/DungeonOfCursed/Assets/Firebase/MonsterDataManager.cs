using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
public class MonsterDataManager : MonoBehaviour
{
    public class Monster
    {
        public int id;
        public int type;
        public float hp;
        public float atk;
        public float def;
        public float spd;
        public string dec;

        public Monster(int id, int type, float hp, float atk, float def, float spd, string dec)
        {
            this.id = id;
            this.type = type;
            this.hp = hp;
            this.atk = atk;
            this.def = def;
            this.spd = spd;
            this.dec = dec;
        }
    }

    public int id;
    public int type;
    public float hp;
    public int atk;
    public float def;
    public float spd;
    public string dec;
    public string monsterName;

    //private DatabaseReference databaseReference;
    void Awake()
    {
        //databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    [ContextMenu("몬스터 정보 저장")]
    void SaveMonsterData()
    {
        var monsterData = new Monster(id, type, hp, atk, def, spd, dec);
        string jsonData = JsonUtility.ToJson(monsterData);
        
        FirebaseR.databaseReference.Child("Monster").Child(monsterName).SetRawJsonValueAsync(jsonData);
        
    }

    [ContextMenu("몬스터 정보 불러오기")]
    async void LoadMonsterData()
    {
        var dataSnapshot = await FirebaseR.databaseReference.Child("Monster").GetValueAsync();

        if(dataSnapshot.HasChildren)
        {
            foreach(var monsterDataSnapshot in dataSnapshot.Children)
            {
                var monsterName = monsterDataSnapshot.Key;
                var monsterValues = monsterDataSnapshot.Value as Dictionary<string, object>;
                
                if(monsterValues != null)
                {
                    var dataString = "";
                    
                    foreach(var monster in monsterValues)
                    {
                        dataString += monster.Key + " " + monster.Value.ToString() + "\n";
                    }
                    
                    Debug.Log(monsterName + " " + dataString);
                }
            }
        }
        else
        {
            Debug.Log("No Monster found.");
        }
    }
}
