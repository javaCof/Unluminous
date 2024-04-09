using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [ContextMenu("Gen Item")]
    public void Gen()
    {
        GetComponent<ItemGenerater>().GenerateItem();
    }
}
