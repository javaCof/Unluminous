using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltipUI : MonoBehaviour
{
    [SerializeField]
    private Text _titleText;

    [SerializeField]
    private Text _contentText;

    [SerializeField]
    private Text _priceText;

    [SerializeField]
    private Image _iconImg;

    private RectTransform _rt;
    private CanvasScaler _canvasScaler;

    private static readonly Vector2 LeftTop = new Vector2(0f, 1f);
    private static readonly Vector2 LeftBottom = new Vector2(0f, 0f);
    private static readonly Vector2 RightTop = new Vector2(1f, 1f);
    private static readonly Vector2 RightBottom = new Vector2(1f, 0f);

    private void Awake()
    {
        //Init();
    }
    private void Init()
    {
        TryGetComponent(out _rt);
        _rt.pivot = LeftTop;
        _canvasScaler = GetComponentInParent<CanvasScaler>();

        DisableAllChildrenRaycastTarget(transform);
    }

    private void DisableAllChildrenRaycastTarget(Transform tr)
    {
        tr.TryGetComponent(out Graphic gr);
        if (gr != null)
            gr.raycastTarget = false;

        int childCount = tr.childCount;
        if (childCount == 0) return;

        for (int i = 0; i < childCount; i++)
        {
            DisableAllChildrenRaycastTarget(tr.GetChild(i));
        }
    }

    public void SetItemInfo(ItemData data)
    {
        if (data != null)
        {
            int price = data.Price;
            string name = data.Icon;

            _titleText.text = data.Name;
            _contentText.text = data.Tooltip;
            _priceText.text = "Price:" + price.ToString();
            _iconImg.sprite = Resources.Load<Sprite>(name);
        }
        else
            Debug.Log("데이터 없음");
    }

    public void SetRectPosition(RectTransform slotRect)
    {

        float wRatio = Screen.width / _canvasScaler.referenceResolution.x;
        float hRatio = Screen.height / _canvasScaler.referenceResolution.y;
        float ratio =
            wRatio * (1f - _canvasScaler.matchWidthOrHeight) +
            hRatio * (_canvasScaler.matchWidthOrHeight);

        float slotWidth = slotRect.rect.width * ratio;
        float slotHeight = slotRect.rect.height * ratio;

        _rt.position = slotRect.position + new Vector3(slotWidth, -slotHeight);
        Vector2 pos = _rt.position;

        float width = _rt.rect.width * ratio;
        float height = _rt.rect.height * ratio;

        bool rightTruncated = pos.x + width > Screen.width;
        bool bottomTruncated = pos.y - height < 0f;

        ref bool R = ref rightTruncated;
        ref bool B = ref bottomTruncated;

        if (R && !B)
        {
            _rt.position = new Vector2(pos.x - width - slotWidth, pos.y);
        }
        else if (!R && B)
        {
            _rt.position = new Vector2(pos.x, pos.y + height + slotHeight);
        }
        else if (R && B)
        {
            _rt.position = new Vector2(pos.x - width - slotWidth, pos.y + height + slotHeight);
        }

    }

   // public void Show() => gameObject.SetActive(true);

   // public void Hide() => gameObject.SetActive(false);

}