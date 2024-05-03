using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePool : ObjectPool
{
    Transform pos;

    public ResourcePool(string name, int id, int n, Transform pos) : base(id, n)
    {
        this.pos = pos;

        for (int i = 0; i < n; i++)
        {
            GameObject go = GameObject.Instantiate(Resources.Load<GameObject>(name), Vector3.zero, Quaternion.identity);
            go.transform.parent = pos;
            go.SetActive(false);
            objects.Add(go);
            go.GetComponent<IPoolObject>().OnPoolCreate(id);
        }
    }
    public ResourcePool(GameObject obj, int id, int n, Transform pos) : base(id, n)
    {
        this.pos = pos;

        for (int i = 0; i < n; i++)
        {
            GameObject go = GameObject.Instantiate(obj, Vector3.zero, Quaternion.identity);
            go.transform.parent = pos;
            go.SetActive(false);
            objects.Add(go);
            go.GetComponent<IPoolObject>().OnPoolCreate(id);
        }
    }

    public override GameObject GetObject(Vector3 pos, Quaternion rot, Transform parent)
    {
        if (idx < objects.Count)
        {
            objects[idx].transform.parent = parent;
            objects[idx].transform.position = pos;
            objects[idx].transform.rotation = rot;
            objects[idx].SetActive(true);
            objects[idx].GetComponent<IPoolObject>().OnPoolEnable(pos, rot);
            return objects[idx++];
        }
        else
        {
            Debug.LogError("OBJECT POOL IS FULL : " + id);
            return null;
        }
    }

    public override void DisableObject(GameObject go)
    {
        go.transform.parent = pos;
        go.SetActive(false);
        go.GetComponent<IPoolObject>().OnPoolDisable();
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
