using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmSetting : MonoBehaviour
{
    private AudioSource audio;

    private void Awake()
    {
        audio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        //audio.volume = SoundManager.instance.bgm;
    }

    public void SetBgm(float volume)
    {
        audio.volume = volume;
    }
}
