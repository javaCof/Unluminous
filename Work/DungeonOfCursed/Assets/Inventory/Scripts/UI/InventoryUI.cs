using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.XR;
using UnityEngine.XR.Interaction;
using UnityEngine.XR.Interaction.Toolkit;


public class InventoryUI : MonoBehaviour
{
    [Header("Options")]
    [Range(0, 10)]
    [SerializeField] private int _horizontalSlotCount = 8;  // 슬롯 가로 개수
    [Range(0, 10)]
    [SerializeField] private int _verticalSlotCount = 8;      // 슬롯 세로 개수
    [SerializeField] private float _slotMargin = 8f;          // 한 슬롯의 상하좌우 여백
    [SerializeField] private float _contentAreaPadding = 20f; // 인벤토리 영역의 내부 여백
    [Range(32, 170)]
    [SerializeField] private float _slotSize = 120f;      // 각 슬롯의 크기

    [Space]
    [SerializeField] private bool _showTooltip = true;
    [SerializeField] private bool _showHighlight = true;
    [SerializeField] private bool _showRemovingPopup = true;

    [Header("Connected Objects")]
    [SerializeField] private RectTransform _contentAreaRT; // 슬롯들이 위치할 영역
    [SerializeField] private GameObject _slotUiPrefab;     // 슬롯의 원본 프리팹
    [SerializeField] private ItemTooltipUI _itemTooltip;   // 아이템 정보를 보여줄 툴팁 UI
    [SerializeField] private InventoryPopupUI _popup;      // 팝업 UI 관리 객체

    [Header("Buttons")]
    [SerializeField] private Button _trimButton;
    [SerializeField] private Button _sortButton;

    [Header("Filter Toggles")]
    [SerializeField] private Toggle _toggleFilterAll;
    [SerializeField] private Toggle _toggleFilterEquipments;
    [SerializeField] private Toggle _toggleFilterPortions;

    [Space(16)]
    [SerializeField] private bool _mouseReversed = false; // 마우스 클릭 반전 여부

    private Inventory _inventory;

    private List<ItemSlotUI> _slotUIList = new List<ItemSlotUI>();
    private GraphicRaycaster _gr;
    private PointerEventData _ped;
    private List<RaycastResult> _rrList;

    private ItemSlotUI _pointerOverSlot; // 현재 포인터가 위치한 곳의 슬롯
    private ItemSlotUI _beginDragSlot; // 현재 드래그를 시작한 슬롯
    private Transform _beginDragIconTransform; // 해당 슬롯의 아이콘 트랜스폼

    private int _leftClick = 0;
    private int _rightClick = 1;
    private bool isOpen = false;

    private Vector3 _beginDragIconPoint;   // 드래그 시작 시 슬롯의 위치
    private Vector3 _beginDragCursorPoint; // 드래그 시작 시 커서의 위치
    private int _beginDragSlotSiblingIndex;

    public XRRayInteractor interactor;
    RaycastResult hit;

    private enum FilterOption
    {
        All, Equipment, Portion
    }
    private FilterOption _currentFilterOption = FilterOption.All;

    private void Awake()
    {
        Init();
        InitSlots();
        InitButtonEvents();
        InitToggleEvents();
        interactor = GameObject.FindObjectOfType<XRRayInteractor>();
    }

    private void Update()
    {
        //vr모드일때
        if (GameManager.Instance.VrEnable)
        {
            // 히트된 지점으로 _ped의 위치를 설정
            interactor.TryGetCurrentUIRaycastResult(out hit);
            _ped.position = hit.screenPosition;
            

        }
        else
        {
            _ped.position = Input.mousePosition;
        }

        OnPointerEnterAndExit();
        if (_showTooltip) ShowOrHideItemTooltip();
        OnPointerDown();
        OnPointerDrag();
        OnPointerUp();
    }

    private void Init()
    {
        TryGetComponent(out _gr);
        if (_gr == null)
            _gr = gameObject.AddComponent<GraphicRaycaster>();

        _ped = new PointerEventData(EventSystem.current);
        _rrList = new List<RaycastResult>(10);

        if (_itemTooltip == null)
        {
            _itemTooltip = GetComponentInChildren<ItemTooltipUI>();
            EditorLog("인스펙터에서 아이템 툴팁 UI를 직접 지정하지 않아 자식에서 발견하여 초기화하였습니다.");
        }
    }

