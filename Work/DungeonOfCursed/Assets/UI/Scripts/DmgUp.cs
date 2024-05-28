using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DmgUp : MonoBehaviour
{
    RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        Vector2 pos = rect.anchoredPosition;
        pos += Vector2.up * 100f * Time.deltaTime;
        rect.anchoredPosition = pos;

        if (rect.anchoredPosition.y > 100f)
        {
            Destroy(gameObject);
        }
    }
}
