using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundManager : MonoBehaviour
{

 

    public void SfxPlay(string sfxName,AudioClip clip)
    {
        //�Ҹ� ���ӿ�����Ʈ ����
        GameObject temp = new GameObject(sfxName + "Sound");

        //������ �� ������Ʈ�� ������ҽ� �߰� �ϰ� �װ� �����ϴ� ������ҽ� ����
        AudioSource audioSource = temp.AddComponent<AudioSource>();

        //�޾ƿ� Ŭ���� ����� �ҽ��� ����
        audioSource.clip = clip;

        //�÷���
        audioSource.Play();
    }


}
