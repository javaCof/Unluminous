using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public List<AudioClip> music;

    public List<AudioClip> sfx;

    public List<AudioClip> playerSounds;

    AudioSource audioSource;

    private void Awake()
    {
        instance = this;
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    public void PlaySfx(string sfxName)
    {
        switch (sfxName)
        {
            case "chest":
                audioSource.clip = sfx[0];
                audioSource.Play();
                break;
            case "inven": //아직
                audioSource.clip = sfx[1];
                audioSource.Play();
                break;
            case "item": 
                audioSource.clip = sfx[2];
                audioSource.Play();
                break;
            case "menu":
                audioSource.clip = sfx[3];
                audioSource.Play();
                break;
            case "portal": 
                audioSource.clip = sfx[4];
                audioSource.Play();
                break;
        }
    }

   

    public void PlayWin()
    {
        audioSource.clip = music[1];

        audioSource.Play();
    }


    public void PlayLose()
    {
        audioSource.clip = music[2];

        audioSource.Play();
    }

    public void PlayHit(AudioSource player)
    {
       player.clip = playerSounds[0];
        player.volume = 0.5f;

        //플레이
        player.Play();
        
    }

    public void PlayRun(AudioSource player)
    {
        
        if (!player.isPlaying)
        {
            player.clip = playerSounds[1];
            player.Play();
        }
        
        
    }

    public void PlaySwrod(AudioSource player)
    {
        //검 소리 무작위
        player.clip = playerSounds[Random.RandomRange(3,7)];

        //플레이
        player.Play();
    }

}
