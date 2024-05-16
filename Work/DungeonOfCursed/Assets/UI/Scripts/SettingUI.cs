using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    public Slider bgmSlider;

    private GameManager game;

    private void Awake()
    {
        game = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        bgmSlider.value = game.BgmVolume;
    }

    public void OnBgmVolumeChanged(Slider slider)
    {
        game.BgmVolume = slider.value;
    }

    public void OnSfxVolumeChanged(Slider slider)
    {
        game.SfxVolume = slider.value;
    }

    public void OnInputSensitivityChanged(Slider slider)
    {
        game.InputSensitivity = slider.value;
    }
}
