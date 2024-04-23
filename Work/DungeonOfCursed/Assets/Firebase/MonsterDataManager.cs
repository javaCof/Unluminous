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
        //await FirebaseR.LoadData();
    }

    async void Start()
    {
        await FirebaseR.LoadData();
        Debug.Log(FirebaseR.equipDataLoad.Child("Helmet").Child("평범한 투구").Child("id").Value);
    }

    [ContextMenu("몬스터 정보 저장")]
    void SaveMonsterData()
    {
        var monsterData = new Monster(id, type, hp, atk, def, spd, dec);
        string jsonData = JsonUtility.ToJson(monsterData);
        
        FirebaseR.databaseReference.Child("Monster").Child(monsterName).SetRawJsonValueAsync(jsonData);
        Debug.Log(jsonData);
    }

    [ContextMenu("몬스터 정보 불러오기")]
    async void LoadMonsterData()
    {
        //var dataSnapshot = await FirebaseR.databaseReference.Child("Monster").GetValueAsync();
        //var monsterData = await FirebaseR.MonsterData();

        await FirebaseR.LoadData();
        if(FirebaseR.monsterDataLoad.HasChildren)
        {
            foreach(var monsterDataSnapshot in FirebaseR.monsterDataLoad.Children)
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

    [ContextMenu("골렘 정보 불러오기")]
    void Golem()
    {
        var data = FirebaseR.monsterDataLoad.Value as Dictionary<string, object>;
        if (data != null && data.ContainsKey("골렘"))
        {
            var monsterData = data["Monster"] as Dictionary<string, object>;
            if (monsterData != null && monsterData.ContainsKey("골렘"))
            {
                var idValue = monsterData["id"];
                Debug.Log(monsterData["id"]);
            }
            else
            {
                Debug.Log("골렘의 id 값이 없습니다.");
            }
        }
        else
        {
            Debug.Log("골렘 데이터가 없습니다.");
        }
    }

    [ContextMenu("몬스터 정보 다시")]
    public async void RenameMonsterKey()
    {
        // 1. "골렘" 데이터 가져오기
        var golemData = await FirebaseR.databaseReference.Child("Equip").Child("Helmet").Child("전설의 투구 ").GetValueAsync();

        // 2. "Golem"으로 새 키에 데이터 복사
        await FirebaseR.databaseReference.Child("Equip").Child("Helmet").Child("전설의 투구").SetValueAsync(golemData.Value);

        // 3. 기존 "골렘" 키 삭제
        await FirebaseR.databaseReference.Child("Equip").Child("Helmet").Child("전설의 투구 ").RemoveValueAsync();

        Debug.Log("키 변경이 완료되었습니다.");
    }

}
