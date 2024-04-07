using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitState
{
    public UnitCharater unit;
    public UnitState(UnitCharater _unit) => unit = _unit;
    public abstract void BeginState();
    public abstract void UpdateState();
    public abstract void EndState();
}
