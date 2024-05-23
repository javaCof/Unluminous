using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupUI : MonoBehaviour
{
    private int popupCount = 0;

    public void PopupOpen(PopupContent popup)
    {
        if (popup.popupEnable) return;
        popup.popupEnable = true;
        popup.gameObject.SetActive(true);
        popupCount++;
        GameManager.Instance.enablePopup = (popupCount > 0);
    }
    public void PopupClose(PopupContent popup)
    {
        if (!popup.popupEnable) return;
        popup.popupEnable = false;
        popup.gameObject.SetActive(false);
        popupCount--;
        GameManager.Instance.enablePopup = (popupCount > 0);
    }
    public void PopupOnOff(PopupContent popup)
    {
        popup.popupEnable = !popup.popupEnable;
        popup.gameObject.SetActive(popup.popupEnable);
        popupCount += popup.popupEnable ? 1 : -1;
        GameManager.Instance.enablePopup = (popupCount > 0);
    }
}
