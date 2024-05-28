using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxSetting : MonoBehaviour
{
    private AudioSource audio;

    private void Awake()
    {
        audio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        audio.volume = GameManager.Instance.SfxVolume;
    }

    public void SetSfx(float volume)
    {
        audio.volume = volume;
    }
}
