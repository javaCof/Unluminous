using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBar : MonoBehaviour
{
    public Transform mytr;

    public Transform healthTr;

    public Enemy enemyact;

    public float hp;

    private void Awake()
    {
        mytr = GetComponent<Transform>();
        healthTr = transform.GetChild(1).GetComponent<Transform>();
        enemyact = transform.parent.GetComponent<Enemy>();
        hp = enemyact.curHP;
    }

    // Update is called once per frame
    void Update()
    {
        //hpBar.transform.position= Camera.main.WorldToScreenPoint(transform.position);

        mytr.LookAt(Camera.main.transform);

        //юс╫ц
        hp = enemyact.curHP;
        healthTr.transform.localScale = new Vector3(hp/100, 1, 1);
    }
}
