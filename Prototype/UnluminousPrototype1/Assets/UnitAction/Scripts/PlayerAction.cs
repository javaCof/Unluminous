using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : UnitCharater
{
    public float attackDist = 2f;
    public float actionDist = 2f;

    private PhotonView pv;
    private Animator anim;
    private Collider target;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        anim = GetComponentInChildren<Animator>();
    }
    private void Start()
    {
        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            isDead = false;
            roomNum = -1;
            SetSampleData();
        }
    }
    private void Update()
    {
        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            UpdateRoomNum();

            SetLookTarget();
            ShowLookTarget();

            if (Input.GetMouseButtonDown(0)) Attack();
            if (Input.GetKeyDown(KeyCode.E)) Action();
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
    void UpdateRoomNum()
    {
        roomNum = GameObject.FindObjectOfType<MapGenerator>().FindRoom(transform.position);
    }

    void Attack()
    {
        anim.SetTrigger("attack");

        if (target != null && target.tag == "Enemy")
        {
            target.GetComponent<EnemyAction>().Hit(this);
        }
    }
    void Action()
    {
        if (target == null) return;

        switch (target.tag)
        {
            case "Enemy":
                Debug.Log(target.gameObject.name);
                break;
            case "Chest":
                //target.GetComponent<Chest>().Open();
                break;
        }
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

        anim.applyRootMotion = true;
        anim.SetTrigger("dead");

        //send to map
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {


    }
}
