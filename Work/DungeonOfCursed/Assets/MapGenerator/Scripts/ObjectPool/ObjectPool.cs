using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectPool
{
    protected int id;
    protected List<GameObject> objects;
    protected int idx;

    protected ObjectPool(int id, int n)
    {
        this.id = id;
        objects = new List<GameObject>(n);
    }

    public abstract GameObject GetObject(Vector3 pos, Quaternion rot, Transform parent = null);
    public abstract void DisableObject(GameObject go);
    public abstract void Reset();
}
