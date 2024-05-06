using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonPool : ObjectPool
{
    public enum PhotonInstantiateOption { LOCAL, STANDARD, SCENE_OBJECT };

    public PhotonPool(string name, int id, int n, PhotonInstantiateOption option = PhotonInstantiateOption.SCENE_OBJECT) : base(id, n)
    {
        for (int i = 0; i < n; i++)
        {
            GameObject go = (option == PhotonInstantiateOption.SCENE_OBJECT) ?
                PhotonNetwork.InstantiateSceneObject(name, Vector3.zero, Quaternion.identity, 0, null) :
                PhotonNetwork.Instantiate(name, Vector3.zero, Quaternion.identity, 0, null);

            objects.Add(go);
            go.GetComponent<IPoolObject>().OnPoolCreate(id);
        }
    }

    public override GameObject GetObject(Vector3 pos, Quaternion rot, Transform parent)
    {
        if (idx < objects.Count)
        {
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