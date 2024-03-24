using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class monsterCtrl3 : MonoBehaviour
{
    public NavMeshAgent myTraceAgent;
    public Vector3 MovePoint = Vector3.zero;
    public GameObject player;
    Animator anim;
    
    


    public enum state { idle = 1, trace, attack };

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
            //�������� ����
            else if (distance<200)
            {//���� ���°� trace�� ����
                enemyMode = state.trace;
            }
            else if (distance>200)
            {
                enemyMode = state.idle;
            }
            


            //��� ����
            if (enemyMode == state.idle)
            {
                anim.Play("Idle");
                myTraceAgent.isStopped = true;
            }
            else if (enemyMode == state.trace)
            {   
                //�����Ҷ� �÷��̾ �ٶ󺸰� LookAt
                transform.LookAt(player.transform.position);
                //gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, player.transform.position, 1 * Time.deltaTime);
                anim.SetFloat("Run", Mathf.Abs(myTraceAgent.velocity.x+ myTraceAgent.velocity.z));
                //anim.Play("Run");//������ �޸��� ���
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
                //transform.FindChild("mon").transform.LookAt(player.transform);
                //gameObject.transform.LookAt(player.transform);
                myTraceAgent.isStopped = true;
                anim.SetTrigger("Attack");
            }
        }



        //if (Input.GetKeyDown("d"))
        //{
        //    Vector3 temp = transform.position;
        //    temp.x += 0.5f;
        //    transform.position = temp;
        //}


        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    enemyMode = state.move;

        //    //MovePoint = player.transform.position;
        //    //myTraceAgent.destination = MovePoint;

        //    //myTraceAgent.stoppingDistance = 2.0f;
        //}


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
