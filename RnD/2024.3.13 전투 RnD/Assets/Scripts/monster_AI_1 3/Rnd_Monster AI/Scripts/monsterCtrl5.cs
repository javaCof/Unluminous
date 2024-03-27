using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class monsterCtrl5 : MonoBehaviour
{   //네비게이션
    public NavMeshAgent myTraceAgent;

    //움직일 위치
    public Vector3 MovePoint = Vector3.zero;

    //플레이어 오브젝트
    public GameObject player;

    //애니메이션
    Animator anim;


    //몬스터의 초기 위치
    Vector3 monsterStartPosition;






    public enum state { idle = 1, trace, attack, look, resetPosition };

    [Header("몬스터의 상태!")]
    public state enemyMode = state.idle;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        myTraceAgent = GetComponent<NavMeshAgent>();
        anim = transform.Find("mon").gameObject.GetComponent<Animator>();
        monsterStartPosition = transform.position;

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
        {   //플레이어와 몬스터 위치거리
            float distance = (transform.position - player.transform.position).sqrMagnitude;
            //몬스터의 초기위치와 현재 위치거리
            float startDistance = (transform.position - monsterStartPosition).sqrMagnitude;

            //Debug.Log(startDistance);

            //모드 셋팅

            if (distance < 5.0f)
            {   //공격시작 거리
                enemyMode = state.attack;
            }
            
            else if (startDistance > 200f)
            {   //초기위치에서 일정거리 이상 떨어졌을때 
                enemyMode = state.resetPosition;
            }
            else if (startDistance < 200f)
            {
                if (enemyMode == state.resetPosition && startDistance < 10f)
                {
                    Debug.Log("초기위치 이동후 idle");
                    enemyMode = state.idle;
                }
                //시작거리에서 가까울때
                else if (enemyMode != state.resetPosition && startDistance < 10f)
                {   //플레이어와 거리가 200이하로 가까울때
                    if (distance < 200f)
                    {   //추적시작
                        enemyMode = state.trace;
                    }
                }
            }
            else if (distance < 300f)
            {   //player한테 반응 시작
                enemyMode = state.look;
            }
            else if (distance > 300f)
            {   //반응거리보다 멀때
                enemyMode = state.idle;
            }









            //모드 변경
            if (enemyMode == state.idle)
            {
                anim.Play("Idle");
                myTraceAgent.isStopped = true;
            }
            else if (enemyMode == state.look)
            {   //몬스터 움직임 멈춤
                myTraceAgent.isStopped = true;

                //몬스터 애니메이션 idle
                anim.Play("Idle");

                //몬스터가 player 바라봄
                transform.LookAt(player.transform.position);
            }
            else if (enemyMode == state.trace)
            {
                //추적할때 플레이어를 바라보게 LookAt
                transform.LookAt(player.transform.position);

                //추적할때 run애니메이션 작동
                anim.SetFloat("Run", Mathf.Abs(myTraceAgent.velocity.x + myTraceAgent.velocity.z));

                //스탑을 해제
                myTraceAgent.isStopped = false;

                //움직일 곳을 플레이어의 위치로 설정 
                myTraceAgent.destination = player.transform.position;

                //플레이어에게서 멈출때 얼마나 거리를 벌릴건지
                myTraceAgent.stoppingDistance = 2.5f;
            }
            else if (enemyMode == state.attack)
            {
                //떄릴때 플레이어를 바라보게 LookAt
                transform.LookAt(player.transform.position);

                //공격할때 이동 멈춤
                myTraceAgent.isStopped = true;

                //공격 애니메이션
                anim.SetTrigger("Attack");
            }
            else if (enemyMode == state.resetPosition)
            {
                //추적할때 run애니메이션 작동
                anim.SetFloat("Run", Mathf.Abs(myTraceAgent.velocity.x + myTraceAgent.velocity.z));

                //움직임 멈춤 취소
                myTraceAgent.isStopped = false;

                //목표를 monsterStartPosition로
                myTraceAgent.destination = monsterStartPosition;



            }
        }






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
