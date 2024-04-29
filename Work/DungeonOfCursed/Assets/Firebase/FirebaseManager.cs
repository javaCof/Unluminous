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
    public static DataSnapshot helmetDataLoad;
    public static DataSnapshot armorDataLoad;
    public static DataSnapshot pantsDataLoad;
    public static DataSnapshot setDataLoad;
    public static Dictionary<string, Dictionary<string, object>> monster = new Dictionary<string, Dictionary<string, object>>();
    public static Dictionary<string, Dictionary<string, object>> item = new Dictionary<string, Dictionary<string, object>>();
    public static Dictionary<string, Dictionary<string, object>> helmet = new Dictionary<string, Dictionary<string, object>>();
    public static Dictionary<string, Dictionary<string, object>> armor = new Dictionary<string, Dictionary<string, object>>();
    public static Dictionary<string, Dictionary<string, object>> pants = new Dictionary<string, Dictionary<string, object>>();
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
                //Debug.Log("itemKey Key: " + itemKey);

                item.Add(itemKey, items);

                foreach (var attributeKey in items.Keys)
                {
                    var attributeValue = items[attributeKey];
                    //Debug.Log(attributeKey + ": " + attributeValue);
                }
            }
        }
    }

    public static async Task EquipLoadData()

    {
        helmetDataLoad = await databaseReference.Child("Equip").Child("Helmet").GetValueAsync();
        armorDataLoad = await databaseReference.Child("Equip").Child("Armor").GetValueAsync();
        pantsDataLoad = await databaseReference.Child("Equip").Child("Pants").GetValueAsync();
        setDataLoad = await databaseReference.Child("Equip").Child("SetEquip").GetValueAsync();

        var helmetData = helmetDataLoad.Value as Dictionary<string, object>;
        var armorData = armorDataLoad.Value as Dictionary<string, object>;
        var pantsData = pantsDataLoad.Value as Dictionary<string, object>;
        var setData = setDataLoad.Value as Dictionary<string, object>;

        foreach (var helmetKey in helmetData.Keys)
        {
            var helmets = helmetData[helmetKey] as Dictionary<string, object>;
            if (helmets != null)
            {
                helmet.Add(helmetKey, helmets);
            }
        }

        foreach (var armorKey in armorData.Keys)
        {
            var armors = armorData[armorKey] as Dictionary<string, object>;
            if (armors != null)
            {
                armor.Add(armorKey, armors);
            }
        }

        foreach (var pantsKey in pantsData.Keys)
        {
            var pantses = pantsData[pantsKey] as Dictionary<string, object>;
            if (pantses != null)
            {
                pants.Add(pantsKey, pantses);
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