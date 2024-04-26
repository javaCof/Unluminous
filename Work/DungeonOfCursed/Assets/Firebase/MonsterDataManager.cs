using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MonsterDataManager : MonoBehaviour
{
    public class Monster
    {
        public int type;
        public float hp;
        public float atk;
        public float def;
        public float spd;
        public string dec;
        public string name;

        public Monster(int type, float hp, float atk, float def, float spd, string dec, string name)
        {
            this.type = type;
            this.hp = hp;
            this.atk = atk;
            this.def = def;
            this.spd = spd;
            this.dec = dec;
            this.name = name;
        }
    }

    public Dictionary<int, Monster> monsters = new Dictionary<int, Monster>();

    public string id;
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
        var monsterData = new Monster(type, hp, atk, def, spd, dec, monsterName);
        string jsonData = JsonUtility.ToJson(monsterData);
        
        FirebaseManager.databaseReference.Child("Monster").Child(id).SetRawJsonValueAsync(jsonData);
        Debug.Log(jsonData);
    }

    [ContextMenu("몬스터 정보 불러오기")]
    void LoadMonsterData()
    {
        //var dataSnapshot = await FirebaseR.databaseReference.Child("Monster").GetValueAsync();
        //var monsterData = await FirebaseR.MonsterData();

        //await FirebaseManager.MonsterLoadData();
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

    [ContextMenu("키 정보 수정")]
    public async void RenameMonsterKey()
    {
        // 1. 데이터 가져오기
        var monsterDataSnapshot = await FirebaseManager.databaseReference.Child("Monster").Child("전설의 투구 ").GetValueAsync();

        // 2. "???"으로 새 키에 데이터 복사
        await FirebaseManager.databaseReference.Child("Monster").Child("전설의 투구").SetValueAsync(monsterDataSnapshot.Value);

        // 3. 기존 키 삭제
        await FirebaseManager.databaseReference.Child("Monster").Child("전설의 투구 ").RemoveValueAsync();

        Debug.Log("키 변경이 완료되었습니다.");
        
    }

    [ContextMenu("Value 정보 수정")]
     public async void RenameMonsterValue()
    {
        // 1. 데이터 가져오기 , monster.Child("???") 수정할 id만 바꿔주기
        var monsterDataSnapshot = await FirebaseManager.databaseReference.Child("Monster").Child("12010").GetValueAsync();
        var monsterData = monsterDataSnapshot.Value as Dictionary<string, object>;

        //ex)수정 부분이 dec 이면 
        if(monsterData != null && monsterData.ContainsKey("atk"))
        {
            monsterData["atk"] = 5;
            //monsterData["name"] = "알비노 그 긴거"; 스트링 형식은 "" 안에 적기
        }

        //monster.child("???") id부분만 바꿔서 수정
        await FirebaseManager.databaseReference.Child("Monster").Child("12010").SetValueAsync(monsterData);

        Debug.Log("Value값 변경이 완료되었습니다.");
        
    }

}
