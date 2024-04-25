using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    public Dictionary<int, Monster> monsters = new Dictionary<int, Monster>();

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

    [ContextMenu("몬스터 정보 저장")]
    void SaveMonsterData()
    {
        var monsterData = new Monster(id, type, hp, atk, def, spd, dec);
        string jsonData = JsonUtility.ToJson(monsterData);
        
        FirebaseManager.databaseReference.Child("Monster").Child(monsterName).SetRawJsonValueAsync(jsonData);
        Debug.Log(jsonData);
    }

    [ContextMenu("몬스터 정보 불러오기")]
    async void LoadMonsterData()
    {
        //var dataSnapshot = await FirebaseR.databaseReference.Child("Monster").GetValueAsync();
        //var monsterData = await FirebaseR.MonsterData();

        await FirebaseManager.MonsterLoadData();
        if(FirebaseManager.monsterDataLoad.HasChildren)
        {
            foreach(var monsterDataSnapshot in FirebaseManager.monsterDataLoad.Children)
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
        //var monsterData = FirebaseManager.monsterData["11002"] as Dictionary<string, object>;
        
        
        Debug.Log(FirebaseManager.monster["11002"]["name"]);

        // if((string)monsterData["name"] == "트리플 쉘 터틀")
        // {
        //     Debug.Log("같음");
        // }
    }

    [ContextMenu("정보 다시")]
    public async void RenameMonsterKey()
    {
        // 1. 데이터 가져오기
        var golemData = await FirebaseManager.databaseReference.Child("Equip").Child("Helmet").Child("전설의 투구 ").GetValueAsync();

        // 2. "???"으로 새 키에 데이터 복사
        await FirebaseManager.databaseReference.Child("Equip").Child("Helmet").Child("전설의 투구").SetValueAsync(golemData.Value);

        // 3. 기존 키 삭제
        await FirebaseManager.databaseReference.Child("Equip").Child("Helmet").Child("전설의 투구 ").RemoveValueAsync();

        Debug.Log("키 변경이 완료되었습니다.");
        
    }

}
