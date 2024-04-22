using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePool : ObjectPool
{
    Transform pos;

    public ResourcePool(string name, int n, Transform pos) : base(n)
    {
        this.pos = pos;

        for (int i = 0; i < n; i++)
        {
            GameObject go = GameObject.Instantiate(Resources.Load<GameObject>(name), Vector3.zero, Quaternion.identity);
            go.transform.parent = pos;
            go.SetActive(false);
            objects.Add(go);
        }
    }

    public override GameObject GetObject(Vector3 pos, Quaternion rot, Transform parent)
    {
        if (idx < objects.Count)
        {
            objects[idx].transform.position = pos;
            objects[idx].transform.rotation = rot;
            objects[idx].transform.parent = parent;
            objects[idx].SetActive(true);
            return objects[idx++];
        }
        else return null;
    }

    public override void DisableObject(GameObject go)
    {
        go.transform.parent = pos;
        go.SetActive(false);
    }

    public override void Reset()
    {
        foreach (GameObject go in objects)
        {
            DisableObject(go);
        }
        idx = 0;
    }
}
