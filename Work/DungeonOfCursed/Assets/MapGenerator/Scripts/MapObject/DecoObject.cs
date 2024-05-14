using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecoObject : MonoBehaviour, IPoolObject
{
    public float deployHeight;

    private void Deploy()
    {
        Vector3 pos = transform.localPosition;
        pos.y = deployHeight;
        transform.localPosition = pos;
    }

    public void OnPoolCreate(int id) { }
    public void OnPoolEnable(Vector3 pos, Quaternion rot) => Deploy();
    public void OnPoolDisable() { }
}