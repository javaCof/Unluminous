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


    //조이스틱 값을 통해 걷는 모션조절 함수
    void IsWalk()
    {
        if (pos == null)
        {
            anim.SetBool("move", false);
        }
        //컨트롤러 조이스틱 인풋값을 pos에 넣음
        else if (m_left.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out pos))
        {
            //만약 조이스틱의 상하 기울기가 0.1보다 클때
            if (Mathf.Abs(pos.y) > 0.1f)
            {
                //걷는 모션
                anim.SetBool("move", true);
            }
            //0.1보다 작을때
            else if (Mathf.Abs(pos.y) > 0.1f)
            {
                //가만히 모션
                anim.SetBool("move", false);
            }

        }

    }
}
