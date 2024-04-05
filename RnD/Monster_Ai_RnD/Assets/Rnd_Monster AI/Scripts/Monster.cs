using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : UnitCharater
{



    public enum State { Search, Alert, Trace, Attack, Repos, Hit, Dead };


    public Rect roomArea;
    UnitState state;


    class MonsterStateSearch : UnitState
    {
        public override void BeginState()
        {
            throw new System.NotImplementedException();
        }

        public override void EndState()
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateState()
        {
            //Ž������ ������ �÷��̾ Ž��
            //Ž������ ���� �÷��̾ ������ ���, �������¿� ����

        }
    }

    void ChangeState(UnitState state)
    {
        if (this.state) this.state.EndState();
        this.state = state;
        this.state.BeginState();
    }

    class MonsterAlertSearch : UnitState
    {
        public override void BeginState()
        {
            throw new System.NotImplementedException();
        }

        public override void EndState()
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateState()
        {
            throw new System.NotImplementedException();
        }
    }



    private Animator anim;              //�ִϸ��̼�
    private NavMeshAgent nav;           //�׺���̼�


    bool isDead;
    bool hasTarget;



    //�÷��̾� ������Ʈ
    private GameObject[] players;
    private GameObject player;



    private void Awake()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

    }
    // Start is called before the first frame update
    void Start()
    {
        nav.isStopped = true;
        state = State.idle;
    }

    // Update is called once per frame
    void Update()
    {
        isDead = (curHP <= 0);

        if (players.Length >0)
        {

            //Ÿ�� ����
            StartCoroutine(TargerSetting());

            //��� ����
            StartCoroutine(ModeSetting());

            //��� �ൿ
            StartCoroutine(ModeAction());
        }

        state.UpdateState();
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


    void ModeSetting()
    {



        //���Ͱ� ���������
        if (curHP>0)
        {
            //�÷��̾�� ���� ��ġ�Ÿ�
            float distance = (transform.position - player.transform.position).sqrMagnitude;
            //Debug.Log(distance);

            //������ �ʱ���ġ�� ���� ��ġ�Ÿ�
            float startDistance = (transform.position - monsterStartPosition.position).sqrMagnitude;


            //���� ���� �������� 10�̻� �϶�(���� ����϶��� �������� �������� ��� ���� ���)
            if (state == State.resetPosition && startDistance > 5)
            {
                state = State.resetPosition;
            }
            //�������� 200�̻����� �Ѿ����
            else if (startDistance > 200)
            {
                state = State.resetPosition;
            }

            //�����Ÿ� 200�̻����� �ȳѾ� ������
            else
            {  //�÷��̾ �ſ� ������ ������
                if (distance < 3)
                {
                    state = State.attack;
                }

                //�÷��̾�� �Ÿ��� ���� �����ȿ� ��������
                else if (distance < 200)
                {
                    state = State.trace;
                }

                //look ���� �ȿ� �� ������
                else if (distance < 300)
                {
                    state = State.look;
                }

                else
                {
                    state = State.idle;
                }
            }
        }
        else
        {
            state = State.dead;
        }
    }

    IEnumerator ModeAction()
    {

        Vector3 playerLook = player.transform.position;
        playerLook.y = transform.position.y;





        //��� ����
        if (state == State.idle)
        {
            //�� ��� ����
            anim.SetFloat("Run", 0);

            //���� ������ ����
            nav.isStopped = true;

            //idle��� ���
            anim.Play("Idle");
        }
        else if (state == State.look)
        {
            //�� ��� ����
            anim.SetFloat("Run", 0);

            //���� ������ ����
            nav.isStopped = true;

            //���� �ִϸ��̼� idle
            anim.Play("Idle");

            //���Ͱ� player �ٶ�
            transform.LookAt(player.transform.position);
        }
        else if (state == State.trace)
        {

            //�����Ҷ� �÷��̾ �ٶ󺸰� LookAt
            transform.LookAt(playerLook);

            //run�ִϸ��̼� �۵�
            anim.SetFloat("Run", 1);

            //��ž�� ����
            nav.isStopped = false;

            //������ ���� �÷��̾��� ��ġ�� ���� 
            nav.destination = player.transform.position;

            //�÷��̾�Լ� ���⶧ �󸶳� �Ÿ��� ��������
            nav.stoppingDistance = 0.5f;
        }
        else if (state == State.attack)
        {
            //�� ��� ����
            anim.SetFloat("Run", 0);

            //������ �÷��̾ �ٶ󺸰� LookAt
            transform.LookAt(playerLook);

            //�����Ҷ� �̵� ����
            nav.isStopped = true;

            //���� �ִϸ��̼�
            anim.SetTrigger("Attack");
        }
        else if (state == State.resetPosition)
        {
            //�����Ҷ� run�ִϸ��̼� �۵�
            anim.SetFloat("Run", 1);
            //anim.SetFloat("Run", Mathf.Abs(myTraceAgent.velocity.x + myTraceAgent.velocity.z));

            //������ ���� ���
            nav.isStopped = false;

            //��ǥ�� monsterStartPosition��
            nav.destination = monsterStartPosition.position;
        }
        else if (state == State.damage)
        {

            //�� ��� ����
            anim.SetFloat("Run", 0);

            //�̵� ����
            nav.isStopped = true;

            //����� �ִϸ��̼� ���
            anim.SetTrigger("Damage");

        }
        else if (state == State.dead)
        {
            //�� ��� ����
            anim.SetFloat("Run", 0);

            //�̵� ����
            nav.isStopped = true;

            //�״� ��� ���
            anim.SetBool("Dead", true);
        }

        yield return null;
    }

    //����� �޴� �Լ�
    public void Damage(float playerDmg)
    { 
        //���� ü�¿� ���� ����� ��ŭ ��
        curHP -= playerDmg;

        //�ӽ�
        if (curHP>0)
        {
            Debug.Log("���ݹ���! ���� ü�� : " + curHP);
        }

        //������ ü���� 0 �����϶�
        if (curHP<=0)
        {   //��� �Լ� ����
            Dead();
        }
    }


    public override void Hit(UnitCharater other)
    {
        throw new System.NotImplementedException();
    }

    public override void Attack(UnitCharater target)
    {
        throw new System.NotImplementedException();
    }
}
