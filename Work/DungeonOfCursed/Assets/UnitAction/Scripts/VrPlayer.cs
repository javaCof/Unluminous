using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrPlayer : Player
{
    //몬스터가 맞는 위치

    public Enemy enemy;
    public GameObject sword;




    private void Awake()
    {
        sword = GameObject.Find("swordColider");
        hitPoint = Vector3.zero;
        pv= GetComponent<PhotonView>();
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

       
    }

    public void VrAttackAction(Collision target)
    {
       

        //타겟이 null이 아니고 타겟의 태그가 에너미일 경우
        if (target == null && target.transform.tag == "Enemy")
        {
            //에너미 변수에 받아온 인자의 에너미를 넣는다
            enemy = target.gameObject.GetComponent<Enemy>();

            //몬스터가 칼이랑 부딫힌 위치
            hitPoint = target.transform.position;

            //에너미가 살아있을때만 이펙트 생성
            if (!enemy.isDead)
            { 
                MakeEffect(hitPoint); 
            }

            if (!PhotonNetwork.inRoom)
            {
                enemy.Hit_Master(stat.ATK);
            }
            //else if (pv.isMine)





        }
    }



    public override void AttackAction()
    {

    }
}
