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

    private PopupContent popup;
    private GameManager game;

    private float bgmVolume;
    private float sfxVolume;

    private void Awake()
    {
        popup = GetComponent<PopupContent>();
        game = FindObjectOfType<GameManager>();
    }
    private void Start()
    {
        bgmSlider.value = game.BgmVolume;
        sfxSlider.value = game.SfxVolume;
        sensiSlider.value = game.InputSensitivity;

        bgmSlider.onValueChanged.AddListener((float value) => {
            UpdateBtnUI(bgmBtn, value > 0);
            bgmVolume = value;
            game.BgmVolume = value;
        });
        sfxSlider.onValueChanged.AddListener((float value) => {
            UpdateBtnUI(sfxBtn, value > 0);
            sfxVolume = value;
            game.SfxVolume = value;
        });
        sensiSlider.onValueChanged.AddListener((float value) => game.InputSensitivity = value);

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

        homeBtn.onClick.AddListener(() => StartCoroutine(GoToMenu()));
        exitBtn.onClick.AddListener(() => game.ExitGame(true));
        closeBtn.onClick.AddListener(() => popup.ClosePopup());
    }

    IEnumerator GoToMenu()
    {
        if (game.curSceneName == "MenuScene") yield break;

        yield return game.StartLoading();
        if (PhotonNetwork.inRoom) PhotonNetwork.LeaveRoom();
        yield return game.ChangeScene("MenuScene");
        yield return game.EndLoading();
    }
    void UpdateBtnUI(Button btn, bool on)
    {
        btn.GetComponent<Image>().color = on ? Color.white : Color.red;
    }
}
