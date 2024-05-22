using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupUI : MonoBehaviour
{
    private GameObject nowPopup;

    public void OpenPopup(GameObject popup)
    {
        if (nowPopup == null)
        {
            nowPopup = Instantiate(popup, transform);

            SoundManager.instance.PlaySfx("menu");
        }
    }

    public void ClosePopup()
    {
        if (nowPopup != null)
        {
            Destroy(nowPopup);
            nowPopup = null;
            SoundManager.instance.PlaySfx("menu");
        }
    }
}
