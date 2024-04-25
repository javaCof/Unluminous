using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAction : UnitAction, IPhotonPoolObject
{
    public enum EnemyState { Search, Trace, Attack, Repos, Dead }

    public bool enableState = true;
    public float minAttackRange = 1.5f;
    public float maxAttackRange = 2f;
    public float traceRange = 5f;
    public float attackDelay = 1f;
    public bool instantAttack = false;
    public float removeDelay = 2f;

    [HideInInspector] public int id;
    [HideInInspector] public Vector3 originPos;

    private Animator anim;
    private NavMeshAgent nav;
    private MapGenerator map;
    private PhotonView pv;

    private EnemyState state;
    private Rect roomRect;

    private Transform traceTarget;
    private Transform attackTarget;

    private float attackDtime;

    private bool animMove;
    private Vector3 curPos;
    private Quaternion curRot;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        nav = GetComponent<NavMeshAgent>();
        map = FindObjectOfType<MapGenerator>();
        pv = GetComponent<PhotonView>();
    }
    void Start()
    {
        if (!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
        {
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
        if (!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
        {
            if (enableState)
            {
                UpdateState(state);
            }
        }
        else
        {
            UpdatePos();
            UpdateAnimMove(animMove);
        }
    }

    //임시 데이터 설정
    void SetSampleData()
    {
        stat = new UnitStatInfo();
        stat.HP = 30;
        stat.ATK = 1;
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


    public void Init()
    {
        
    }

    public void Reset()
    {
        curHP = stat.HP;
        isDead = false;
        enableState = true;
        anim.applyRootMotion = false;
    }

    public void SetStat(UnitStatInfo stat)
    {
        
    }





    void InitState(EnemyState _state)
    {
        state = _state;
        BeginState(state);
    }
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
    void UpdateState(EnemyState _state)
    {
        switch (_state)
        {
            case EnemyState.Search:
                {
                    SetTraceTarget();

                    if (traceTarget != null) ChangeState(EnemyState.Trace);
                }
                break;
            case EnemyState.Trace:
                {
                    CheckTraceTarget();

                    if (traceTarget == null)
                    {
                        ChangeState(EnemyState.Repos);
                        break;
                    }

                    SetNav(traceTarget.position, minAttackRange);

                    if (!InRoom()) ChangeState(EnemyState.Repos);
                    if (IsNavEnd()) ChangeState(EnemyState.Attack);
                }
                break;
            case EnemyState.Attack:
                {
                    CheckTraceTarget();

                    if (traceTarget == null)
                    {
                        ChangeState(EnemyState.Repos);
                        break;
                    }

                    /*attack routine*/
                    if (Time.time - attackDtime > attackDelay)
                    {
                        transform.LookAt(traceTarget);

                        if (PhotonNetwork.inRoom) pv.RPC("Attack_All", PhotonTargets.All);
                        else Attack_All();

                        attackDtime = Time.time;
                    }

                    if (OutRange(traceTarget.position, maxAttackRange)) ChangeState(EnemyState.Trace);
                }
                break;
            case EnemyState.Repos:
                {
                    CheckTraceTarget();

                    if (traceTarget != null)
                    {
                        ChangeState(EnemyState.Trace);
                        break;
                    }

                    if (IsNavEnd()) ChangeState(EnemyState.Search);
                }
                break;
        }
    }
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
    void ChangeState(EnemyState _state)
    {
        EndState(state);
        state = _state;
        BeginState(state);
    }

    void SetTraceTarget()
    {
        traceTarget = null;

        float minDist = -1;
        foreach (var obj in GameObject.FindGameObjectsWithTag("Player"))
        {
            PlayerAction player = obj.GetComponent<PlayerAction>();

            if (!player.isDead && player.roomNum == this.roomNum)
            {
                float dist = (player.transform.position - transform.position).sqrMagnitude;
                if (minDist == -1 || dist < minDist)
                {
                    traceTarget = player.transform;
                    minDist = dist;
                }
            }
        }

        if (traceTarget != null && Vector3.Distance(traceTarget.position, transform.position) > traceRange)
            traceTarget = null;
    }
    void CheckTraceTarget()
    {
        if (traceTarget == null) return;

        PlayerAction player = traceTarget.GetComponent<PlayerAction>();
        if (player.isDead || player.roomNum != roomNum || Vector3.Distance(traceTarget.position, transform.position) > traceRange)
            traceTarget = null;
    }

    void SetNav(Vector3 dest, float stopDist)
    {
        nav.destination = dest;
        nav.stoppingDistance = stopDist;
        nav.isStopped = false;
    }
    bool InRoom()
    {
        Vector2 pos = new Vector2(transform.position.x, transform.position.z);
        return pos.x > roomRect.xMin && pos.x < roomRect.xMax && pos.y > roomRect.yMin && pos.y < roomRect.yMax;
    }
    bool InRange(Vector3 target, float range) => (target - transform.position).sqrMagnitude < range * range;
    bool OutRange(Vector3 target, float range) => !InRange(target, range);
    bool IsNavEnd() => InRange(nav.destination, nav.stoppingDistance + 0.1f);

    [PunRPC] void Attack_All()
    {
        anim.SetTrigger("attack");
    }
    public override void AttackAction()
    {
        if (!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
        {
            CheckAttackTarget();

            if (attackTarget != null)
            {
                if (PhotonNetwork.inRoom)
                {
                    attackTarget.GetComponent<PhotonView>().RPC("Hit_Owner", PhotonTargets.All, stat.ATK);
                }
                else
                {
                    attackTarget.GetComponent<PlayerAction>().Hit_Owner(stat.ATK);
                }
            }
        }
    }
    void CheckAttackTarget()
    {
        attackTarget = null;

        float minDist = -1;
        foreach (var obj in GameObject.FindGameObjectsWithTag("Player"))
        {
            PlayerAction player = obj.GetComponent<PlayerAction>();

            if (!player.isDead && player.roomNum == this.roomNum)
            {
                float dist = (player.transform.position - transform.position).sqrMagnitude;
                if (minDist == -1 || dist < minDist)
                {
                    attackTarget = player.transform;
                    minDist = dist;
                }
            }
        }

        if (attackTarget != null && Vector3.Distance(attackTarget.position, transform.position) > maxAttackRange)
            attackTarget = null;
    }

    [PunRPC] public void Hit_Master(float dmg)
    {
        if (isDead) return;

        curHP -= dmg;

        if (curHP <= 0)
        {
            if (PhotonNetwork.inRoom)
                pv.RPC("Dead_All", PhotonTargets.All);
            else Dead_All();

            isDead = true;
            enableState = false;

            StartCoroutine(RemoveObject(removeDelay));
        }
        else
        {
            if (PhotonNetwork.inRoom)
                pv.RPC("Hit_All", PhotonTargets.All);
            else Hit_All();
        }
    }
    [PunRPC] void Hit_All()
    {
        anim.SetTrigger("hit");
    }
    [PunRPC] void Dead_All()
    {
        anim.applyRootMotion = true;
        anim.SetTrigger("dead");
    }
    IEnumerator RemoveObject(float delay)
    {
        yield return new WaitForSeconds(delay);

        map.RemoveObject(gameObject, id);
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
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(animMove);
        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
            curRot = (Quaternion)stream.ReceiveNext();
            animMove = (bool)stream.ReceiveNext();
        }
    }

    [PunRPC] public void OnPoolCreate()
    {
        if (PhotonNetwork.isMasterClient)
            pv.RPC("OnPoolCreate", PhotonTargets.Others);

        transform.parent = map.poolPos;
        gameObject.SetActive(false);
    }
    [PunRPC] public void OnPoolEnable()
    {
        if (PhotonNetwork.isMasterClient)
            pv.RPC("OnPoolEnable", PhotonTargets.Others);

        transform.parent = map.objectPos;
        gameObject.SetActive(true);
    }
    [PunRPC] public void OnPoolDisable()
    {
        if (PhotonNetwork.isMasterClient)
            pv.RPC("OnPoolDisable", PhotonTargets.Others);

        transform.parent = map.poolPos;
        gameObject.SetActive(false);
    }
}



//on player exit =>