using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //플레이어 스크립트
    Player player;

    //캐릭터 컨트롤러
    CharacterController playerCon;

    //플레이어 애니메이터
    public Animator anim;

    //레이캐스트를 맞은 몬스터를 담기위한 변수 
    //public monsterCtrl mon = null;

    //플레이어 체력
    public float hp = 100.0f;

    float lastHp;

    //플레이어 공격력
    public float attackDmg = 33.5f;

    Ray ray;

    public RaycastHit hitInfo;

    //레이캐스트 거리
    public float maxRayDist = 30.0f;

    //상태
    public enum State { Idle, Sprint, Attack, Hit, Dead };

    [Header("플레이어의 상태!")]
    public State playerMode;



    private void Awake()
    {
        //Animator 컴포넌트 연결
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

        if (hp <= 0)  //죽엇을때
        {
            playerMode = State.Dead;
        }
        else
        {
            //맞았을때
            if (lastHp > hp) //수정해야함
            {
                playerMode = State.Hit;
            }
            else if (Input.GetKeyDown(KeyCode.Mouse0))      //마우스 왼클릭 눌렀을떄
            {
                playerMode = State.Attack;
            }
            //움직일때
            else if (Mathf.Abs(playerCon.velocity.x + playerCon.velocity.y) > 0)
            {
                playerMode = State.Sprint;
            }
            else  //아무것도 안할때
            {
                playerMode = State.Idle;
            }
        }

    }

    public void ModeAction()
    {
        //기본적으로 달리기 off 
        anim.SetFloat("Run", 0f);

        if (playerMode == State.Sprint)  //달리는 모드일때
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
        //플레이어 ray 설정
        ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        //ray 보기위한 디버그
        Debug.DrawRay(ray.origin, ray.direction * maxRayDist, Color.red);

        //쏜 ray에 맞은것의 태그가 Enemy 일때
        if (Physics.Raycast(ray, out hitInfo, maxRayDist) && hitInfo.transform.tag == "Enemy")
        {
            ////mon 변수에 지금 ray에 닿은 monster를 참조
            //mon = hitInfo.transform.GetComponent<monsterCtrl>();

            //Debug.Log("공격!");

            ////닿은 monster의 대미지 함수 실행
            //mon.Damage(attackDmg);
        }
    }

    public void Damage(float MonDam)
    {
        //현재 체력에 받은 대미지 만큼 뺌
        hp -= MonDam;

        if (hp > 0)
        {
            Debug.Log("몬스터한테 공격받음! 남은 체력 : " + hp);
        }

        if (hp <= 0)
        {//사망 함수 실행
            Dead();
        }

        //마지막 체력을 공격받은 최근 체력으로
        lastHp = hp;
    }

    public void Dead()
    {
        //죽었을때 쓸 내용
        //ex)플레이어 사라짐
    }

}
