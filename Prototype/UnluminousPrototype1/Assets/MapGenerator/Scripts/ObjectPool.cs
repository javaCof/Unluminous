using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    List<GameObject> objects;
    Transform pos;
    int idx;

    public ObjectPool(GameObject obj, int n, Transform pos)
    {
        objects = new List<GameObject>(n);
        this.pos = pos;

        for (int i = 0; i < n; i++)
        {
            GameObject go = GameObject.Instantiate(obj, pos);
            go.SetActive(false);
            objects.Add(go);
        }
    }

    public GameObject GetObject(Vector3 pos, Quaternion rot, Transform parent)
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

    public void Reset()
    {
        foreach (GameObject go in objects)
        {
            go.transform.parent = pos;
            go.SetActive(false);
        }
        idx = 0;
    }
}
