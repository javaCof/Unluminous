using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupContent : MonoBehaviour
{
    [HideInInspector] public bool popupEnable;

    public void ClosePopup()
    {
        GetComponentInParent<PopupUI>().PopupClose(this);
    }
}
