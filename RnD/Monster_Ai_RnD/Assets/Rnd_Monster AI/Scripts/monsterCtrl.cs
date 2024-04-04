using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class monsterCtrl : MonoBehaviour
{
    //몬스터의 체력
    public float hp=100;

    public float attackDmg=10;

    //네비게이션
    public NavMeshAgent myTraceAgent;

    //움직일 위치
    public Vector3 MovePoint = Vector3.zero;

    //플레이어 오브젝트
    public GameObject[] players;
    public GameObject player;

    //애니메이션
    Animator anim;

    //몬스터의 초기 위치
    public Transform monsterStartPosition;


    public enum state { idle = 1, trace, attack, look, resetPosition, damage, dead };

    [Header("몬스터의 상태!")]
    public state enemyMode = state.idle;

    private void Awake()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
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
        if (players.Length >0)
        {

            //타겟 셋팅
            StartCoroutine(TargerSetting());

            //모드 셋팅
            StartCoroutine(ModeSetting());

            //모드 행동
            StartCoroutine(ModeAction());
        }

    }

    IEnumerator TargerSetting()
    {

        //플레이어와 몬스터 위치거리


        if (players.Length >= 0)
        {
            float dist = (transform.position - players[0].transform.position).sqrMagnitude;

            player = players[0];

            foreach (GameObject _player in players)
            {
                if ((transform.position - _player.transform.position).sqrMagnitude < dist)
                {
                    player = _player;
                    dist = (transform.position - _player.transform.position).sqrMagnitude;
                    //Debug.Log("가까운 거리" + dist);
                }
            }
        }


        yield return null;
    }


    IEnumerator ModeSetting()
    {
        //몬스터가 살아있을때
        if (hp>0)
        {
            //플레이어와 몬스터 위치거리
            float distance = (transform.position - player.transform.position).sqrMagnitude;
            //Debug.Log(distance);

            //몬스터의 초기위치와 현재 위치거리
            float startDistance = (transform.position - monsterStartPosition.position).sqrMagnitude;


            //리셋 모드고 원점에서 10이상 일때(리셋 모드일때는 원점으로 갈때까지 계속 리셋 모드)
            if (enemyMode == state.resetPosition && startDistance > 5)
            {
                enemyMode = state.resetPosition;
            }
            //원점에서 200이상으로 넘어갔을떄
            else if (startDistance > 200)
            {
                enemyMode = state.resetPosition;
            }

            //원점거리 200이상으로 안넘어 갔을때
            else
            {  //플레이어가 매우 가까이 있을때
                if (distance < 3)
                {
                    enemyMode = state.attack;
                }

                //플레이어와 거리가 추적 범위안에 들어왔을때
                else if (distance < 200)
                {
                    enemyMode = state.trace;
                }

                //look 범위 안에 들어가 있을때
                else if (distance < 300)
                {
                    enemyMode = state.look;
                }

                else
                {
                    enemyMode = state.idle;
                }
            }
        }
        else
        {
            enemyMode = state.dead;
        }

        yield return null;


    }

    IEnumerator ModeAction()
    {

        Vector3 playerLook = player.transform.position;
        playerLook.y = transform.position.y;





        //모드 변경
        if (enemyMode == state.idle)
        {
            //런 모션 멈춤
            anim.SetFloat("Run", 0);

            //몬스터 움직임 멈춤
            myTraceAgent.isStopped = true;

            //idle모션 재생
            anim.Play("Idle");
        }
        else if (enemyMode == state.look)
        {
            //런 모션 멈춤
            anim.SetFloat("Run", 0);

            //몬스터 움직임 멈춤
            myTraceAgent.isStopped = true;

            //몬스터 애니메이션 idle
            anim.Play("Idle");

            //몬스터가 player 바라봄
            transform.LookAt(player.transform.position);
        }
        else if (enemyMode == state.trace)
        {

            //추적할때 플레이어를 바라보게 LookAt
            transform.LookAt(playerLook);

            //run애니메이션 작동
            anim.SetFloat("Run", 1);

            //스탑을 해제
            myTraceAgent.isStopped = false;

            //움직일 곳을 플레이어의 위치로 설정 
            myTraceAgent.destination = player.transform.position;

            //플레이어에게서 멈출때 얼마나 거리를 벌릴건지
            myTraceAgent.stoppingDistance = 0.5f;
        }
        else if (enemyMode == state.attack)
        {
            //런 모션 멈춤
            anim.SetFloat("Run", 0);

            //떄릴때 플레이어를 바라보게 LookAt
            transform.LookAt(playerLook);

            //공격할때 이동 멈춤
            myTraceAgent.isStopped = true;

            //공격 애니메이션
            anim.SetTrigger("Attack");
        }
        else if (enemyMode == state.resetPosition)
        {
            //추적할때 run애니메이션 작동
            anim.SetFloat("Run", 1);
            //anim.SetFloat("Run", Mathf.Abs(myTraceAgent.velocity.x + myTraceAgent.velocity.z));

            //움직임 멈춤 취소
            myTraceAgent.isStopped = false;

            //목표를 monsterStartPosition로
            myTraceAgent.destination = monsterStartPosition.position;
        }
        else if (enemyMode == state.damage)
        {

            //런 모션 멈춤
            anim.SetFloat("Run", 0);

            //이동 멈춤
            myTraceAgent.isStopped = true;

            //대미지 애니메이션 재생
            anim.SetTrigger("Damage");

        }
        else if (enemyMode == state.dead)
        {
            //런 모션 멈춤
            anim.SetFloat("Run", 0);

            //이동 멈춤
            myTraceAgent.isStopped = true;

            //죽는 모션 재생
            anim.SetBool("Dead", true);
        }

        yield return null;
    }

    //대미지 받는 함수
    public void Damage(float playerDmg)
    { 
        //현재 체력에 받은 대미지 만큼 뺌
        hp -= playerDmg;

        //임시
        if (hp>0)
        {
            Debug.Log("공격받음! 남은 체력 : " + hp);
        }

        //몬스터의 체력이 0 이하일때
        if (hp<=0)
        {   //사망 함수 실행
            Dead();
        }
    }

    //몬스터를 죽이는 함수
    public void Dead()
    {
        //2초후 없어짐
        Destroy(gameObject, 2.0f);

        //gameObject.SetActive(false);
    }

}
