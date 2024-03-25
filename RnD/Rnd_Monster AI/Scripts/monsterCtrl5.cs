using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class monsterCtrl5 : MonoBehaviour
{   //�׺���̼�
    public NavMeshAgent myTraceAgent;

    //������ ��ġ
    public Vector3 MovePoint = Vector3.zero;

    //�÷��̾� ������Ʈ
    public GameObject player;

    //�ִϸ��̼�
    Animator anim;


    //������ �ʱ� ��ġ
    Vector3 monsterStartPosition;






    public enum state { idle = 1, trace, attack, look, resetPosition };

    [Header("������ ����!")]
    public state enemyMode = state.idle;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        myTraceAgent = GetComponent<NavMeshAgent>();
        anim = transform.Find("mon").gameObject.GetComponent<Animator>();
        monsterStartPosition = transform.position;

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
        if (player != null)
        {   //�÷��̾�� ���� ��ġ�Ÿ�
            float distance = (transform.position - player.transform.position).sqrMagnitude;
            //������ �ʱ���ġ�� ���� ��ġ�Ÿ�
            float startDistance = (transform.position - monsterStartPosition).sqrMagnitude;

            //Debug.Log(startDistance);

            //��� ����

            if (distance < 5.0f)
            {   //���ݽ��� �Ÿ�
                enemyMode = state.attack;
            }
            
            else if (startDistance > 200f)
            {   //�ʱ���ġ���� �����Ÿ� �̻� ���������� 
                enemyMode = state.resetPosition;
            }
            else if (startDistance < 200f)
            {
                if (enemyMode == state.resetPosition && startDistance < 10f)
                {
                    Debug.Log("�ʱ���ġ �̵��� idle");
                    enemyMode = state.idle;
                }
                //���۰Ÿ����� ����ﶧ
                else if (enemyMode != state.resetPosition && startDistance < 10f)
                {   //�÷��̾�� �Ÿ��� 200���Ϸ� ����ﶧ
                    if (distance < 200f)
                    {   //��������
                        enemyMode = state.trace;
                    }
                }
            }
            else if (distance < 300f)
            {   //player���� ���� ����
                enemyMode = state.look;
            }
            else if (distance > 300f)
            {   //�����Ÿ����� �ֶ�
                enemyMode = state.idle;
            }









            //��� ����
            if (enemyMode == state.idle)
            {
                anim.Play("Idle");
                myTraceAgent.isStopped = true;
            }
            else if (enemyMode == state.look)
            {   //���� ������ ����
                myTraceAgent.isStopped = true;

                //���� �ִϸ��̼� idle
                anim.Play("Idle");

                //���Ͱ� player �ٶ�
                transform.LookAt(player.transform.position);
            }
            else if (enemyMode == state.trace)
            {
                //�����Ҷ� �÷��̾ �ٶ󺸰� LookAt
                transform.LookAt(player.transform.position);

                //�����Ҷ� run�ִϸ��̼� �۵�
                anim.SetFloat("Run", Mathf.Abs(myTraceAgent.velocity.x + myTraceAgent.velocity.z));

                //��ž�� ����
                myTraceAgent.isStopped = false;

                //������ ���� �÷��̾��� ��ġ�� ���� 
                myTraceAgent.destination = player.transform.position;

                //�÷��̾�Լ� ���⶧ �󸶳� �Ÿ��� ��������
                myTraceAgent.stoppingDistance = 2.5f;
            }
            else if (enemyMode == state.attack)
            {
                //������ �÷��̾ �ٶ󺸰� LookAt
                transform.LookAt(player.transform.position);

                //�����Ҷ� �̵� ����
                myTraceAgent.isStopped = true;

                //���� �ִϸ��̼�
                anim.SetTrigger("Attack");
            }
            else if (enemyMode == state.resetPosition)
            {
                //�����Ҷ� run�ִϸ��̼� �۵�
                anim.SetFloat("Run", Mathf.Abs(myTraceAgent.velocity.x + myTraceAgent.velocity.z));

                //������ ���� ���
                myTraceAgent.isStopped = false;

                //��ǥ�� monsterStartPosition��
                myTraceAgent.destination = monsterStartPosition;



            }
        }






    }


    IEnumerator ModeSetting()
    {//�����̽��ٸ� ������

        yield return null;
    }

    IEnumerator ModeAction()
    {

        //else if (enemyMode==state.idle)
        //{
        //    myTraceAgent.isStopped = true;
        //    anim.Play("Idle");
        //}

        yield return null;
    }
    //private void OnTriggerEnter(Collider col)
    //{
    //    if (col.gameObject.tag == "Player")
    //    {
    //        Debug.Log("�÷��̾ �տ� ����!");
    //        anim.SetTrigger("Attack");

    //    }
    //}
}
