using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public int Capacity { get; private set; }

    [SerializeField, Range(8, 64)]
    private int _initalCapacity = 20;

    [SerializeField, Range(8, 64)]
    private int _maxCapacity = 64;

    [SerializeField]
    private InventoryUI _inventoryUI; // 연결된 인벤토리 UI

    [SerializeField]
    private Item2[] _items;

    [SerializeField] private Button _openInvenButton;
    [SerializeField] private Button _closeInvenButton;
    [SerializeField] private GameObject _inventory;

    private bool isOpen = false;

    private readonly HashSet<int> _indexSetForUpdate = new HashSet<int>();

    private List<TemporaryItem> temporaryItemList = new List<TemporaryItem>();

    private readonly static Dictionary<Type, int> _sortWeightDict = new Dictionary<Type, int>
        {
            { typeof(PortionItemData), 10000 },
            { typeof(WeaponItemData),  20000 },
            { typeof(ArmorItemData),   30000 },
        };

    private class ItemComparer : IComparer<Item2>
    {
        public int Compare(Item2 a, Item2 b)
        {
            return (a.Data.ID + _sortWeightDict[a.Data.GetType()])
                 - (b.Data.ID + _sortWeightDict[b.Data.GetType()]);
        }
    }

    private class TemporaryItem
    {
        public ItemData itemData;
        public int amount;

        public TemporaryItem(ItemData itemData, int amount)
        {
            this.itemData = itemData;
            this.amount = amount;
        }
    }

    private static readonly ItemComparer _itemComparer = new ItemComparer();


#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_initalCapacity > _maxCapacity)
            _initalCapacity = _maxCapacity;
    }
