using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class monsterCtrl3 : MonoBehaviour
{
    public NavMeshAgent myTraceAgent;
    public Vector3 MovePoint = Vector3.zero;
    public GameObject player;
    Animator anim;
    
    


    public enum state { idle = 1, trace, attack };

    [Header("몬스터의 상태!")]
    public state enemyMode = state.idle;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        myTraceAgent = GetComponent<NavMeshAgent>();
        anim = transform.Find("mon").gameObject.GetComponent<Animator>();
       
       
    }
    // Start is called before the first frame update
    void Start()
    {
        myTraceAgent.isStopped = true;

        enemyMode = state.idle;
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {   //모드 셋팅
            float distance = (transform.position - player.transform.position).sqrMagnitude;
            Debug.Log(distance);


            //공격시작 거리
            if (distance < 5.0f)
            {
                enemyMode = state.attack;
            }
            //추적시작 범위
            else if (distance<200)
            {//몬스터 상태가 trace로 변경
                enemyMode = state.trace;
            }
            else if (distance>200)
            {
                enemyMode = state.idle;
            }
            


            //모드 변경
            if (enemyMode == state.idle)
            {
                anim.Play("Idle");
                myTraceAgent.isStopped = true;
            }
            else if (enemyMode == state.trace)
            {   
                //추적할때 플레이어를 바라보게 LookAt
                transform.LookAt(player.transform.position);
                //gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, player.transform.position, 1 * Time.deltaTime);
                anim.SetFloat("Run", Mathf.Abs(myTraceAgent.velocity.x+ myTraceAgent.velocity.z));
                //anim.Play("Run");//원래는 달리는 모션
                //스탑을 해제
                myTraceAgent.isStopped = false;
                //움직일 곳을 플레이어의 위치로 설정 
                Debug.Log("추적");
                myTraceAgent.destination = player.transform.position;
                //플레이어에게서 멈출때 얼마나 거리를 벌릴건지
                myTraceAgent.stoppingDistance = 2.5f;
            }
            else if (enemyMode == state.attack)
            {
                //떄릴때 플레이어를 바라보게 LookAt
                //transform.FindChild("mon").transform.LookAt(player.transform);
                //gameObject.transform.LookAt(player.transform);
                myTraceAgent.isStopped = true;
                anim.SetTrigger("Attack");
            }
        }



        //if (Input.GetKeyDown("d"))
        //{
        //    Vector3 temp = transform.position;
        //    temp.x += 0.5f;
        //    transform.position = temp;
        //}


        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    enemyMode = state.move;

        //    //MovePoint = player.transform.position;
        //    //myTraceAgent.destination = MovePoint;

        //    //myTraceAgent.stoppingDistance = 2.0f;
        //}


    }


    IEnumerator ModeSetting()
    {//스페이스바를 누르면

        yield return null;
    }

    IEnumerator ModeAction()
    {

        //else if (enemyMode==state.idle)
        //{
        //    myTraceAgent.isStopped = true;
        //    anim.Play("Idle");
        //}

        yield return null;
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
