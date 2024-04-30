using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using System.Threading.Tasks;

public class FirebaseManager
{
    public static DatabaseReference databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    public static DataSnapshot monsterDataLoad;
    public static DataSnapshot itemDataLoad;
    public static DataSnapshot equipDataLoad;
    public static DataSnapshot setDataLoad;

    //FIrebaseManager.monster[""][""];
    public static Dictionary<string, Dictionary<string, object>> monster = new Dictionary<string, Dictionary<string, object>>();
    
    //FIrebaseManager.item[""][""];
    public static Dictionary<string, Dictionary<string, object>> item = new Dictionary<string, Dictionary<string, object>>();

    //FIrebaseManager.equip[""][""];
    public static Dictionary<string, Dictionary<string, object>> equip = new Dictionary<string, Dictionary<string, object>>();

    //FIrebaseManager.setEquip[""][""];
    public static Dictionary<string, Dictionary<string, object>> setEquip = new Dictionary<string, Dictionary<string, object>>();
    
    

    public static async Task MonsterLoadData()
    {
        monsterDataLoad = await databaseReference.Child("Monster").GetValueAsync();

        var monsterData = monsterDataLoad.Value as Dictionary<string, object>;
        foreach (var monsterKey in monsterData.Keys)
        {
            var monsters = monsterData[monsterKey] as Dictionary<string, object>;
            if (monsters != null)
            {
                monster.Add(monsterKey, monsters);
            }
        }

    }

    public static async Task ItemLoadData()
    {
        itemDataLoad = await databaseReference.Child("Item").GetValueAsync();

        var itemData = itemDataLoad.Value as Dictionary<string, object>;
        foreach (var itemKey in itemData.Keys)
        {
            var items = itemData[itemKey] as Dictionary<string, object>;
            if (items != null)
            {
                item.Add(itemKey, items);
            }
        }
    }

    public static async Task EquipLoadData()

    {
        equipDataLoad = await databaseReference.Child("Equip").GetValueAsync();

        setDataLoad = await databaseReference.Child("Equip").Child("SetEquip").GetValueAsync();

        var equipData = equipDataLoad.Value as Dictionary<string, object>;
        var setData = setDataLoad.Value as Dictionary<string, object>;

        foreach (var equipKey in equipData.Keys)
        {
            var equips = equipData[equipKey] as Dictionary<string, object>;
            if (equips != null)
            {
                equip.Add(equipKey, equips);
            }
        }

        foreach (var setEquipKey in setData.Keys)
        {
            var sets = setData[setEquipKey] as Dictionary<string, object>;
            if (sets != null)
            {
                setEquip.Add(setEquipKey, sets);
            }
        }
    }
}