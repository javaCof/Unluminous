using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class VRCanvas : MonoBehaviour
{
    public Camera uiCam;
    public GameObject vrInteracter;

    private void Start()
    {
        GameManager.Instance.VrOnOff(GameManager.Instance.VrEnable);
    }
}
