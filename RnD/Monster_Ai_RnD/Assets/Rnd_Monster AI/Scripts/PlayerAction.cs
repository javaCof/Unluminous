using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : UnitCharater
{


    public enum PlayerState { Idle, Move, Jump, Attack } //���
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






    //����ĳ��Ʈ�� ���� ���͸� ������� ���� 
    public Monster mon = null;
    //�÷��̾� ü��

    

    Ray ray;

    public RaycastHit hitInfo;

    //����ĳ��Ʈ �Ÿ�
    public float maxRayDist = 30.0f;


    // Update is called once per frame
    void rUpdate()
    {
        //�÷��̾� ray ����
        ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        //ray �������� �����
        Debug.DrawRay(ray.origin, ray.direction * maxRayDist, Color.red);

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            sAttack();
        }
    }

    public void sAttack()
    {       //�� ray�� �������� �±װ� Enemy �϶�
        if (Physics.Raycast(ray, out hitInfo, maxRayDist) && hitInfo.transform.tag == "Enemy")
        {
            //mon ������ ���� ray�� ���� monster�� ����
            mon = hitInfo.transform.GetComponent<Monster>();

            Debug.Log("����!");

            //���� monster�� ����� �Լ� ����
            mon.Hit(this);
        }
    }

    public override void Hit(UnitCharater other)
    {
        curHP -= other.stat.ATK;
    }
}
