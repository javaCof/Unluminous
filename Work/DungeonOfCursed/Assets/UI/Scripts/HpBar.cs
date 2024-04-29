using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBar : MonoBehaviour
{
    Transform mytr;



    private void Awake()
    {
        mytr = GetComponent<Transform>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //hpBar.transform.position= Camera.main.WorldToScreenPoint(transform.position);

        mytr.LookAt(Camera.main.transform);
    }
}
