using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitCharater : MonoBehaviour
{
    protected UnitStatInfo statInfo;
    protected float curHP;

    protected UnitState state;
    public void ChangeState(UnitState state)
    {
        if (this.state != null) this.state.EndState();
        this.state = state;
        if (this.state != null) this.state.BeginState();
    }

    protected string model_name;
    protected string icon_name;

    public abstract void Hit(UnitCharater other);
    public abstract void Attack(UnitCharater target);
    public abstract void Dead();
}

// { Search, Alert, Trace, Attack, Repos, Hit, Dead };
