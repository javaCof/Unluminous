using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundManager : MonoBehaviour
{

 

    public void SfxPlay(string sfxName,AudioClip clip)
    {
        //소리 게임오브젝트 생성
        GameObject temp = new GameObject(sfxName + "Sound");

        //생성한 겜 오브젝트에 오디오소스 추가 하고 그걸 참조하는 오디오소스 변수
        AudioSource audioSource = temp.AddComponent<AudioSource>();

        //받아온 클립을 오디오 소스에 넣음
        audioSource.clip = clip;

        //플레이
        audioSource.Play();
    }


}
