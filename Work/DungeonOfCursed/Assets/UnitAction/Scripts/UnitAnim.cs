using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnim : MonoBehaviour
{
    public void Attack()
    {
        GetComponentInParent<UnitAction>().AttackAction();
    }
}
