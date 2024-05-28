using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookUI : MonoBehaviour
{
    private void Update()
    {
        Player target = GameManager.Instance.player;
        if (target != null)
        {
            Vector3 pos = target.transform.position;
            pos.y = transform.position.y;
            transform.LookAt(pos);
        }
    }
}
