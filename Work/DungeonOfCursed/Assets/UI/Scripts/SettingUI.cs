using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    public Slider bgmSlider;
    public Slider sfxSlider;
    public Slider sensiSlider;
    public Button bgmBtn;
    public Button sfxBtn;
    public Button homeBtn;
    public Button exitBtn;
    public Button closeBtn;
    public Button vrBtn;

    private PopupContent popup;

    [SerializeField] private float bgmVolume;
    [SerializeField] private float sfxVolume;

    private void Awake()
    {
        popup = GetComponent<PopupContent>();
    }
    private void Start()
    {
        bgmSlider.value = GameManager.Instance.BgmVolume;
        sfxSlider.value = GameManager.Instance.SfxVolume;
        sensiSlider.value = GameManager.Instance.InputSensitivity;

        bgmSlider.onValueChanged.AddListener((float value) => {
            UpdateBtnUI(bgmBtn, value > 0);
            GameManager.Instance.BgmVolume = value;
        });
        sfxSlider.onValueChanged.AddListener((float value) => {
            UpdateBtnUI(sfxBtn, value > 0);
            GameManager.Instance.SfxVolume = value;
        });
        sensiSlider.onValueChanged.AddListener((float value) => GameManager.Instance.InputSensitivity = value);

        bgmBtn.onClick.AddListener(() => {
            if (bgmSlider.value > 0)
            {
                bgmVolume = bgmSlider.value;
                bgmSlider.value = 0;
            }
            else
            {
                bgmSlider.value = bgmVolume;
            }
            UpdateBtnUI(bgmBtn, bgmSlider.value > 0);
        });
        sfxBtn.onClick.AddListener(() => {
            if (sfxSlider.value > 0)
            {
                sfxVolume = sfxSlider.value;
                sfxSlider.value = 0;
            }
            else
            {
                sfxSlider.value = sfxVolume;
            }
            UpdateBtnUI(sfxBtn, sfxSlider.value > 0);
        });
        UpdateBtnUI(bgmBtn, bgmSlider.value > 0);
        UpdateBtnUI(sfxBtn, sfxSlider.value > 0);

        homeBtn.onClick.AddListener(() => {
            if (GameManager.Instance.curScene == "MenuScene") return;
            if (PhotonNetwork.inRoom) PhotonNetwork.LeaveRoom();
            GameManager.Instance.LoadingScene("MenuScene");
        });
        exitBtn.onClick.AddListener(() => {
            if (PhotonNetwork.inRoom) PhotonNetwork.LeaveRoom();
            GameManager.Instance.ExitGame(true);
        });
        closeBtn.onClick.AddListener(() => {
            popup.ClosePopup();
            SoundManager.instance.PlaySfx("menu");
        });

        if (GameManager.Instance.curScene == "MenuScene") vrBtn.gameObject.SetActive(false);
        vrBtn.GetComponentInChildren<Text>().text = GameManager.Instance.VrEnable ? "PC" : "VR";
    }

    void UpdateBtnUI(Button btn, bool on)
    {
        btn.GetComponent<Image>().color = on ? Color.white : Color.red;
    }
    public void SwitchVr()
    {
        GameManager.Instance.VrOnOff();
        vrBtn.GetComponentInChildren<Text>().text = GameManager.Instance.VrEnable ? "PC" : "VR";
        SoundManager.instance.PlaySfx("menu");
    }
}
