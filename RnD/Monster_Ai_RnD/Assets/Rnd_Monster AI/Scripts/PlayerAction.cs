using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : UnitCharater
{


    public enum PlayerState { Idle, Move, Jump, Attack } //폐기
    public PlayerState state;



    //player look


    public float attackDist = 2f;
    public float actionDist = 2f;

    private Collider target;


    private Animator anim;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        LookTarget();

        if (Input.GetMouseButtonDown(0)) Attack();
        if (Input.GetKeyDown(KeyCode.E)) Action();
    }

    void LookTarget()
    {
        Vector3 camPos = Camera.main.transform.position;
        Vector3 lookVec = Camera.main.transform.forward * actionDist;
        Debug.DrawRay(Camera.main.transform.position, lookVec, Color.red, 0.01f);

        RaycastHit hit;
        Ray ray = new Ray(camPos, lookVec);
        target = null;
        if (Physics.Raycast(ray, out hit, actionDist, LayerMask.NameToLayer("LookTarget")))
            target = hit.collider;
        

    }

    void Attack()
    {
        anim.SetTrigger("attack");
    }

    void Action()
    {
        if (target == null) return;

        switch (target.tag)
        {
            
        }
    }












    public enum LookTargetType { NONE=-1, MONSTER, CHEST, TRADER, PORTAL }
    LookTargetType lookType;
    GameObject lookObj;

    void LookAction()
    {
        switch (lookType)
        {
            case LookTargetType.MONSTER:
                if (Input.GetMouseButton(0))
                {
                    //monster attack
                    //lookObj.Hit()
                }
                break;
            case LookTargetType.CHEST:
                //open chest
                break;
            case LookTargetType.TRADER:
                //talk trader
                break;
            case LookTargetType.PORTAL:
                //move next level
                break;
        }
    }






    //레이캐스트를 맞은 몬스터를 담기위한 변수 
    public Monster mon = null;
    //플레이어 체력

    

    Ray ray;

    public RaycastHit hitInfo;

    //레이캐스트 거리
    public float maxRayDist = 30.0f;


    // Update is called once per frame
    void rUpdate()
    {
        //플레이어 ray 설정
        ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        //ray 보기위한 디버그
        Debug.DrawRay(ray.origin, ray.direction * maxRayDist, Color.red);

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            sAttack();
        }
    }

    public void sAttack()
    {       //쏜 ray에 맞은것의 태그가 Enemy 일때
        if (Physics.Raycast(ray, out hitInfo, maxRayDist) && hitInfo.transform.tag == "Enemy")
        {
            //mon 변수에 지금 ray에 닿은 monster를 참조
            mon = hitInfo.transform.GetComponent<Monster>();

            Debug.Log("공격!");

            //닿은 monster의 대미지 함수 실행
            mon.Hit(this);
        }
    }

    public override void Hit(UnitCharater other)
    {
        curHP -= other.stat.ATK;
    }
}
