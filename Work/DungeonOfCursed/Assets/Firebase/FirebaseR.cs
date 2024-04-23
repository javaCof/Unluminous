using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using System.Threading.Tasks;
public class FirebaseR
{
    public static DatabaseReference databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    public static DataSnapshot monsterDataLoad;
    public static DataSnapshot itemDataLoad;
    public static DataSnapshot equipDataLoad;

    public static async Task LoadData()
    {
        monsterDataLoad = await databaseReference.Child("Monster").GetValueAsync();
        itemDataLoad = await databaseReference.Child("Item").GetValueAsync();
        equipDataLoad = await databaseReference.Child("Equip").GetValueAsync();
    }
}
