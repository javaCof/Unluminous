using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UnitDataManager : MonoBehaviour
{
    public class Unit
    {
        public int type;
        public float hp;
        public float atk;
        public float def;
        public float spd;
        public string dec;
        public string name;

        public Unit(int type, float hp, float atk, float def, float spd, string dec, string name)
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

    public Dictionary<int, Unit> units = new Dictionary<int, Unit>();

    public string id;
    public int type;
    public float hp;
    public int atk;
    public float def;
    public float spd;
    public string dec;
    public string unitName;

    //private DatabaseReference databaseReference;
    void Awake()
    {
        //await FirebaseR.LoadData();
    }

    [ContextMenu("몬스터 정보 저장")]
    void SaveUnitData()
    {
        var unitData = new Unit(type, hp, atk, def, spd, dec, unitName);
        string jsonData = JsonUtility.ToJson(unitData);
        
        FirebaseManager.databaseReference.Child("Unit").Child(id).SetRawJsonValueAsync(jsonData);
        Debug.Log(jsonData);
    }

    [ContextMenu("몬스터 정보 불러오기")]
    void LoadUnitData()
    {
        //var dataSnapshot = await FirebaseR.databaseReference.Child("Unit").GetValueAsync();
        //var unitData = await FirebaseR.UnitData();

        //await FirebaseManager.UnitLoadData();
        if(FirebaseManager.unitDataLoad.HasChildren)
        {
            foreach(var unitDataSnapshot in FirebaseManager.unitDataLoad.Children)
            {
                var unitName = unitDataSnapshot.Key;
                var unitValues = unitDataSnapshot.Value as Dictionary<string, object>;
                
                if(unitValues != null)
                {
                    var dataString = "";
                    
                    foreach(var unit in unitValues)
                    {
                        dataString += unit.Key + " " + unit.Value.ToString() + "\n";
                    }
                    
                    Debug.Log(unitName + " " + dataString);
                }
            }
        }
        else
        {
           Debug.Log("No Unit found.");
        }
    }

    [ContextMenu("속성 한가지 불러오기")]
    void Boolluogi()
    {
        //var unitData = FirebaseManager.unitData["11002"] as Dictionary<string, object>;
        //float a = (float)FirebaseManager.unit["10000"]["atk"];
        
        //Debug.Log(a);

        // if((string)unitData["name"] == "트리플 쉘 터틀")
        // {
        //     Debug.Log("같음");
        // }
    }

    [ContextMenu("키 정보 수정")]
    public async void RenameUnitKey()
    {
        // 1. 데이터 가져오기
        var unitDataSnapshot = await FirebaseManager.databaseReference.Child("Equip").Child("Pants").Child("2200").GetValueAsync();

        // 2. "???"으로 새 키에 데이터 복사
        await FirebaseManager.databaseReference.Child("Equip").Child("2200").SetValueAsync(unitDataSnapshot.Value);

        // 3. 기존 키 삭제
        await FirebaseManager.databaseReference.Child("Equip").Child("Pants").Child("2200").RemoveValueAsync();

        Debug.Log("키 변경이 완료되었습니다.");
        
    }

    [ContextMenu("Value 정보 수정")]
     public async void RenameUnitValue()
    {
        // 1. 데이터 가져오기 , unit.Child("???") 수정할 id만 바꿔주기
        var unitDataSnapshot = await FirebaseManager.databaseReference.Child("Unit").Child("12010").GetValueAsync();
        var unitData = unitDataSnapshot.Value as Dictionary<string, object>;

        //ex)수정 부분이 dec 이면 
        if(unitData != null && unitData.ContainsKey("atk"))
        {
            unitData["atk"] = 5;
            //unitData["name"] = "알비노 그 긴거"; 스트링 형식은 "" 안에 적기
        }

        //unit.child("???") id부분만 바꿔서 수정
        await FirebaseManager.databaseReference.Child("Unit").Child("12010").SetValueAsync(unitData);

        Debug.Log("Value값 변경이 완료되었습니다.");
        
    }

}