    private void InitSlots()
    {
        _slotUiPrefab.TryGetComponent(out RectTransform slotRect);
        slotRect.sizeDelta = new Vector2(_slotSize, _slotSize);

        _slotUiPrefab.TryGetComponent(out ItemSlotUI itemSlot);
        if (itemSlot == null)
            _slotUiPrefab.AddComponent<ItemSlotUI>();

        _slotUiPrefab.SetActive(false);

        Vector2 beginPos = new Vector2(_contentAreaPadding, -_contentAreaPadding);
        Vector2 curPos = beginPos;

        _slotUIList = new List<ItemSlotUI>(_verticalSlotCount * _horizontalSlotCount);

        for (int j = 0; j < _verticalSlotCount; j++)
        {
            for (int i = 0; i < _horizontalSlotCount; i++)
            {
                int slotIndex = (_horizontalSlotCount * j) + i;

                var slotRT = CloneSlot();
                slotRT.pivot = new Vector2(0f, 1f);
                slotRT.anchoredPosition = curPos;
                slotRT.gameObject.SetActive(true);
                slotRT.gameObject.name = $"Item Slot [{slotIndex}]";

                var slotUI = slotRT.GetComponent<ItemSlotUI>();
                slotUI.SetSlotIndex(slotIndex);
                _slotUIList.Add(slotUI);

                curPos.x += (_slotMargin + _slotSize);
            }

            curPos.x = beginPos.x;
            curPos.y -= (_slotMargin + _slotSize);
        }

        if (_slotUiPrefab.scene.rootCount != 0)
            Destroy(_slotUiPrefab);

        RectTransform CloneSlot()
        {
            GameObject slotGo = Instantiate(_slotUiPrefab);
            RectTransform rt = slotGo.GetComponent<RectTransform>();
            rt.SetParent(_contentAreaRT);

            return rt;
        }
    }

    private void InitButtonEvents()
    {
        _trimButton.onClick.AddListener(() => _inventory.TrimAll());
        _sortButton.onClick.AddListener(() => _inventory.SortAll());
    }

    private void InitToggleEvents()
    {
        _toggleFilterAll.onValueChanged.AddListener(flag => UpdateFilter(flag, FilterOption.All));
        _toggleFilterEquipments.onValueChanged.AddListener(flag => UpdateFilter(flag, FilterOption.Equipment));
        _toggleFilterPortions.onValueChanged.AddListener(flag => UpdateFilter(flag, FilterOption.Portion));

        void UpdateFilter(bool flag, FilterOption option)
        {
            if (flag)
            {
                _currentFilterOption = option;
                UpdateAllSlotFilters();
            }
        }
    }
    private bool IsOverUI()
        => EventSystem.current.IsPointerOverGameObject();

    private T RaycastAndGetFirstComponent<T>() where T : Component
    {
        _rrList.Clear();

        _gr.Raycast(_ped, _rrList);

        if (_rrList.Count == 0)
            return null;

        return _rrList[0].gameObject.GetComponent<T>();
    }
    private void OnPointerEnterAndExit()
    {
        var prevSlot = _pointerOverSlot;

        var curSlot = _pointerOverSlot = RaycastAndGetFirstComponent<ItemSlotUI>();

        if (prevSlot == null)
        {
            if (curSlot != null)
            {
                OnCurrentEnter();
            }
        }
        else
        {
            if (curSlot == null)
            {
                OnPrevExit();
            }
            else if (prevSlot != curSlot)
            {
                OnPrevExit();
                OnCurrentEnter();
            }
        }

        void OnCurrentEnter()
        {
            if (_showHighlight)
                curSlot.Highlight(true);
        }

        void OnPrevExit()
        {
            prevSlot.Highlight(false);
        }
    }

    private void ShowOrHideItemTooltip()
    {
        bool isValid =
            _pointerOverSlot != null && _pointerOverSlot.HasItem && _pointerOverSlot.IsAccessible
            && (_pointerOverSlot != _beginDragSlot);

        if (isValid)
        {
            UpdateTooltipUI(_pointerOverSlot);
            _itemTooltip.Show();
        }
        //else
        //   // _itemTooltip.Hide();
    }

