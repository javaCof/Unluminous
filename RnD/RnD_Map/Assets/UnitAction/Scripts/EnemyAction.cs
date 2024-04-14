using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAction : UnitAction
{
    public enum EnemyState { Search, Trace, Attack, Repos, Dead }

    public bool enableState = true;
    public float minAttackRange = 1.5f;
    public float maxAttackRange = 2f;
    public float attackDelay = 1f;
    public bool instantAttack = false;
    public float removeDelay = 2f;

    private Animator anim;
    private NavMeshAgent nav;
    private PhotonView pv;
    private MapGenerator map;

    private EnemyState state;
    private Rect roomRect;

    private GameObject[] players;
    private Transform target;
    private Vector3 originPos;
    private float attackDtime;

    private bool animMove;
    private Vector3 curPos;
    private Quaternion curRot;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        nav = GetComponent<NavMeshAgent>();
        pv = GetComponent<PhotonView>();
        map = GameObject.FindObjectOfType<MapGenerator>();
    }
    void Start()
    {
        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            isDead = false;
            originPos = transform.position;

            SetSampleData();
            InitState(EnemyState.Search);
        }
        else
        {
            Destroy(nav);
        }
    }
    void Update()
    {
        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            if (enableState)
            {
                UpdateState(state);
            }
            else if (state != EnemyState.Search)
            {
                ChangeState(EnemyState.Search);
            }
        }
        else
        {
            UpdatePos();
            UpdateAnimMove(animMove);
        }
    }

    //�ӽ� ������ ����
    void SetSampleData()
    {
        stat = new UnitStatInfo();
        stat.HP = 30;
        stat.ATK = 1;
        stat.DEF = 5;
        stat.SPD = 1;
        curHP = stat.HP;
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
                UpdateAnimMove(true);
                break;
            case EnemyState.Attack:
                nav.isStopped = true;
                attackDtime = instantAttack ? Time.time - attackDelay : Time.time;
                break;
            case EnemyState.Repos:
                UpdateAnimMove(true);
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
                        if (PhotonNetwork.inRoom)
                            pv.RPC("Attack", PhotonTargets.All);
                        else Attack();

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
                UpdateAnimMove(false);
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

    //������ǥ ����
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

    //������ǥ �˻�
    void CheckTarget()
    {
        if (target == null) return;
        PlayerAction player = target.GetComponent<PlayerAction>();
        if (player.isDead || player.roomNum != roomNum)
        {
            target = null;
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

    [PunRPC] void Attack()
    {
        transform.LookAt(target);
        anim.SetTrigger("attack");

        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            if (Vector3.Distance(transform.position, target.position) <= maxAttackRange)
                target.GetComponent<PlayerAction>().Hit(this);
        }
    }
    public override void Hit(UnitAction other)
    {
        curHP -= other.stat.ATK;

        if (curHP <= 0) Dead();
        else anim.SetTrigger("hit");
    }
    void Dead()
    {
        isDead = true;
        anim.applyRootMotion = true;
        anim.SetTrigger("dead");
        StartCoroutine(RemoveObject(removeDelay));
    }

    IEnumerator RemoveObject(float delay)
    {
        yield return new WaitForSeconds(delay);
        map.RemoveObject(gameObject);
    }

    void UpdatePos()
    {
        transform.position = Vector3.Lerp(transform.position, curPos, 3.0f * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, curRot, 3.0f * Time.deltaTime);
    }
    void UpdateAnimMove(bool isMove)
    {
        animMove = isMove;
        anim.SetBool("move", animMove);
    }
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(gameObject.GetActive());
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(animMove);
        }
        else
        {
            gameObject.SetActive((bool)stream.ReceiveNext());
            curPos = (Vector3)stream.ReceiveNext();
            curRot = (Quaternion)stream.ReceiveNext();
            animMove = (bool)stream.ReceiveNext();
        }
    }
}
