using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;



public class XrInput : MonoBehaviour
{
    float leftValue;
    float rightValue;

    public AudioSource audioSource;

    [SerializeField] XRController m_left;
    [SerializeField] XRController m_right;

    public Animator anim;

    Vector2 pos;


    // Update is called once per frame
    void Update()
    {
        StartCoroutine(isGesture());
        StartCoroutine(IsWalk());
    }

    IEnumerator isGesture()
    {
        //�޼� �׸�
        if (m_left.inputDevice.TryGetFeatureValue(CommonUsages.grip, out leftValue))
        {
            anim.SetFloat("Left Grab", leftValue);
        }

        //������ �׸�
        if (m_right.inputDevice.TryGetFeatureValue(CommonUsages.grip, out rightValue))
        {
            anim.SetFloat("Right Grab", rightValue);
        }

        //�޼� Ʈ����
        if (m_left.inputDevice.TryGetFeatureValue(CommonUsages.trigger, out leftValue))
        {
            anim.SetFloat("Left Pinch", leftValue);
        }

        //������ Ʈ����
        if (m_right.inputDevice.TryGetFeatureValue(CommonUsages.trigger, out rightValue))
        {
            anim.SetFloat("Right Pinch", rightValue);
        }

        yield return null;
    }



    IEnumerator IsWalk()
    {

        //��Ʈ�ѷ� ���̽�ƽ ��ǲ���� pos�� ����
        if (m_left.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out pos))
        {
            //���� ���̽�ƽ�� ���� ���Ⱑ 0.1���� Ŭ��
            if (Mathf.Abs(pos.y) > 0.1f)
            {
                //�ȴ� ���
                anim.SetBool("move", true);
                SoundManager.instance.PlayRun(audioSource);


            }
            //0.1���� ������
            else if (Mathf.Abs(pos.y) < 0.1f)
            {

                //������ ���
                anim.SetBool("move", false);

            }
        }

        yield return null;
    }
}
