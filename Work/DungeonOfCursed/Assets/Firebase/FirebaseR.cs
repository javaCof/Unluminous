using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using System.Threading.Tasks;
public class FirebaseR
{
    public static DatabaseReference databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

    public static async Task<DataSnapshot> MonsterData()
    {
        var monsterData = await databaseReference.Child("Monster").GetValueAsync();

        return monsterData;
    }

    public static async Task<DataSnapshot> ItemData()
    {
        var itemData = await databaseReference.Child("Item").GetValueAsync();

        return itemData;
    }

    public static async Task<DataSnapshot> EquipData()
    {
        var equipData = await databaseReference.Child("Equip").GetValueAsync();

        return equipData;
    }
}
