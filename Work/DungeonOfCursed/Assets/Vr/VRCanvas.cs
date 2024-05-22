using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class VRCanvas : MonoBehaviour
{
    public Camera uiCam;
    public GameObject vrInteracter;

    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;

    private void Start()
    {
        GameManager.Instance.VrOnOff(GameManager.Instance.VrEnable);
    }
}
