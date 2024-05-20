using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;



public class AnimateOnInput : MonoBehaviour
{
    float leftValue;
    float rightValue;

    [SerializeField] XRController m_left;
    [SerializeField] XRController m_right;

    public Animator anim;

    public float actionValue;

    // Update is called once per frame
    void Update()
    {
        isGesture();
    }

    void isGesture()
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
    }
}
