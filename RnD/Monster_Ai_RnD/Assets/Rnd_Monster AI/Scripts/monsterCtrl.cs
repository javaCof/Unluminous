using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class monsterCtrl : MonoBehaviour
{
    //������ ü��
    public float hp=100;

    public float attackDmg=10;

    //�׺���̼�
    public NavMeshAgent myTraceAgent;

    //������ ��ġ
    public Vector3 MovePoint = Vector3.zero;

    //�÷��̾� ������Ʈ
    public GameObject[] players;
    public GameObject player;

    //�ִϸ��̼�
    Animator anim;

    //������ �ʱ� ��ġ
    public Transform monsterStartPosition;


    public enum state { idle = 1, trace, attack, look, resetPosition, damage, dead };

    [Header("������ ����!")]
    public state enemyMode = state.idle;

    private void Awake()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        myTraceAgent = GetComponent<NavMeshAgent>();
        anim = transform.Find("mon").gameObject.GetComponent<Animator>();

    }
    // Start is called before the first frame update
    void Start()
    {
        myTraceAgent.isStopped = true;

        enemyMode = state.idle;
    }

    // Update is called once per frame
    void Update()
    {
        if (players.Length >0)
        {

            //Ÿ�� ����
            StartCoroutine(TargerSetting());

            //��� ����
            StartCoroutine(ModeSetting());

            //��� �ൿ
            StartCoroutine(ModeAction());
        }

    }

    IEnumerator TargerSetting()
    {

        //�÷��̾�� ���� ��ġ�Ÿ�


        if (players.Length >= 0)
        {
            float dist = (transform.position - players[0].transform.position).sqrMagnitude;

            player = players[0];

            foreach (GameObject _player in players)
            {
                if ((transform.position - _player.transform.position).sqrMagnitude < dist)
                {
                    player = _player;
                    dist = (transform.position - _player.transform.position).sqrMagnitude;
                    //Debug.Log("����� �Ÿ�" + dist);
                }
            }
        }


        yield return null;
    }


    IEnumerator ModeSetting()
    {
        //���Ͱ� ���������
        if (hp>0)
        {
            //�÷��̾�� ���� ��ġ�Ÿ�
            float distance = (transform.position - player.transform.position).sqrMagnitude;
            //Debug.Log(distance);

            //������ �ʱ���ġ�� ���� ��ġ�Ÿ�
            float startDistance = (transform.position - monsterStartPosition.position).sqrMagnitude;


            //���� ���� �������� 10�̻� �϶�(���� ����϶��� �������� �������� ��� ���� ���)
            if (enemyMode == state.resetPosition && startDistance > 5)
            {
                enemyMode = state.resetPosition;
            }
            //�������� 200�̻����� �Ѿ����
            else if (startDistance > 200)
            {
                enemyMode = state.resetPosition;
            }

            //�����Ÿ� 200�̻����� �ȳѾ� ������
            else
            {  //�÷��̾ �ſ� ������ ������
                if (distance < 3)
                {
                    enemyMode = state.attack;
                }

                //�÷��̾�� �Ÿ��� ���� �����ȿ� ��������
                else if (distance < 200)
                {
                    enemyMode = state.trace;
                }

                //look ���� �ȿ� �� ������
                else if (distance < 300)
                {
                    enemyMode = state.look;
                }

                else
                {
                    enemyMode = state.idle;
                }
            }
        }
        else
        {
            enemyMode = state.dead;
        }

        yield return null;


    }

    IEnumerator ModeAction()
    {

        Vector3 playerLook = player.transform.position;
        playerLook.y = transform.position.y;





        //��� ����
        if (enemyMode == state.idle)
        {
            //�� ��� ����
            anim.SetFloat("Run", 0);

            //���� ������ ����
            myTraceAgent.isStopped = true;

            //idle��� ���
            anim.Play("Idle");
        }
        else if (enemyMode == state.look)
        {
            //�� ��� ����
            anim.SetFloat("Run", 0);

            //���� ������ ����
            myTraceAgent.isStopped = true;

            //���� �ִϸ��̼� idle
            anim.Play("Idle");

            //���Ͱ� player �ٶ�
            transform.LookAt(player.transform.position);
        }
        else if (enemyMode == state.trace)
        {

            //�����Ҷ� �÷��̾ �ٶ󺸰� LookAt
            transform.LookAt(playerLook);

            //run�ִϸ��̼� �۵�
            anim.SetFloat("Run", 1);

            //��ž�� ����
            myTraceAgent.isStopped = false;

            //������ ���� �÷��̾��� ��ġ�� ���� 
            myTraceAgent.destination = player.transform.position;

            //�÷��̾�Լ� ���⶧ �󸶳� �Ÿ��� ��������
            myTraceAgent.stoppingDistance = 0.5f;
        }
        else if (enemyMode == state.attack)
        {
            //�� ��� ����
            anim.SetFloat("Run", 0);

            //������ �÷��̾ �ٶ󺸰� LookAt
            transform.LookAt(playerLook);

            //�����Ҷ� �̵� ����
            myTraceAgent.isStopped = true;

            //���� �ִϸ��̼�
            anim.SetTrigger("Attack");
        }
        else if (enemyMode == state.resetPosition)
        {
            //�����Ҷ� run�ִϸ��̼� �۵�
            anim.SetFloat("Run", 1);
            //anim.SetFloat("Run", Mathf.Abs(myTraceAgent.velocity.x + myTraceAgent.velocity.z));

            //������ ���� ���
            myTraceAgent.isStopped = false;

            //��ǥ�� monsterStartPosition��
            myTraceAgent.destination = monsterStartPosition.position;
        }
        else if (enemyMode == state.damage)
        {

            //�� ��� ����
            anim.SetFloat("Run", 0);

            //�̵� ����
            myTraceAgent.isStopped = true;

            //����� �ִϸ��̼� ���
            anim.SetTrigger("Damage");

        }
        else if (enemyMode == state.dead)
        {
            //�� ��� ����
            anim.SetFloat("Run", 0);

            //�̵� ����
            myTraceAgent.isStopped = true;

            //�״� ��� ���
            anim.SetBool("Dead", true);
        }

        yield return null;
    }

    //����� �޴� �Լ�
    public void Damage(float playerDmg)
    { 
        //���� ü�¿� ���� ����� ��ŭ ��
        hp -= playerDmg;

        //�ӽ�
        if (hp>0)
        {
            Debug.Log("���ݹ���! ���� ü�� : " + hp);
        }

        //������ ü���� 0 �����϶�
        if (hp<=0)
        {   //��� �Լ� ����
            Dead();
        }
    }

    //���͸� ���̴� �Լ�
    public void Dead()
    {
        //2���� ������
        Destroy(gameObject, 2.0f);

        //gameObject.SetActive(false);
    }

}