#endif
    private void Awake()
    {
        _items = new Item2[_maxCapacity];
        Capacity = _initalCapacity;
        _inventoryUI.SetInventoryReference(this);

    }

    private void Start()
    {
        UpdateAccessibleStatesAll();
        ShowPanel();
        HidePanel();
    }

    private bool IsValidIndex(int index)
    {
        return index >= 0 && index < Capacity;
    }

    private int FindEmptySlotIndex(int startIndex = 0)
    {
        for (int i = startIndex; i < Capacity; i++)
            if (_items[i] == null)
                return i;
        return -1;
    }

    private int FindCountableItemSlotIndex(CountableItemData target, int startIndex = 0)
    {
        for (int i = startIndex; i < Capacity; i++)
        {
            var current = _items[i];
            if (current == null)
                continue;

            if (current.Data == target && current is CountableItem ci)
            {
                if (!ci.IsMax)
                    return i;
            }
        }

        return -1;
    }

    private void UpdateSlot(int index)
    {
        if (!IsValidIndex(index))
        {
            return;
        }

        Item2 item = _items[index];

        if (item != null)
        {
            Sprite sprite = Resources.Load<Sprite>(item.Data.Icon);
            
            if(sprite == null)
            {
                Debug.Log("스프라이트 없음");
                return;
            }
     
            _inventoryUI.SetItemIcon(index, sprite);

            if (item is CountableItem ci)
            {

                if (ci.IsEmpty)
                {
                    _items[index] = null;
                    RemoveIcon();
                    return;
                }
                else
                {
                    _inventoryUI.SetItemAmountText(index, ci.Amount);
                }
            }
            else
            {
                _inventoryUI.HideItemAmountText(index);
            }

            _inventoryUI.UpdateSlotFilterState(index, item.Data);
        }
        else
        {
            RemoveIcon();
        }

        void RemoveIcon()
        {
            _inventoryUI.RemoveItem(index);
            _inventoryUI.HideItemAmountText(index);
        }
    }

    private void UpdateSlot(params int[] indices)
    {
        foreach (var i in indices)
        {
            UpdateSlot(i);
        }
    }

    private void UpdateAllSlot()
    {
        for (int i = 0; i < Capacity; i++)
        {
            UpdateSlot(i);
        }
    }

    public void ConnectUI(InventoryUI inventoryUI)
    {
        _inventoryUI = inventoryUI;
        _inventoryUI.SetInventoryReference(this);
    }

    public int Add(ItemData itemData, int amount = 1)
    {
        int index = -1;

        if (itemData is CountableItemData ciData)
        {
            bool findNextCountable = true;

            ciData.MaxAmount = 99;

            while (amount > 0)
            {
                if (findNextCountable)
                {
                    index = FindCountableItemSlotIndex(ciData, index + 1);

                    if (index == -1)
                    {
                        findNextCountable = false;
                    }
                    else
                    {
                        CountableItem ci = _items[index] as CountableItem;
                        amount = ci.AddAmountAndGetExcess(amount);
                        UpdateSlot(index);
                    }
                }
                else
                {

                    index = FindEmptySlotIndex(index + 1);

                    if (index == -1)
                    {
                        break;
                    }
                    else
                    {
                        CountableItem ci = ciData.CreateItem() as CountableItem;
                        ci.SetAmount(amount);
                        
                        _items[index] = ci;
                        amount = (amount > ciData.MaxAmount) ? (amount - ciData.MaxAmount) : 0;
                        UpdateSlot(index);
                    }
                }
            }
        }
        else if (itemData is ArmorItemData)
        {
            // ArmorItemData는 중복되지 않도록 한 번만 추가
            index = FindEmptySlotIndex();
            if (index != -1)
            {
                _items[index] = itemData.CreateItem();
                amount = 0; // 아이템 하나만 추가되도록 설정
                Debug.Log($"Added ArmorItemData: {itemData.Name}");
                UpdateSlot(index);
            }
        }
        else
        {
            // 기본 ItemData 처리
            for (; amount > 0; amount--)
            {
                index = FindEmptySlotIndex(index + 1);
                Debug.Log($"FindEmptySlotIndex returned: {index}");

                if (index == -1)
                {
                    break;
                }
                _items[index] = itemData.CreateItem();
                Debug.Log($"Added ItemData: {itemData.Name}");
                UpdateSlot(index);
            }
        }

        return amount;
    }

    public void Remove(int index)
    {
        if (!IsValidIndex(index)) return;

        _items[index] = null;
        _inventoryUI.RemoveItem(index);
    }

    public void Swap(int indexA, int indexB)
    {
        if (!IsValidIndex(indexA)) return;
        if (!IsValidIndex(indexB)) return;

        Item2 itemA = _items[indexA];
        Item2 itemB = _items[indexB];

        if (itemA != null && itemB != null &&
            itemA.Data == itemB.Data &&
            itemA is CountableItem ciA && itemB is CountableItem ciB)
        {
            int maxAmount = ciB.MaxAmount;
            int sum = ciA.Amount + ciB.Amount;

            if (sum <= maxAmount)
            {
                ciA.SetAmount(0);
                ciB.SetAmount(sum);
            }
            else
            {
                ciA.SetAmount(sum - maxAmount);
                ciB.SetAmount(maxAmount);
            }
        }
        else
        {
            _items[indexA] = itemB;
            _items[indexB] = itemA;
        }

        UpdateSlot(indexA, indexB);
    }

    public void SeparateAmount(int indexA, int indexB, int amount)
    {
        // amount : 나눌 목표 수량

        if (!IsValidIndex(indexA)) return;
        if (!IsValidIndex(indexB)) return;

        Item2 _itemA = _items[indexA];
        Item2 _itemB = _items[indexB];

        CountableItem _ciA = _itemA as CountableItem;

        if (_ciA != null && _itemB == null)
        {
            _items[indexB] = _ciA.SeperateAndClone(amount);

            UpdateSlot(indexA, indexB);
        }
    }
    public void Use(int index)
    {
        if (!IsValidIndex(index)) return;
        if (_items[index] == null) return;

        if (_items[index] is IUsableItem uItem)
        {
            // 아이템 사용
            bool succeeded = uItem.Use();

            if (succeeded)
            {
                UpdateSlot(index);
            }
        }
    }

    public void UpdateAccessibleStatesAll()
    {
        _inventoryUI.SetAccessibleSlotRange(Capacity);
    }

    public bool HasItem(int index)
    {
        return IsValidIndex(index) && _items[index] != null;
    }

    public bool IsCountableItem(int index)
    {
        return HasItem(index) && _items[index] is CountableItem;
    }

    public int GetCurrentAmount(int index)
    {
        if (!IsValidIndex(index)) return -1;
        if (_items[index] == null) return 0;

        CountableItem ci = _items[index] as CountableItem;
        if (ci == null)
            return 1;

        return ci.Amount;
    }

    public ItemData GetItemData(int index)
    {
        if (!IsValidIndex(index)) return null;
        if (_items[index] == null) return null;

        return _items[index].Data;
    }

    public string GetItemName(int index)
    {
        if (!IsValidIndex(index)) return "";
        if (_items[index] == null) return "";

        return _items[index].Data.Name;
    }


    public void TrimAll()
    {

        _indexSetForUpdate.Clear();

        int i = -1;
        while (_items[++i] != null) ;
        int j = i;

        while (true)
        {
            while (++j < Capacity && _items[j] == null) ;

            if (j == Capacity)
                break;

            _indexSetForUpdate.Add(i);
            _indexSetForUpdate.Add(j);

            _items[i] = _items[j];
            _items[j] = null;
            i++;
        }

        foreach (var index in _indexSetForUpdate)
        {
            UpdateSlot(index);
        }
        _inventoryUI.UpdateAllSlotFilters();
    }
    public void SortAll()
    {
        int i = -1;
        while (_items[++i] != null) ;
        int j = i;

        while (true)
        {
            while (++j < Capacity && _items[j] == null) ;

            if (j == Capacity)
                break;

            _items[i] = _items[j];
            _items[j] = null;
            i++;
        }

        Array.Sort(_items, 0, i, _itemComparer);

        UpdateAllSlot();
        _inventoryUI.UpdateAllSlotFilters();
    }

    private void HidePanel() => _inventory.SetActive(false);
    private void ShowPanel() => _inventory.SetActive(true);
}