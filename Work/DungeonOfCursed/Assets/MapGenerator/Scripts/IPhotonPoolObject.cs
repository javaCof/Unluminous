using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPhotonPoolObject
{
    public void OnPoolCreate();
    public void OnPoolEnable();
    public void OnPoolDisable();
}