    private void OnPointerDown()
    {

        if (Input.GetMouseButtonDown(_leftClick))
        {
            _beginDragSlot = RaycastAndGetFirstComponent<ItemSlotUI>();


            if (_beginDragSlot != null && _beginDragSlot.HasItem && _beginDragSlot.IsAccessible)
            {
                EditorLog($"Drag Begin : Slot [{_beginDragSlot.Index}]");

                _beginDragIconTransform = _beginDragSlot.IconRect.transform;
                _beginDragIconPoint = _beginDragIconTransform.position;
                _beginDragCursorPoint = Input.mousePosition;

                _beginDragSlotSiblingIndex = _beginDragSlot.transform.GetSiblingIndex();
                _beginDragSlot.transform.SetAsLastSibling();

                _beginDragSlot.SetHighlightOnTop(false);
            }
            else
            {
                _beginDragSlot = null;
            }
        }

        else if (Input.GetMouseButtonDown(_rightClick))
        {
            ItemSlotUI slot = RaycastAndGetFirstComponent<ItemSlotUI>();

            if (slot != null && slot.HasItem && slot.IsAccessible)
            {
                TryUseItem(slot.Index);
            }
        }
    }

    private void OnPointerDrag()
    {
        if (_beginDragSlot == null) return;

        if (Input.GetMouseButton(_leftClick))
        {

            _beginDragIconTransform.position =
                _beginDragIconPoint + (Input.mousePosition - _beginDragCursorPoint);
        }
    }

    private void OnPointerUp()
    {
        if (Input.GetMouseButtonUp(_leftClick))
        {

            if (_beginDragSlot != null)
            {
                _beginDragIconTransform.position = _beginDragIconPoint;

                _beginDragSlot.transform.SetSiblingIndex(_beginDragSlotSiblingIndex);

                EndDrag();

                _beginDragSlot.SetHighlightOnTop(true);

                _beginDragSlot = null;
                _beginDragIconTransform = null;
            }
        }
    }

    private void EndDrag()
    {
        ItemSlotUI endDragSlot = RaycastAndGetFirstComponent<ItemSlotUI>();

        if (endDragSlot != null && endDragSlot.IsAccessible)
        {

            bool isSeparatable =
                (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift)) &&
                (_inventory.IsCountableItem(_beginDragSlot.Index) && !_inventory.HasItem(endDragSlot.Index));

            bool isSeparation = false;
            int currentAmount = 0;

            if (isSeparatable)
            {
                currentAmount = _inventory.GetCurrentAmount(_beginDragSlot.Index);
                if (currentAmount > 1)
                {
                    isSeparation = true;
                }
            }

            if (isSeparation)
                TrySeparateAmount(_beginDragSlot.Index, endDragSlot.Index, currentAmount);

            else
                TrySwapItems(_beginDragSlot, endDragSlot);

            UpdateTooltipUI(endDragSlot);
            return;
        }

