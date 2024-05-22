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
        //왼손 그립
        if (m_left.inputDevice.TryGetFeatureValue(CommonUsages.grip, out leftValue))
        {
            anim.SetFloat("Left Grab", leftValue);
        }

        //오른손 그립
        if (m_right.inputDevice.TryGetFeatureValue(CommonUsages.grip, out rightValue))
        {
            anim.SetFloat("Right Grab", rightValue);
        }

        //왼손 트리거
        if (m_left.inputDevice.TryGetFeatureValue(CommonUsages.trigger, out leftValue))
        {
            anim.SetFloat("Left Pinch", leftValue);
        }

        //오른손 트리거
        if (m_right.inputDevice.TryGetFeatureValue(CommonUsages.trigger, out rightValue))
        {
            anim.SetFloat("Right Pinch", rightValue);
        }

        yield return null;
    }



    IEnumerator IsWalk()
    {

        //컨트롤러 조이스틱 인풋값을 pos에 넣음
        if (m_left.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out pos))
        {
            //만약 조이스틱의 상하 기울기가 0.1보다 클때
            if (Mathf.Abs(pos.y) > 0.1f)
            {
                //걷는 모션
                anim.SetBool("move", true);
                SoundManager.instance.PlayRun(audioSource);


            }
            //0.1보다 작을때
            else if (Mathf.Abs(pos.y) < 0.1f)
            {

                //가만히 모션
                anim.SetBool("move", false);

            }
        }

        yield return null;
    }
}
