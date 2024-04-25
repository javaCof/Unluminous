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
    public float attackDelay = 1f;
    public bool instantAttack = false;
    public float removeDelay = 2f;

    [HideInInspector] public int id;

    private Animator anim;
    private NavMeshAgent nav;
    private MapGenerator map;
    private PhotonView pv;

    public EnemyState state;
    private Rect roomRect;

    private GameObject[] players;
    public Transform target;
    private Vector3 originPos;
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
                        if (PhotonNetwork.inRoom) pv.RPC("Attack_All", PhotonTargets.All);
                        else Attack_All();

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
                UpdateAnimMove(false);
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

    [PunRPC]
    void Attack_All()
    {
        transform.LookAt(target);
        anim.SetTrigger("attack");

        Attack_Master();
    }
    public void Attack_Master()      //call by anim
    {
        if (!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
        {
            if (Vector3.Distance(transform.position, target.position) <= maxAttackRange)
            {
                if (PhotonNetwork.inRoom)
                {
                    target.GetComponent<PhotonView>().RPC("Hit_All", PhotonTargets.All, stat.ATK);
                }
                else
                {
                    target.GetComponent<PlayerAction>().Hit_All(stat.ATK);
                }
            }
        }
    }
    [PunRPC]
    public void Hit_All(float dmg)
    {
        if (!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
        {
            curHP -= dmg;

            if (curHP <= 0)
            {
                if (PhotonNetwork.inRoom)
                {
                    pv.RPC("Dead_All", PhotonTargets.All);
                }
                else
                {
                    Dead_All();
                }

                return;
            }
        }
        anim.SetTrigger("hit");
    }
    [PunRPC]
    void Dead_All()
    {
        if (!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
        {
            isDead = true;
            enableState = false;
        }

        anim.applyRootMotion = true;
        anim.SetTrigger("dead");

        StartCoroutine(RemoveObject(removeDelay));
    }
    IEnumerator RemoveObject(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
            map.RemoveObject(gameObject, id);
        else gameObject.SetActive(false);
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

    public void OnPoolCreate()
    {
        if (PhotonNetwork.isMasterClient)
            pv.RPC("OnEnemyCreate", PhotonTargets.All);
    }
    public void OnPoolEnable()
    {
        if (PhotonNetwork.isMasterClient)
            pv.RPC("OnEnemyEnable", PhotonTargets.All);
    }
    public void OnPoolDisable()
    {
        if (PhotonNetwork.isMasterClient)
            pv.RPC("OnEnemyDisable", PhotonTargets.All);
    }

    [PunRPC] void OnEnemyCreate()
    {
        transform.parent = map.poolPos;
        gameObject.SetActive(false);
    }
    [PunRPC] void OnEnemyEnable()
    {
        transform.parent = map.objectPos;
        gameObject.SetActive(true);
    }
    [PunRPC] void OnEnemyDisable()
    {
        transform.parent = map.poolPos;
        gameObject.SetActive(false);
    }
}



//on player exit =>