using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAction : UnitCharater
{
    public float minAttackRange = 1.5f;
    public float maxAttackRange = 2f;
    public float attackDelay = 1f;
    public bool instantAttack = false;
    public float removeDelay = 2f;

    public enum EnemyState { Search, Trace, Attack, Repos, Dead }
    [HideInInspector] public EnemyState state;

    [HideInInspector] public Rect roomRect;

    private Animator anim;
    private NavMeshAgent nav;

    private GameObject[] players;
    private Transform target;
    private Vector3 originPos;
    private float attackDtime;

    PhotonView pv;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        nav = GetComponent<NavMeshAgent>();
        pv = GetComponent<PhotonView>();
    }
    void Start()
    {
        if (pv.isMine)
        {
            isDead = false;
            SetSampleData();

            originPos = transform.position;
            InitState(EnemyState.Search);
        }
        else
        {
            Destroy(nav);
        }
    }
    void Update()
    {
        if (pv.isMine)
        {
            UpdateState(state);
        }
    }

    //임시 데이터 설정
    void SetSampleData()
    {
        stat = new UnitStatInfo();
        stat.HP = 30;
        stat.ATK = 5;
        stat.DEF = 5;
        stat.SPD = 1;
        curHP = stat.HP;
    }

    //방 정보 설정
    public void SetRoom(int id, Rect rect)
    {
        roomNum = id;
        roomRect = rect;
    }

    //상태 초기화
    void InitState(EnemyState _state)
    {
        state = _state;
        BeginState(state);
    }

    //상태 시작
    void BeginState(EnemyState _state)
    {
        switch (_state)
        {
            case EnemyState.Search:
                nav.isStopped = true;
                break;
            case EnemyState.Trace:
                anim.SetBool("move", true);
                break;
            case EnemyState.Attack:
                nav.isStopped = true;
                attackDtime = instantAttack ? Time.time - attackDelay : Time.time;
                break;
            case EnemyState.Repos:
                anim.SetBool("move", true);
                SetNav(originPos, 0.1f);
                break;
        }
    }

    //상태 진행중
    void UpdateState(EnemyState _state)
    {
        switch (_state)
        {
            case EnemyState.Search:
                {
                    SetTarget();
                    if (target != null) ChangeState(EnemyState.Trace);
                }
                break;
            case EnemyState.Trace:
                {
                    CheckTarget();
                    if (target == null)
                    {
                        ChangeState(EnemyState.Repos);
                        break;
                    }

                    SetNav(target.position, minAttackRange);
                    if (!InRoom()) ChangeState(EnemyState.Repos);
                    if (IsNavEnd()) ChangeState(EnemyState.Attack);
                }
                break;
            case EnemyState.Attack:
                {
                    CheckTarget();
                    if (target == null)
                    {
                        ChangeState(EnemyState.Repos);
                        break;
                    }

                    /*attack routine*/
                    if (Time.time - attackDtime > attackDelay)
                    {
                        Attack();
                        attackDtime = Time.time;
                    }

                    if (OutRange(target.position, maxAttackRange)) ChangeState(EnemyState.Trace);
                }
                break;
            case EnemyState.Repos:
                {
                    if (IsNavEnd()) ChangeState(EnemyState.Search);
                }
                break;
        }
    }

    //상태 종료
    void EndState(EnemyState _state)
    {
        switch (_state)
        {
            case EnemyState.Search:
                break;
            case EnemyState.Trace:
            case EnemyState.Repos:
                anim.SetBool("move", false);
                break;
            case EnemyState.Attack:
                break;
        }
    }

    //상태 변경
    void ChangeState(EnemyState _state)
    {
        EndState(state);
        state = _state;
        BeginState(state);
    }

    void Attack()
    {
        transform.LookAt(target);
        anim.SetTrigger("attack");

        if (Vector3.Distance(transform.position, target.position) <= maxAttackRange)
            target.GetComponent<PlayerAction>().Hit(this);
    }
    public override void Hit(UnitCharater other)
    {
        curHP -= other.stat.ATK;

        if (curHP <= 0) Dead();
        else anim.SetTrigger("hit");
    }
    void Dead()
    {
        isDead = true;
        ChangeState(EnemyState.Dead);

        anim.applyRootMotion = true;
        anim.SetTrigger("dead");
        StartCoroutine(RemoveObject(removeDelay));
    }

    //추적목표 설정
    void SetTarget()
    {
        target = null;
        float minDist = -1;

        players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var obj in players)
        {
            PlayerAction player = obj.GetComponent<PlayerAction>();

            if (!player.isDead && player.roomNum == this.roomNum)
            {
                float dist = (player.transform.position - transform.position).sqrMagnitude;
                if (minDist == -1 || dist < minDist)
                {
                    target = player.transform;
                    minDist = dist;
                }
            }
        }
    }

    //추적목표 검사
    void CheckTarget()
    {
        if (target == null) return;
        PlayerAction player = target.GetComponent<PlayerAction>();
        if (player.isDead || player.roomNum != roomNum)
        {
            Debug.Log(player.isDead);
            Debug.Log("" + player.roomNum + roomNum);

            target = null;
        }
    }

    //네비게이션 설정 (목적지, 정지거리)
    void SetNav(Vector3 dest, float stopDist)
    {
        nav.destination = dest;
        nav.stoppingDistance = stopDist;
        nav.isStopped = false;
    }

    //방 내부에 있는지 검사
    bool InRoom()
    {
        Vector2 pos = new Vector2(transform.position.x, transform.position.z);
        return pos.x > roomRect.xMin && pos.x < roomRect.xMax && pos.y > roomRect.yMin && pos.y < roomRect.yMax;
    }

    //대상이 특정 범위 내에 있는지 검사
    bool InRange(Vector3 target, float range) => (target - transform.position).sqrMagnitude < range * range;

    //대상이 특정 범위를 벗어났는지 검사
    bool OutRange(Vector3 target, float range) => !InRange(target, range);

    //현재 네비게이션 종료 확인
    bool IsNavEnd() => InRange(nav.destination, nav.stoppingDistance + 0.1f);

    IEnumerator RemoveObject(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
        
    }
}
