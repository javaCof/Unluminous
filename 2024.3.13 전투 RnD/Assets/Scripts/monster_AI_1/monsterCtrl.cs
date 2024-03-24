using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class monsterCtrl : MonoBehaviour
{
    public NavMeshAgent myTraceAgent;
    public Vector3 MovePoint = Vector3.zero;
    public GameObject player;
    Animator anim;


    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        myTraceAgent = GetComponent<NavMeshAgent>();
        anim = transform.Find("mon").gameObject.GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float distance = (transform.position - player.transform.position).sqrMagnitude;
        Debug.Log(distance);

        if (Input.GetKeyDown("d"))
        {
            Vector3 temp = transform.position;
            temp.x += 0.5f;
            transform.position = temp;
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            MovePoint = player.transform.position;
            myTraceAgent.destination = MovePoint;

            myTraceAgent.stoppingDistance = 2.0f;
        }

        if (distance < 6.0f)
        {
            anim.SetTrigger("Attack");
        }
    }

    //private void OnTriggerEnter(Collider col)
    //{
    //    if (col.gameObject.tag == "Player")
    //    {
    //        Debug.Log("플레이어가 앞에 있음!");
    //        anim.SetTrigger("Attack");

    //    }
    //}
}
