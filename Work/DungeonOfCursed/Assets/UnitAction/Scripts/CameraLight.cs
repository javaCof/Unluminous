using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLight : MonoBehaviour
{
    public float minLightIntensity = 0.5f;
    public float maxLightIntensity = 1f;
    public float deltaIntensity = 0.01f;

    private Light camLight;
    private float curLightIntensity;
    private bool incIntensity;

    private void Awake()
    {
        camLight = GetComponentInChildren<Light>();
    }

    public void LightOn()
    {
        curLightIntensity = 0f;
        incIntensity = true;
    }

    private void Update()
    {
        if (incIntensity && curLightIntensity > maxLightIntensity) incIntensity = false;
        if (!incIntensity && curLightIntensity < minLightIntensity) incIntensity = true;

        curLightIntensity += (incIntensity ? deltaIntensity : -deltaIntensity) * Time.deltaTime;

        camLight.intensity = curLightIntensity;
    }
}
