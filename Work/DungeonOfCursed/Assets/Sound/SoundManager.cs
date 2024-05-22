using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SoundManager : MonoBehaviour
{
    public List<AudioClip> music;

    public List<AudioClip> sfx;

    public List<AudioClip> playerSounds;

    public void PlaySfx(string sfxName)
    {
        //소리 게임오브젝트 생성
        GameObject temp = new GameObject(sfxName + "Sound");

        //생성한 겜 오브젝트에 오디오소스 추가 하고 그걸 참조하는 오디오소스 변수
        AudioSource audioSource = temp.AddComponent<AudioSource>();

        switch (sfxName)
        {
            case "chest":
                audioSource.clip = sfx[0];
                audioSource.Play();
                Destroy(temp, sfx[0].length + 0.1f);
                break;
            case "inven":
                audioSource.clip = sfx[1];
                audioSource.Play();
                Destroy(temp, sfx[1].length + 0.1f);
                break;
            case "item":
                audioSource.clip = sfx[2];
                Destroy(temp, sfx[2].length + 0.1f);
                break;
            case "menu":
                audioSource.clip = sfx[3];
                Destroy(temp, sfx[3].length + 0.1f);
                break;
            case "portal":
                audioSource.clip = sfx[4];
                Destroy(temp, sfx[4].length + 0.1f);
                break;
        }
    }

    //public void PlayBgm()
    //{

    //    AudioSource audioSource = gameObject.GetComponent<AudioSource>();

    //    audioSource.clip = music[0];
    //    audioSource.loop = true;
    //    audioSource.playOnAwake = true;



    //}

    public void PlayWin()
    {
        GameObject temp = new GameObject("WinSound");

        AudioSource audioSource = temp.AddComponent<AudioSource>();

        audioSource.clip = music[1];

        audioSource.Play();
        Destroy(temp, music[1].length + 0.1f);
    }


    public void PlayLose()
    {
        GameObject temp = new GameObject("LoseSound");

        AudioSource audioSource = temp.AddComponent<AudioSource>();

        audioSource.clip = music[2];

        audioSource.Play();

        Destroy(temp, sfx[2].length + 0.1f); 
    }

    public void PlayHit()
    {
        //소리 게임오브젝트 생성
        GameObject temp = new GameObject("HitSound");

        //생성한 겜 오브젝트에 오디오소스 추가 하고 그걸 참조하는 오디오소스 변수
        AudioSource audioSource = temp.AddComponent<AudioSource>();

        audioSource.clip = playerSounds[0];

        //플레이
        audioSource.Play();
    }

    public void PlayRun()
    {
        //소리 게임오브젝트 생성
        GameObject temp = new GameObject("RunSound");

        //생성한 겜 오브젝트에 오디오소스 추가 하고 그걸 참조하는 오디오소스 변수
        AudioSource audioSource = temp.AddComponent<AudioSource>();


        audioSource.clip = playerSounds[1];

        //플레이
        audioSource.Play();
    }

    public void PlaySwrod()
    {
        //소리 게임오브젝트 생성
        GameObject temp = new GameObject("SwordSound");

        //생성한 겜 오브젝트에 오디오소스 추가 하고 그걸 참조하는 오디오소스 변수
        AudioSource audioSource = temp.AddComponent<AudioSource>();

        //검 소리 무작위
        audioSource.clip = playerSounds[Random.RandomRange(3,7)];

        //플레이
        audioSource.Play();
    }

}
