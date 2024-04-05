using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitState : MonoBehaviour
{
    public abstract void BeginState();
    public abstract void UpdateState();
    public abstract void EndState();


}
