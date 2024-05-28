using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public PopupContent settingPopup;
    public PopupContent invenPopup;
    public PopupContent tradePopup;

    public GameObject dmgEffect;

    public GameObject leftJoyStick;
    public Button actionButton;
    public Button jumpButton;
    public TouchPad touchPad;
    public Image hpBar;

    private PopupUI popup;

    private void Awake()
    {
        popup = GetComponent<PopupUI>();
    }

    private void Start()
    {
        leftJoyStick.SetActive(false);
        actionButton.gameObject.SetActive(false);
        jumpButton.gameObject.SetActive(false);
        touchPad.gameObject.SetActive(false);

#if UNITY_ANDROID
        leftJoyStick.SetActive(true);
        actionButton.gameObject.SetActive(true);
        jumpButton.gameObject.SetActive(true);
        touchPad.gameObject.SetActive(true);
#endif
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            popup.PopupOnOff(settingPopup);
            SoundManager.instance.PlaySfx("menu");
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            popup.PopupOnOff(invenPopup);
            if (invenPopup.popupEnable) SoundManager.instance.PlaySfx("inven");
        }
    }

    public void OpenSetting()
    {
        popup.PopupOpen(settingPopup);
        SoundManager.instance.PlaySfx("menu");
    }
    public void OpenInven()
    {
        popup.PopupOpen(invenPopup);
        SoundManager.instance.PlaySfx("inven");
    }
    public void OpenTrade()
    {
        popup.PopupOpen(tradePopup);
        SoundManager.instance.PlaySfx("inven");
    }

    public void DmgEffect(Vector3 pos, float dmg)
    {
        GameObject go = Instantiate(dmgEffect, Camera.main.WorldToScreenPoint(pos), Quaternion.identity, transform);
        go.GetComponent<Text>().text = dmg.ToString();
    }
}
