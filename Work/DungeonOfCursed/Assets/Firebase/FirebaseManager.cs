    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using System.Threading.Tasks;

public class FirebaseManager
{
    public static DatabaseReference databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    public static DataSnapshot monsterDataLoad;
    public static DataSnapshot itemDataLoad;
    public static DataSnapshot equipDataLoad;
    public static Dictionary<string, Dictionary<string, object>> monster = new Dictionary<string, Dictionary<string, object>>();
    public static Dictionary<string, Dictionary<string, object>> item = new Dictionary<string, Dictionary<string, object>>();
    public static Dictionary<string, Dictionary<string, object>> equip = new Dictionary<string, Dictionary<string, object>>();

    public static async Task MonsterLoadData()
    {
        monsterDataLoad = await databaseReference.Child("Monster").GetValueAsync();

        var monsterData = monsterDataLoad.Value as Dictionary<string, object>;
        foreach (var monsterKey in monsterData.Keys)
        {
            var monsters = monsterData[monsterKey] as Dictionary<string, object>;
            if (monsters != null)
            {
                //Debug.Log("Monster Key: " + monsterKey);

                monster.Add(monsterKey, monsters);

                foreach (var attributeKey in monsters.Keys)
                {
                    var attributeValue = monsters[attributeKey];
                   // Debug.Log(attributeKey + ": " + attributeValue);
                }
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
                Debug.Log("itemKey Key: " + itemKey);

                item.Add(itemKey, items);

                foreach (var attributeKey in items.Keys)
                {
                    var attributeValue = items[attributeKey];
                    Debug.Log(attributeKey + ": " + attributeValue);
                }
            }
        }
    }

    public static async Task EquipLoadData()

    {
        equipDataLoad = await databaseReference.Child("Equip").GetValueAsync();

        var equipData = equipDataLoad.Value as Dictionary<string, object>;
        foreach (var equipKey in equipData.Keys)
        {
            var equips = equipData[equipKey] as Dictionary<string, object>;
            if (equips != null)
            {
                Debug.Log("Monster Key: " + equipKey);

                equip.Add(equipKey, equips);

                foreach (var attributeKey in equips.Keys)
                {
                    var attributeValue = equips[attributeKey];
                    Debug.Log(attributeKey + ": " + attributeValue);
                }
            }
        }
    }
}