        if (!IsOverUI())
        {
            int index = _beginDragSlot.Index;
            string itemName = _inventory.GetItemName(index);
            int amount = _inventory.GetCurrentAmount(index);

            if (amount > 1)
                itemName += $" x{amount}";

            if (_showRemovingPopup)
                _popup.OpenConfirmationPopup(() => TryRemoveItem(index), itemName);
            else
                TryRemoveItem(index);
        }
        else
        {
            EditorLog($"Drag End(Do Nothing)");
        }
    }

    private void TryRemoveItem(int index)
    {
        EditorLog($"UI - Try Remove Item : Slot [{index}]");

        _inventory.Remove(index);
    }

    private void TryUseItem(int index)
    {
        EditorLog($"UI - Try Use Item : Slot [{index}]");

        _inventory.Use(index);
    }

    private void TrySwapItems(ItemSlotUI from, ItemSlotUI to)
    {
        if (from == to)
        {
            EditorLog($"UI - Try Swap Items: Same Slot [{from.Index}]");
            return;
        }

        EditorLog($"UI - Try Swap Items: Slot [{from.Index} -> {to.Index}]");

        from.SwapOrMoveIcon(to);
        _inventory.Swap(from.Index, to.Index);
    }

    private void TrySeparateAmount(int indexA, int indexB, int amount)
    {
        if (indexA == indexB)
        {
            EditorLog($"UI - Try Separate Amount: Same Slot [{indexA}]");
            return;
        }

        EditorLog($"UI - Try Separate Amount: Slot [{indexA} -> {indexB}]");

        string itemName = $"{_inventory.GetItemName(indexA)} x{amount}";

        _popup.OpenAmountInputPopup(
            amt => _inventory.SeparateAmount(indexA, indexB, amt),
            amount, itemName
        );
    }
    private void UpdateTooltipUI(ItemSlotUI slot)
    {
        if (!slot.IsAccessible || !slot.HasItem)
            return;

        _itemTooltip.SetItemInfo(_inventory.GetItemData(slot.Index));
        //_itemTooltip.SetRectPosition(slot.SlotRect);
    }

    public void SetInventoryReference(Inventory inventory)
    {
        _inventory = inventory;
    }

    public void InvertMouse(bool value)
    {
        _leftClick = value ? 1 : 0;
        _rightClick = value ? 0 : 1;

        _mouseReversed = value;
    }

    public void SetItemIcon(int index, Sprite icon)
    {
        EditorLog($"Set Item Icon : Slot [{index}]");

        _slotUIList[index].SetItem(icon);
    }

    public void SetItemAmountText(int index, int amount)
    {
        EditorLog($"Set Item Amount Text : Slot [{index}], Amount [{amount}]");

        _slotUIList[index].SetItemAmount(amount);
    }

    public void HideItemAmountText(int index)
    {
        EditorLog($"Hide Item Amount Text : Slot [{index}]");

        _slotUIList[index].SetItemAmount(1);
    }

    public void RemoveItem(int index)
    {
        EditorLog($"Remove Item : Slot [{index}]");

        _slotUIList[index].RemoveItem();
    }

    public void SetAccessibleSlotRange(int accessibleSlotCount)
    {
        for (int i = 0; i < _slotUIList.Count; i++)
        {
            _slotUIList[i].SetSlotAccessibleState(i < accessibleSlotCount);
        }
    }

    public void UpdateSlotFilterState(int index, ItemData itemData)
    {
        bool isFiltered = true;

        if (itemData != null)
            switch (_currentFilterOption)
            {
                case FilterOption.Equipment:
                    isFiltered = (itemData is EquipmentItemData);
                    break;

                case FilterOption.Portion:
                    isFiltered = (itemData is PortionItemData);
                    break;
            }

        _slotUIList[index].SetItemAccessibleState(isFiltered);
    }

    public void UpdateAllSlotFilters()
    {
        int capacity = _inventory.Capacity;

        for (int i = 0; i < capacity; i++)
        {
            ItemData data = _inventory.GetItemData(i);
            UpdateSlotFilterState(i, data);
        }
    }

#if UNITY_EDITOR
    [Header("Editor Options")]
    [SerializeField] private bool _showDebug = true;
#endif
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void EditorLog(object message)
    {
        //if (!_showDebug) return;                                      //>>>ERROR
        UnityEngine.Debug.Log($"[InventoryUI] {message}");
    }

