using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class monsterCtrl4 : MonoBehaviour
{
    public NavMeshAgent myTraceAgent;
    public Vector3 MovePoint = Vector3.zero;
    public GameObject player;
    Animator anim;
    
    


    public enum state { idle = 1, trace, attack ,look};

    [Header("������ ����!")]
    public state enemyMode = state.idle;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
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
        if (player != null)
        {   //��� ����
            float distance = (transform.position - player.transform.position).sqrMagnitude;
            Debug.Log(distance);


            //���ݽ��� �Ÿ�
            if (distance < 5.0f)
            {
                enemyMode = state.attack;
            }
            else if (distance<200f)
            {   //��������
                enemyMode = state.trace;
            }
            else if (distance < 300f)
            {   //player���� ���� ����
                enemyMode = state.look;
            }
            else if (distance > 300f)
            {   
                enemyMode = state.idle;
            }




            //��� ����
            if (enemyMode == state.idle)
            {   
                anim.Play("Idle");
                myTraceAgent.isStopped = true;
            }
            else if (enemyMode == state.look)
            {
                myTraceAgent.isStopped = true;
                anim.Play("Idle");
                transform.LookAt(player.transform.position);
            }
            else if (enemyMode == state.trace)
            {   
                //�����Ҷ� �÷��̾ �ٶ󺸰� LookAt
                transform.LookAt(player.transform.position);
                //gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, player.transform.position, 1 * Time.deltaTime);
                anim.SetFloat("Run", Mathf.Abs(myTraceAgent.velocity.x+ myTraceAgent.velocity.z));
                //��ž�� ����
                myTraceAgent.isStopped = false;
                //������ ���� �÷��̾��� ��ġ�� ���� 
                Debug.Log("����");
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
