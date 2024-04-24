using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupContent : MonoBehaviour
{
    private PopupUI popupUI;

    private void Awake()
    {
        popupUI = transform.GetComponentInParent<PopupUI>();
    }

    public void ClosePopup()
    {
        popupUI.ClosePopup();
    }
}
