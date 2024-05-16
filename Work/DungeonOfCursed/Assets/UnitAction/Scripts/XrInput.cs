using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

//CommonUsages.triggerButton
//CommonUsages.thumbrest
//CommonUsages.primary2DAxisClick
//CommonUsages.primary2DAxisTouch
//CommonUsages.menuButton
//CommonUsages.gripButton
//CommonUsages.secondaryButton
//CommonUsages.secondaryTouch
//CommonUsages.primaryButton
//CommonUsages.primaryTouch

public class XrInput : MonoBehaviour
{
    [SerializeField] XRController m_left;

    [SerializeField] Animator anim;

    Vector2 pos;



    // Update is called once per frame
    void Update()
    {

        IsWalk();
    }


    //���̽�ƽ ���� ���� �ȴ� ������� �Լ�
    void IsWalk()
    {
        if (pos == null)
        {
            anim.SetBool("move", false);
        }
        //��Ʈ�ѷ� ���̽�ƽ ��ǲ���� pos�� ����
        else if (m_left.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out pos))
        {
            //���� ���̽�ƽ�� ���� ���Ⱑ 0.1���� Ŭ��
            if (Mathf.Abs(pos.y) > 0.1f)
            {
                //�ȴ� ���
                anim.SetBool("move", true);
            }
            //0.1���� ������
            else if (Mathf.Abs(pos.y) > 0.1f)
            {
                //������ ���
                anim.SetBool("move", false);
            }

        }

    }
}
