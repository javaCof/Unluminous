using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : UnitCharater
{
    public float minAttackRange = 1.5f;
    public float maxAttackRange = 2f;
    public float attackDelay = 1f;
    public bool instantAttack = false;
    public float removeDelay = 2f;

    public enum EnemyState { Search, Trace, Attack, Repos, Dead }
    public EnemyState state;

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
        InitState(EnemyState.Search);
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
    void InitState(EnemyState _state)
    {
        state = _state;
        BeginState(state);
    }

    //���� ����
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

    //���� ������
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

    //���� ����
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

    //���� ����
    void ChangeState(EnemyState _state)
    {
        EndState(state);
        state = _state;
        BeginState(state);
    }

    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
    void Attack()
    {
        //>>>target �Ÿ� �˻�
        transform.LookAt(target);
        anim.SetTrigger("attack");
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
        RemoveObject(removeDelay);

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

    //������ǥ �˻�
    void CheckTarget()
    {
        PlayerAction player = target.GetComponent<PlayerAction>();
        if (player.isDead || player.roomNum != roomNum)
            target = null;
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

    void RemoveObject(float delay)
    {
        Destroy(gameObject, delay);
    }
}