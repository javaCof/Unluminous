using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;

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
    public void SaveItemData()
    {  
        var Item = new Item(id, price);
        string jsonData = JsonUtility.ToJson(Item);

        databaseReference.Child("Item").Child(itemName).SetRawJsonValueAsync(jsonData);
    }
}
