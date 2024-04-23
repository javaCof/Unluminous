using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISizeFitter : MonoBehaviour
{
    public int viewHeight = 1080;
    public Font font;

    private RectTransform canvas;

    private void Awake()
    {
        canvas = GetComponent<RectTransform>();
    }

    private void Start()
    {
        float heightRate = (float)Screen.height / viewHeight;

        foreach (var ui in canvas.GetComponentsInChildren<RectTransform>(true))
        {
            ui.anchoredPosition *= heightRate;
            ui.sizeDelta *= heightRate;
        }
        foreach (var txt in canvas.GetComponentsInChildren<Text>(true))
        {
            txt.fontSize = (int)(txt.fontSize * heightRate);
            if (font != null) txt.font = font;
        }
    }
}
