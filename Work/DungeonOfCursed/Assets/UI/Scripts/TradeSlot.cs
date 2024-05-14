using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradeSlot : MonoBehaviour
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

    private bool _isAccessibleSlot = true; // 슬롯 접근가능 여부
    private bool _isAccessibleItem = true; // 아이템 접근가능 여부

    private RectTransform _slotRect;
    private RectTransform _iconRect;
    private RectTransform _highlightRect;

    private GameObject _iconGo;
    private GameObject _textGo;
    private GameObject _highlightGo;

    private float _currentHLAlpha = 0f;
    
    public Image _slotImage;

    private static readonly Color InaccessibleSlotColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
    private static readonly Color InaccessibleIconColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    void Awkae()
    {
        _slotRect = GetComponent<RectTransform>();
        _iconRect = _iconImage.rectTransform;
        _highlightRect = _highlightImage.rectTransform;

        _iconGo = _iconRect.gameObject;
        _textGo = _amountText.gameObject;
        _highlightGo = _highlightImage.gameObject;

        _slotImage = GetComponent<Image>();
    }
}
