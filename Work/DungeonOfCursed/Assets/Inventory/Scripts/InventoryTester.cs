using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InventoryTester : MonoBehaviour
{
    public Inventory _inventory;

    public Item[] _itemDataArray;

    [Space(12)]
    public Button _removeAllButton;

    [Space(8)]
    public Button _AddArmorA1;
    public Button _AddArmorB1;
    public Button _AddSwordA1;
    public Button _AddSwordB1;
    public Button _AddPortionA1;
    public Button _AddPortionA50;
    public Button _AddPortionB1;
    public Button _AddPortionB50;

    private void Start()
    {
        //if (_itemDataArray?.Length > 0)
        //{
        //    for (int i = 0; i < _itemDataArray.Length; i++)
        //    {
        //        _inventory.Add(_itemDataArray[i], 3);

        //        if (_itemDataArray[i] is CountableItemData)
        //            _inventory.Add(_itemDataArray[i], 255);
        //    }
        //}

        //_removeAllButton.onClick.AddListener(() =>
        //{
        //    int capacity = _inventory.Capacity;
        //    for (int i = 0; i < capacity; i++)
        //        _inventory.Remove(i);
        //});

        //_AddArmorA1.onClick.AddListener(() => _inventory.Add(_itemDataArray[1]));

        //_inventory.Add(new CountableItemData(1002), 10);


        //_AddArmorB1.onClick.AddListener(() => _inventory.Add(_itemDataArray[1]));

        //_AddSwordA1.onClick.AddListener(() => _inventory.Add(_itemDataArray[2]));
        //_AddSwordB1.onClick.AddListener(() => _inventory.Add(_itemDataArray[3]));

        //_AddPortionA1.onClick.AddListener(() => _inventory.Add(_itemDataArray[4]));
        //_AddPortionA50.onClick.AddListener(() => _inventory.Add(_itemDataArray[4], 50));
        //_AddPortionB1.onClick.AddListener(() => _inventory.Add(_itemDataArray[5]));
        //_AddPortionB50.onClick.AddListener(() => _inventory.Add(_itemDataArray[5], 50));
    }

    private void Update()
    {
        //Debug.Log(_itemDataArray[0].Res);
        Item item = null;
        Item item2 = null;
        Item item3 = null;

        item = _itemDataArray[0];
        item2 = _itemDataArray[1];
        item3 = _itemDataArray[2];

        ItemData itemData = item.gameObject.GetComponent<ItemData>();
        ItemData itemData2 = item2.gameObject.GetComponent<ItemData>();
        ItemData itemData3 = item3.gameObject.GetComponent<ItemData>();

        if(itemData != null)
        {
            itemData.ID = item.id;
            itemData.Name = FirebaseManager.items[item.id].name;
            itemData.Price = FirebaseManager.items[item.id].price;
            itemData.Tooltip = FirebaseManager.items[item.id].dec;
            itemData.Res = FirebaseManager.items[item.id].res;
            itemData.Icon = FirebaseManager.items[item.id].icon;
        }

        if (itemData2 != null)
        {
            itemData2.ID = item2.id;
            itemData2.Name = FirebaseManager.items[item2.id].name;
            itemData2.Price = FirebaseManager.items[item2.id].price;
            itemData2.Tooltip = FirebaseManager.items[item2.id].dec;
            itemData2.Res = FirebaseManager.items[item2.id].res;
            itemData2.Icon = FirebaseManager.items[item2.id].icon;
        }

        if (itemData3 != null)
        {
            ArmorItemData elseItemData = itemData3 as ArmorItemData;
            elseItemData.Name = FirebaseManager.equips[item3.id].name;
            elseItemData.Price = FirebaseManager.equips[item3.id].price;
            elseItemData.Tooltip = FirebaseManager.equips[item3.id].dec;
            elseItemData.Res = FirebaseManager.equips[item3.id].res;
            elseItemData.Icon = FirebaseManager.equips[item3.id].icon;
            elseItemData.Atk = FirebaseManager.equips[item3.id].atk;
            elseItemData.Def = FirebaseManager.equips[item3.id].def;
            elseItemData.Speed = FirebaseManager.equips[item3.id].speed;
            elseItemData.Hp = FirebaseManager.equips[item3.id].hp;
            elseItemData.Set = FirebaseManager.equips[item3.id].set;

            itemData3 = elseItemData;
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            _inventory.Add(itemData, 99);
            Debug.Log("추가");
        }

        if (Input.GetKeyDown(KeyCode.F6))
        {
            _inventory.Add(itemData2);
            Debug.Log("추가");
        }

        if (Input.GetKeyDown(KeyCode.F7))
        {
            _inventory.Add(itemData3);
            Debug.Log("추가");
        }
    }

}