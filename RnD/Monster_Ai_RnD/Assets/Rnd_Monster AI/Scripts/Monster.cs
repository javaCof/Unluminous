using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : UnitCharater
{
    public Rect roomRect;          //�� ����

    private Animator anim;
    private NavMeshAgent nav;

    public Transform Target { get; private set; }
    
    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        nav = GetComponent<NavMeshAgent>();
    }
    void Start()
    {
        nav.isStopped = true;
        state = new MonsterSearchState(this);
    }
    void Update()
    {
        state.UpdateState();
    }

    class MonsterSearchState : UnitState
    {   //Ž������
        GameObject[] players;

        public MonsterSearchState(UnitCharater _unit) : base(_unit) { }
        public override void BeginState() 
        {
            players = GameObject.FindGameObjectsWithTag("Player");
        }
        public override void UpdateState()
        {

            unit.ChangeState(new MonsterTraceState(unit));
        }
        public override void EndState() { }
    }
        
    class MonsterAlertState : UnitState
    {   //������ (�̻��)
        public MonsterAlertState(UnitCharater _unit) : base(_unit) { }
        public override void BeginState() { }
        public override void UpdateState() { }
        public override void EndState() { }
    }

    class MonsterTraceState : UnitState
    {   //��������
        public MonsterTraceState(UnitCharater _unit) : base(_unit) { }
        public override void BeginState() { }
        public override void UpdateState() { }
        public override void EndState() { }
    }

    class MonsterAttackState : UnitState
    {   //���ݻ���
        public MonsterAttackState(UnitCharater _unit) : base(_unit) { }
        public override void BeginState() { }
        public override void UpdateState() { }
        public override void EndState() { }
    }

    class MonsterReposState : UnitState
    {   //���ͻ���
        public MonsterReposState(UnitCharater _unit) : base(_unit) { }
        public override void BeginState() { }
        public override void UpdateState() { }
        public override void EndState() { }
    }

    public override void Hit(UnitCharater other) { }
    public override void Attack(UnitCharater target) { }
    public override void Dead() { }






    /*���� �ڵ�*/

    GameObject[] players;
    GameObject player;

    void TargerSetting()
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

    void ModeAction()
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
}
