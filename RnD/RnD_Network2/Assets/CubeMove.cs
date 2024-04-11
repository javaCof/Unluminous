using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMove : MonoBehaviour
{
    int dDir = 1;
    void Update()
    {
        transform.position += Vector3.right * dDir * 5f * Time.deltaTime;
        if (Mathf.Abs(transform.position.x) > 3f) dDir = -dDir;
    }
}
