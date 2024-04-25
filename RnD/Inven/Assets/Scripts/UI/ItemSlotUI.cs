using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlotUI : MonoBehaviour
{
    [Tooltip("슬롯 내에서 아이콘과 슬롯 사이의 여백")]
    [SerializeField] private float _padding = 1f;

    [Tooltip("아이템 아이콘 이미지")]
    [SerializeField] private Image _iconImage;

    [Tooltip("아이템 개수 텍스트")]
    [SerializeField] private Text _amountText;

    [Tooltip("슬롯이 포커스될 때 나타나는 하이라이트 이미지")]
    [SerializeField] private Image _highlightImage;

    [Space]
    [Tooltip("하이라이트 이미지 알파 값")]
    [SerializeField] private float _highlightAlpha = 0.5f;

    [Tooltip("하이라이트 소요 시간")]
    [SerializeField] private float _highlightFadeDuration = 0.2f;

    public int Index { get; private set; }
    public bool HasItem => _iconImage.sprite != null;
    public bool IsAccessible => _isAccessibleSlot && _isAccessibleItem;

    public RectTransform SlotRect => _slotRect;
    public RectTransform IconRect => _iconRect;

    private InventoryUI _inventoryUI;

    private RectTransform _slotRect;
    private RectTransform _iconRect;
    private RectTransform _highlightRect;

    private GameObject _iconGo;
    private GameObject _textGo;
    private GameObject _highlightGo;

    private Image _slotImage;

    private float _currentHLAlpha = 0f;

    private bool _isAccessibleSlot = true; // 슬롯 접근가능 여부
    private bool _isAccessibleItem = true; // 아이템 접근가능 여부

    private static readonly Color InaccessibleSlotColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
    private static readonly Color InaccessibleIconColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    private void Awake()
    {
        InitComponents();
        InitValues();
    }

    private void InitComponents()
    {
        _inventoryUI = GetComponentInParent<InventoryUI>();

        _slotRect = GetComponent<RectTransform>();
        _iconRect = _iconImage.rectTransform;
        _highlightRect = _highlightImage.rectTransform;

        _iconGo = _iconRect.gameObject;
        _textGo = _amountText.gameObject;
        _highlightGo = _highlightImage.gameObject;

        _slotImage = GetComponent<Image>();
    }
    private void InitValues()
    {
        _iconRect.pivot = new Vector2(0.5f, 0.5f);
        _iconRect.anchorMin = Vector2.zero;
        _iconRect.anchorMax = Vector2.one;

        _iconRect.offsetMin = Vector2.one * (_padding);
        _iconRect.offsetMax = Vector2.one * (-_padding);

        _highlightRect.pivot = _iconRect.pivot;
        _highlightRect.anchorMin = _iconRect.anchorMin;
        _highlightRect.anchorMax = _iconRect.anchorMax;
        _highlightRect.offsetMin = _iconRect.offsetMin;
        _highlightRect.offsetMax = _iconRect.offsetMax;

        _iconImage.raycastTarget = false;
        _highlightImage.raycastTarget = false;

        HideIcon();
        _highlightGo.SetActive(false);
    }

    private void ShowIcon() => _iconGo.SetActive(true);
    private void HideIcon() => _iconGo.SetActive(false);

    private void ShowText() => _textGo.SetActive(true);
    private void HideText() => _textGo.SetActive(false);

    public void SetSlotIndex(int index) => Index = index;

    public void SetSlotAccessibleState(bool value)
    {
        if (_isAccessibleSlot == value) return;

        if (value)
        {
            _slotImage.color = Color.black;
        }
        else
        {
            _slotImage.color = InaccessibleSlotColor;
            HideIcon();
            HideText();
        }

        _isAccessibleSlot = value;
    }

    public void SetItemAccessibleState(bool value)
    {
        if (_isAccessibleItem == value) return;

        if (value)
        {
            _iconImage.color = Color.white;
            _amountText.color = Color.white;
        }
        else
        {
            _iconImage.color = InaccessibleIconColor;
            _amountText.color = InaccessibleIconColor;
        }

        _isAccessibleItem = value;
    }

    public void SwapOrMoveIcon(ItemSlotUI other)
    {
        if (other == null) return;
        if (other == this) return;
        if (!this.IsAccessible) return;
        if (!other.IsAccessible) return;

        var temp = _iconImage.sprite;

        if (other.HasItem) SetItem(other._iconImage.sprite);

        else RemoveItem();

        other.SetItem(temp);
    }

    public void SetItem(Sprite itemSprite)
    {

        if (itemSprite != null)
        {
            _iconImage.sprite = itemSprite;
            ShowIcon();
        }
        else
        {
            RemoveItem();
        }
    }

    public void RemoveItem()
    {
        _iconImage.sprite = null;
        HideIcon();
        HideText();
    }

    public void SetIconAlpha(float alpha)
    {
        _iconImage.color = new Color(
            _iconImage.color.r, _iconImage.color.g, _iconImage.color.b, alpha
        );
    }

    public void SetItemAmount(int amount)
    {

        if (HasItem && amount > 1)
            ShowText();
        else
            HideText();

        _amountText.text = amount.ToString();
    }

    public void Highlight(bool show)
    {
        if (!this.IsAccessible) return;

        if (show)
            StartCoroutine(nameof(HighlightFadeInRoutine));
        else
            StartCoroutine(nameof(HighlightFadeOutRoutine));
    }

    public void SetHighlightOnTop(bool value)
    {
        if (value)
            _highlightRect.SetAsLastSibling();
        else
            _highlightRect.SetAsFirstSibling();
    }

    private IEnumerator HighlightFadeInRoutine()
    {
        StopCoroutine(nameof(HighlightFadeOutRoutine));
        _highlightGo.SetActive(true);

        float unit = _highlightAlpha / _highlightFadeDuration;

        for (; _currentHLAlpha <= _highlightAlpha; _currentHLAlpha += unit * Time.deltaTime)
        {
            _highlightImage.color = new Color(
                _highlightImage.color.r,
                _highlightImage.color.g,
                _highlightImage.color.b,
                _currentHLAlpha
            );

            yield return null;
        }
    }

    private IEnumerator HighlightFadeOutRoutine()
    {
        StopCoroutine(nameof(HighlightFadeInRoutine));

        float unit = _highlightAlpha / _highlightFadeDuration;

        for (; _currentHLAlpha >= 0f; _currentHLAlpha -= unit * Time.deltaTime)
        {
            _highlightImage.color = new Color(
                _highlightImage.color.r,
                _highlightImage.color.g,
                _highlightImage.color.b,
                _currentHLAlpha
            );

            yield return null;
        }

        _highlightGo.SetActive(false);
    }

}