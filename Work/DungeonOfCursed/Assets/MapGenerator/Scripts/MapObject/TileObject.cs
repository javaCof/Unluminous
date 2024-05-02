using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObject : MonoBehaviour, IPoolObject
{
    public void OnPoolCreate(int id) { }
    public void OnPoolDisable() { }
    public void OnPoolEnable(Vector3 pos, Quaternion rot) { }
}
