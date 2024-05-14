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

    
   

    // Update is called once per frame
    void Update()
    {
        if (m_left.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 pos))
        {
            Debug.Log(pos.y);
            
        }
        

       
    }
}