#if UNITY_EDITOR
    [SerializeField] private bool __showPreview = false;

    [Range(0.01f, 1f)]
    [SerializeField] private float __previewAlpha = 0.1f;

    private List<GameObject> __previewSlotGoList = new List<GameObject>();
    private int __prevSlotCountPerLine;
    private int __prevSlotLineCount;
    private float __prevSlotSize;
    private float __prevSlotMargin;
    private float __prevContentPadding;
    private float __prevAlpha;
    private bool __prevShow = false;
    private bool __prevMouseReversed = false;

    /*
    private void OnValidate()
    {
        if (__prevMouseReversed != _mouseReversed)
        {
            __prevMouseReversed = _mouseReversed;
            InvertMouse(_mouseReversed);

            EditorLog($"Mouse Reversed : {_mouseReversed}");
        }

        if (Application.isPlaying) return;

        if (__showPreview && !__prevShow)
        {
            CreateSlots();
        }
        __prevShow = __showPreview;

        if (Unavailable())
        {
            ClearAll();
            return;
        }
        if (CountChanged())
        {
            ClearAll();
            CreateSlots();
            __prevSlotCountPerLine = _horizontalSlotCount;
            __prevSlotLineCount = _verticalSlotCount;
        }
        if (ValueChanged())
        {
            DrawGrid();
            __prevSlotSize = _slotSize;
            __prevSlotMargin = _slotMargin;
            __prevContentPadding = _contentAreaPadding;
        }
        if (AlphaChanged())
        {
            SetImageAlpha();
            __prevAlpha = __previewAlpha;
        }

        bool Unavailable()
        {
            return !__showPreview ||
                    _horizontalSlotCount < 1 ||
                    _verticalSlotCount < 1 ||
                    _slotSize <= 0f ||
                    _contentAreaRT == null ||
                    _slotUiPrefab == null;
        }
        bool CountChanged()
        {
            return _horizontalSlotCount != __prevSlotCountPerLine ||
                   _verticalSlotCount != __prevSlotLineCount;
        }
        bool ValueChanged()
        {
            return _slotSize != __prevSlotSize ||
                   _slotMargin != __prevSlotMargin ||
                   _contentAreaPadding != __prevContentPadding;
        }
        bool AlphaChanged()
        {
            return __previewAlpha != __prevAlpha;
        }
        void ClearAll()
        {
            foreach (var go in __previewSlotGoList)
            {
                Destroyer.Destroy(go);
            }
            __previewSlotGoList.Clear();
        }
        void CreateSlots()
        {
            int count = _horizontalSlotCount * _verticalSlotCount;
            __previewSlotGoList.Capacity = count;

            RectTransform slotPrefabRT = _slotUiPrefab.GetComponent<RectTransform>();
            slotPrefabRT.pivot = new Vector2(0f, 1f);

            for (int i = 0; i < count; i++)
            {
                GameObject slotGo = Instantiate(_slotUiPrefab);
                slotGo.transform.SetParent(_contentAreaRT.transform);
                slotGo.SetActive(true);
                slotGo.AddComponent<PreviewItemSlot>();

                slotGo.transform.localScale = Vector3.one;

                HideGameObject(slotGo);

                __previewSlotGoList.Add(slotGo);
            }

            DrawGrid();
            SetImageAlpha();
        }
        void DrawGrid()
        {
            Vector2 beginPos = new Vector2(_contentAreaPadding, -_contentAreaPadding);
            Vector2 curPos = beginPos;

            // Draw Slots
            int index = 0;
            for (int j = 0; j < _verticalSlotCount; j++)
            {
                for (int i = 0; i < _horizontalSlotCount; i++)
                {
                    GameObject slotGo = __previewSlotGoList[index++];
                    RectTransform slotRT = slotGo.GetComponent<RectTransform>();

                    slotRT.anchoredPosition = curPos;
                    slotRT.sizeDelta = new Vector2(_slotSize, _slotSize);
                    __previewSlotGoList.Add(slotGo);

                    curPos.x += (_slotMargin + _slotSize);
                }

                curPos.x = beginPos.x;
                curPos.y -= (_slotMargin + _slotSize);
            }
        }
        void HideGameObject(GameObject go)
        {
            go.hideFlags = HideFlags.HideAndDontSave;

            Transform tr = go.transform;
            for (int i = 0; i < tr.childCount; i++)
            {
                tr.GetChild(i).gameObject.hideFlags = HideFlags.HideAndDontSave;
            }
        }
        void SetImageAlpha()
        {
            foreach (var go in __previewSlotGoList)
            {
                var images = go.GetComponentsInChildren<Image>();
                foreach (var img in images)
                {
                    img.color = new Color(img.color.r, img.color.g, img.color.b, __previewAlpha);
                    var outline = img.GetComponent<Outline>();
                    if (outline)
                        outline.effectColor = new Color(outline.effectColor.r, outline.effectColor.g, outline.effectColor.b, __previewAlpha);
                }
            }
        }
    }
    */
    private class PreviewItemSlot : MonoBehaviour { }

    [UnityEditor.InitializeOnLoad]
    private static class Destroyer
    {
        private static Queue<GameObject> targetQueue = new Queue<GameObject>();

        static Destroyer()
        {
            UnityEditor.EditorApplication.update += () =>
            {
                for (int i = 0; targetQueue.Count > 0 && i < 100000; i++)
                {
                    var next = targetQueue.Dequeue();
                    DestroyImmediate(next);
                }
            };
        }
        public static void Destroy(GameObject go) => targetQueue.Enqueue(go);
    }
#endif
}