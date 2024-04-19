using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using System.Globalization;

public class ItemDataManager : MonoBehaviour
{
    public class Item
    {
        public int id;
        public int price;

        public Item(int id, int price)
        {
            this.id = id;
            this.price = price;
        }
    }

    public string itemName;
    public int id;
    public int price;

    private DatabaseReference databaseReference;

    void Awake()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    [ContextMenu("아이템 정보 저장")]
    void SaveItemData()
    {  
        var Item = new Item(id, price);
        string jsonData = JsonUtility.ToJson(Item);

        databaseReference.Child("Item").Child(itemName).SetRawJsonValueAsync(jsonData);
    }

    [ContextMenu("아이템 정보 불러오기")]
    async void LoadItemData()
    {
        var dataSnapshot = await databaseReference.Child("Item").GetValueAsync();

        if(dataSnapshot.HasChildren)
        {
            var count = dataSnapshot.ChildrenCount;

            string[] namedataString = new string[count];

            int index = 0;

            foreach(var name in dataSnapshot.Children)
            {
                namedataString[index] = name.Key; 
                Debug.Log(namedataString[index]);
                index++;
            }
        }  
    }

    [ContextMenu("아이템 정보 불러오기2")]
    async void LoadItemData2()
    {
        var dataSnapshot = await databaseReference.Child("Item").GetValueAsync();
    
        if(dataSnapshot.HasChildren)
        {
            foreach(var itemDataSnapshot in dataSnapshot.Children)
            {
                var itemName = itemDataSnapshot.Key;
                var itemValues = itemDataSnapshot.Value as Dictionary<string, object>;
                
                if(itemValues != null)
                {
                    var dataString = "";
                    
                    foreach(var item in itemValues)
                    {
                        dataString += item.Key + " " + item.Value.ToString() + "\n";
                    }
                    
                    Debug.Log(itemName + " " + dataString);
                }
            }
        }
        else
        {
            Debug.Log("No items found.");
        }
    }
}
