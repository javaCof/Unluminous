using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolObject
{
    public void OnPoolCreate(int id);
    public void OnPoolEnable(Vector3 pos, Quaternion rot);
    public void OnPoolDisable();
}
