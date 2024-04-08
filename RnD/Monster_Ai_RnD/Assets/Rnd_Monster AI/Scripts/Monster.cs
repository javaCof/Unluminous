using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : UnitCharater
{
    public float minAttackRange = 1.5f;
    public float maxAttackRange = 2f;
    public float attackDelay = 1f;

    public enum MonsterState { Search, Trace, Attack, Repos }
    public MonsterState state;

    [HideInInspector] public Rect roomRect;

    private Animator anim;
    private NavMeshAgent nav;

    private GameObject[] players;
    private Transform target;
    private Vector3 originPos;
    private float attackDtime;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        nav = GetComponent<NavMeshAgent>();
        players = GameObject.FindGameObjectsWithTag("Player");
    }
    void Start()
    {
        isDead = false;
        SetSampleData();

        originPos = transform.position;
        InitState(MonsterState.Search);
    }
    void Update()
    {
        UpdateState(state);
    }

    //�ӽ� ������ ����
    void SetSampleData()
    {
        stat = new UnitStatInfo();
        stat.HP = 30;
        stat.ATK = 5;
        stat.DEF = 5;
        stat.SPD = 1;
        curHP = stat.HP;
        SetRoom(0, new Rect(transform.position.x - 100, transform.position.y - 100, 200, 200));
    }

    //�� ���� ����
    public void SetRoom(int id, Rect rect)
    {
        roomNum = id;
        roomRect = rect;
    }

    //���� �ʱ�ȭ
    void InitState(MonsterState _state)
    {
        state = _state;
        BeginState(state);
    }

    //���� ����
    void BeginState(MonsterState _state)
    {
        switch (_state)
        {
            case MonsterState.Search:
                nav.isStopped = true;
                break;
            case MonsterState.Trace:
                anim.SetBool("move", true);
                break;
            case MonsterState.Attack:
                nav.isStopped = true;
                attackDtime = Time.time;
                break;
            case MonsterState.Repos:
                anim.SetBool("move", true);
                SetNav(originPos, 0.1f);
                break;
        }
    }

    //���� ������
    void UpdateState(MonsterState _state)
    {
        switch (_state)
        {
            case MonsterState.Search:
                {
                    SetTarget();
                    if (target != null) ChangeState(MonsterState.Trace);
                }
                break;
            case MonsterState.Trace:
                {
                    SetNav(target.position, minAttackRange);
                    if (!InRoom()) ChangeState(MonsterState.Repos);
                    if (IsNavEnd()) ChangeState(MonsterState.Attack);
                }
                break;
            case MonsterState.Attack:
                {
                    /*attack routine*/
                    if (Time.time - attackDtime > attackDelay)
                    {
                        Attack();
                        attackDtime = Time.time;
                    }

                    if (OutRange(target.position, maxAttackRange)) ChangeState(MonsterState.Trace);
                }
                break;
            case MonsterState.Repos:
                {
                    if (IsNavEnd()) ChangeState(MonsterState.Search);
                }
                break;
        }
    }

    //���� ����
    void EndState(MonsterState _state)
    {
        switch (_state)
        {
            case MonsterState.Search:
                break;
            case MonsterState.Trace:
            case MonsterState.Repos:
                anim.SetBool("move", false);
                break;
            case MonsterState.Attack:
                break;
        }
    }

    //���� ����
    void ChangeState(MonsterState _state)
    {
        EndState(state);
        state = _state;
        BeginState(state);
    }

    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
    void Attack()
    {
        anim.SetTrigger("attack");
        //player.Hit(this)
    }
    public override void Hit(UnitCharater other)
    {
        curHP -= other.stat.ATK;
        //hit animation

        if (curHP <= 0) Dead();
    }
    void Dead()
    {
        isDead = true;
        //dead animation
        //send to map
    }
    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    //������ǥ ����
    void SetTarget()
    {
        target = null;
        float minDist = -1;
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

    //�׺���̼� ���� (������, �����Ÿ�)
    void SetNav(Vector3 dest, float stopDist)
    {
        nav.destination = dest;
        nav.stoppingDistance = stopDist;
        nav.isStopped = false;
    }

    //�� ���ο� �ִ��� �˻�
    bool InRoom()
    {
        Vector2 pos = new Vector2(transform.position.x, transform.position.z);
        return pos.x > roomRect.xMin && pos.x < roomRect.xMax && pos.y > roomRect.yMin && pos.y < roomRect.yMax;
    }

    //����� Ư�� ���� ���� �ִ��� �˻�
    bool InRange(Vector3 target, float range) => (target - transform.position).sqrMagnitude < range * range;

    //����� Ư�� ������ ������� �˻�
    bool OutRange(Vector3 target, float range) => !InRange(target, range);

    //���� �׺���̼� ���� Ȯ��
    bool IsNavEnd() => InRange(nav.destination, nav.stoppingDistance + 0.1f);
}
