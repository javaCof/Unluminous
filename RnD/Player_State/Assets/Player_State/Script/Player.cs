using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //�÷��̾� ��ũ��Ʈ
    Player player;

    //ĳ���� ��Ʈ�ѷ�
    CharacterController playerCon;

    //�÷��̾� �ִϸ�����
    public Animator anim;

    //����ĳ��Ʈ�� ���� ���͸� ������� ���� 
    //public monsterCtrl mon = null;

    //�÷��̾� ü��
    public float hp = 100.0f;

    float lastHp;

    //�÷��̾� ���ݷ�
    public float attackDmg = 33.5f;

    Ray ray;

    public RaycastHit hitInfo;

    //����ĳ��Ʈ �Ÿ�
    public float maxRayDist = 30.0f;

    //����
    public enum State { Idle, Sprint, Attack, Hit, Dead };

    [Header("�÷��̾��� ����!")]
    public State playerMode;



    private void Awake()
    {
        //Animator ������Ʈ ����
        anim = gameObject.GetComponentInChildren<Animator>();

        player = GetComponent<Player>();

        playerCon = GetComponent<CharacterController>();
    }


    // Start is called before the first frame update
    void Start()
    {
        playerMode = State.Idle;
        lastHp = hp;
    }

    // Update is called once per frame
    void Update()
    {


        if (player != null)
        {
            ModeSet();

            ModeAction();
        }
    }

    public void ModeSet()
    {

        if (hp <= 0)  //�׾�����
        {
            playerMode = State.Dead;
        }
        else
        {
            //�¾�����
            if (lastHp > hp) //�����ؾ���
            {
                playerMode = State.Hit;
            }
            else if (Input.GetKeyDown(KeyCode.Mouse0))      //���콺 ��Ŭ�� ��������
            {
                playerMode = State.Attack;
            }
            //�����϶�
            else if (Mathf.Abs(playerCon.velocity.x + playerCon.velocity.y) > 0)
            {
                playerMode = State.Sprint;
            }
            else  //�ƹ��͵� ���Ҷ�
            {
                playerMode = State.Idle;
            }
        }

    }

    public void ModeAction()
    {
        //�⺻������ �޸��� off 
        anim.SetFloat("Run", 0f);

        if (playerMode == State.Sprint)  //�޸��� ����϶�
        {
            anim.SetFloat("Run", 1f);
        }
        else if (playerMode == State.Attack)
        {
            anim.SetTrigger("Attack");
            Attack();
        }
        else if (playerMode == State.Hit)
        {
            anim.SetTrigger("Hit");
            //Damage();
        }
        else if (playerMode == State.Dead)
        {
            anim.SetTrigger("Dead");
        }
    }

    public void Attack()
    {
        //�÷��̾� ray ����
        ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        //ray �������� �����
        Debug.DrawRay(ray.origin, ray.direction * maxRayDist, Color.red);

        //�� ray�� �������� �±װ� Enemy �϶�
        if (Physics.Raycast(ray, out hitInfo, maxRayDist) && hitInfo.transform.tag == "Enemy")
        {
            ////mon ������ ���� ray�� ���� monster�� ����
            //mon = hitInfo.transform.GetComponent<monsterCtrl>();

            //Debug.Log("����!");

            ////���� monster�� ����� �Լ� ����
            //mon.Damage(attackDmg);
        }
    }

    public void Damage(float MonDam)
    {
        //���� ü�¿� ���� ����� ��ŭ ��
        hp -= MonDam;

        if (hp > 0)
        {
            Debug.Log("�������� ���ݹ���! ���� ü�� : " + hp);
        }

        if (hp <= 0)
        {//��� �Լ� ����
            Dead();
        }

        //������ ü���� ���ݹ��� �ֱ� ü������
        lastHp = hp;
    }

    public void Dead()
    {
        //�׾����� �� ����
        //ex)�÷��̾� �����
    }

}
