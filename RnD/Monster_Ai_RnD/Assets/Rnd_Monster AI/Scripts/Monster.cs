using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : UnitCharater
{



    public enum State { Search, Alert, Trace, Attack, Repos, Hit, Dead };


    public Rect roomArea;
    UnitState state;


    class MonsterStateSearch : UnitState
    {
        public override void BeginState()
        {
            throw new System.NotImplementedException();
        }

        public override void EndState()
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateState()
        {
            //탐색범위 내에서 플레이어를 탐색
            //탐색범위 내에 플레이어가 존재할 경우, 추적상태에 진입

        }
    }

    void ChangeState(UnitState state)
    {
        if (this.state) this.state.EndState();
        this.state = state;
        this.state.BeginState();
    }

    class MonsterAlertSearch : UnitState
    {
        public override void BeginState()
        {
            throw new System.NotImplementedException();
        }

        public override void EndState()
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateState()
        {
            throw new System.NotImplementedException();
        }
    }



    private Animator anim;              //애니메이션
    private NavMeshAgent nav;           //네비게이션


    bool isDead;
    bool hasTarget;



    //플레이어 오브젝트
    private GameObject[] players;
    private GameObject player;



    private void Awake()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

    }
    // Start is called before the first frame update
    void Start()
    {
        nav.isStopped = true;
        state = State.idle;
    }

    // Update is called once per frame
    void Update()
    {
        isDead = (curHP <= 0);

        if (players.Length >0)
        {

            //타겟 셋팅
            StartCoroutine(TargerSetting());

            //모드 셋팅
            StartCoroutine(ModeSetting());

            //모드 행동
            StartCoroutine(ModeAction());
        }

        state.UpdateState();
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


    void ModeSetting()
    {



        //몬스터가 살아있을때
        if (curHP>0)
        {
            //플레이어와 몬스터 위치거리
            float distance = (transform.position - player.transform.position).sqrMagnitude;
            //Debug.Log(distance);

            //몬스터의 초기위치와 현재 위치거리
            float startDistance = (transform.position - monsterStartPosition.position).sqrMagnitude;


            //리셋 모드고 원점에서 10이상 일때(리셋 모드일때는 원점으로 갈때까지 계속 리셋 모드)
            if (state == State.resetPosition && startDistance > 5)
            {
                state = State.resetPosition;
            }
            //원점에서 200이상으로 넘어갔을떄
            else if (startDistance > 200)
            {
                state = State.resetPosition;
            }

            //원점거리 200이상으로 안넘어 갔을때
            else
            {  //플레이어가 매우 가까이 있을때
                if (distance < 3)
                {
                    state = State.attack;
                }

                //플레이어와 거리가 추적 범위안에 들어왔을때
                else if (distance < 200)
                {
                    state = State.trace;
                }

                //look 범위 안에 들어가 있을때
                else if (distance < 300)
                {
                    state = State.look;
                }

                else
                {
                    state = State.idle;
                }
            }
        }
        else
        {
            state = State.dead;
        }
    }

    IEnumerator ModeAction()
    {

        Vector3 playerLook = player.transform.position;
        playerLook.y = transform.position.y;





        //모드 변경
        if (state == State.idle)
        {
            //런 모션 멈춤
            anim.SetFloat("Run", 0);

            //몬스터 움직임 멈춤
            nav.isStopped = true;

            //idle모션 재생
            anim.Play("Idle");
        }
        else if (state == State.look)
        {
            //런 모션 멈춤
            anim.SetFloat("Run", 0);

            //몬스터 움직임 멈춤
            nav.isStopped = true;

            //몬스터 애니메이션 idle
            anim.Play("Idle");

            //몬스터가 player 바라봄
            transform.LookAt(player.transform.position);
        }
        else if (state == State.trace)
        {

            //추적할때 플레이어를 바라보게 LookAt
            transform.LookAt(playerLook);

            //run애니메이션 작동
            anim.SetFloat("Run", 1);

            //스탑을 해제
            nav.isStopped = false;

            //움직일 곳을 플레이어의 위치로 설정 
            nav.destination = player.transform.position;

            //플레이어에게서 멈출때 얼마나 거리를 벌릴건지
            nav.stoppingDistance = 0.5f;
        }
        else if (state == State.attack)
        {
            //런 모션 멈춤
            anim.SetFloat("Run", 0);

            //떄릴때 플레이어를 바라보게 LookAt
            transform.LookAt(playerLook);

            //공격할때 이동 멈춤
            nav.isStopped = true;

            //공격 애니메이션
            anim.SetTrigger("Attack");
        }
        else if (state == State.resetPosition)
        {
            //추적할때 run애니메이션 작동
            anim.SetFloat("Run", 1);
            //anim.SetFloat("Run", Mathf.Abs(myTraceAgent.velocity.x + myTraceAgent.velocity.z));

            //움직임 멈춤 취소
            nav.isStopped = false;

            //목표를 monsterStartPosition로
            nav.destination = monsterStartPosition.position;
        }
        else if (state == State.damage)
        {

            //런 모션 멈춤
            anim.SetFloat("Run", 0);

            //이동 멈춤
            nav.isStopped = true;

            //대미지 애니메이션 재생
            anim.SetTrigger("Damage");

        }
        else if (state == State.dead)
        {
            //런 모션 멈춤
            anim.SetFloat("Run", 0);

            //이동 멈춤
            nav.isStopped = true;

            //죽는 모션 재생
            anim.SetBool("Dead", true);
        }

        yield return null;
    }

    //대미지 받는 함수
    public void Damage(float playerDmg)
    { 
        //현재 체력에 받은 대미지 만큼 뺌
        curHP -= playerDmg;

        //임시
        if (curHP>0)
        {
            Debug.Log("공격받음! 남은 체력 : " + curHP);
        }

        //몬스터의 체력이 0 이하일때
        if (curHP<=0)
        {   //사망 함수 실행
            Dead();
        }
    }


    public override void Hit(UnitCharater other)
    {
        throw new System.NotImplementedException();
    }

    public override void Attack(UnitCharater target)
    {
        throw new System.NotImplementedException();
    }
}
