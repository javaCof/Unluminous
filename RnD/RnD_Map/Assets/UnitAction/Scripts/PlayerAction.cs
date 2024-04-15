using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerAction : UnitAction
{
    public float attackDist = 2f;
    public float actionDist = 2f;

    [HideInInspector] public bool controllable = true;

    private PhotonView pv;
    private Animator anim;
    private CharacterController ctl;
    private MapGenerator map;

    private Collider target;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        anim = GetComponentInChildren<Animator>();
        ctl = GetComponent<CharacterController>();
        map = GameObject.FindObjectOfType<MapGenerator>();
    }
    private void Start()
    {
        isDead = false;
        roomNum = -1;

        SetSampleData();
    }
    private void Update()
    {
        UpdateRoomNum();

        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            SetLookTarget();
            ShowLookTarget();

            if (controllable)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (PhotonNetwork.inRoom) pv.RPC("Attack", PhotonTargets.All);
                    else Attack();
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (PhotonNetwork.inRoom) pv.RPC("Action", PhotonTargets.All);
                    else Action();
                }
            }
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

    void UpdateRoomNum()
    {
        roomNum = map.FindRoom(transform.position);
    }
    void SetLookTarget()
    {
        Vector3 camPos = Camera.main.transform.position;
        Vector3 lookVec = Camera.main.transform.forward * actionDist;
        Debug.DrawRay(Camera.main.transform.position, lookVec, Color.red, 0.01f);

        RaycastHit hit;
        Ray ray = new Ray(camPos, lookVec);
        target = null;
        if (Physics.Raycast(ray, out hit, actionDist, 1 << LayerMask.NameToLayer("LookTarget")))
            target = hit.collider;
    }
    void ShowLookTarget() { }

    [PunRPC] void Attack()
    {
        anim.SetTrigger("attack");
        AttackDamage();
    }
    public void AttackDamage()      //call by anim
    {
        if (target != null && target.tag == "Enemy")
        {
            if (PhotonNetwork.inRoom)
            {
                //PhotonView tpv = target.GetComponent<PhotonView>();
                //tpv.RPC("Hit", tpv.owner, stat.ATK);

                target.GetComponent<EnemyAction>().Hit(stat.ATK);
            }
            else
            {
                target.GetComponent<EnemyAction>().Hit(stat.ATK);
            }
        }
    }
    void Action()
    {
        if (target == null) return;

        switch (target.tag)
        {
            case "Enemy":
                break;
            case "Chest":
                //target.GetComponent<Chest>().Open();
                break;
        }
    }
    [PunRPC] public void Hit(float dmg)
    {
        curHP -= dmg;

        if (curHP <= 0) Dead();
        else anim.SetTrigger("hit");
    }
    void Dead()
    {
        isDead = true;
        controllable = false;

        anim.applyRootMotion = true;
        anim.SetTrigger("dead");
        ctl.enabled = false;

        SceneManager.LoadSceneAsync("LoadingScene", LoadSceneMode.Additive);
    }
}
