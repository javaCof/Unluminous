using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonPool : ObjectPool
{
    public PhotonPool(string name, int n) : base(n)
    {
        for (int i = 0; i < n; i++)
        {
            GameObject go = PhotonNetwork.InstantiateSceneObject(name, Vector3.zero, Quaternion.identity, 0, null);
            objects.Add(go);
            go.GetComponent<IPhotonPoolObject>().OnPoolCreate();
        }
    }

    public override GameObject GetObject(Vector3 pos, Quaternion rot, Transform parent)
    {
        if (idx < objects.Count)
        {
            objects[idx].transform.position = pos;
            objects[idx].transform.rotation = rot;
            objects[idx].GetComponent<IPhotonPoolObject>().OnPoolEnable();

            return objects[idx++];
        }
        else return null;
    }

    public override void DisableObject(GameObject go)
    {
        go.GetComponent<IPhotonPoolObject>().OnPoolDisable();
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
