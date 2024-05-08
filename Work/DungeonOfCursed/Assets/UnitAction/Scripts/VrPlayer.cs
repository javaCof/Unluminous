using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class VrPlayer : Player
{
    //몬스터가 맞는 위치

    Rigidbody rigidbody;

    private Enemy enemy;
    public GameObject sword;

    float lastAttackTime;

    //공격 쿨타임
    public float attackCooldown = 0.3f;



    private void Awake()
    {
        anim= anim = GetComponentInChildren<Animator>();
        sword = GameObject.Find("swordColider");
        hitPoint = Vector3.zero;
    }



    // Start is called before the first frame update
    void Start()
    {
        
        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            roomNum = -1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        AnimMove(Mathf.Abs(rigidbody.velocity.x + rigidbody.velocity.z) > 0.1);
        

    }

    //움직이는 애니메이션 동기화
   void AnimMove(bool isMove)
    {
        anim.SetBool("move", isMove);

    }

    void Reset()
    {
        curHP = stat.HP;
        isDead = false;
    }

     void UpdateRoomNum()
    {
        roomNum = map.FindRoom(transform.position);
    }

    //상인거래
    public void VrTrade(Collision trade)
    {
        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            if (trade != null && trade.gameObject.tag == "Trade")
                trade.gameObject.GetComponent<Trader>().Trade();
        }
    }

    //에너미 공격
    public void VrAttackAction(Collision target)
    {
        if (target != null && target.gameObject.tag == "Enemy")
        {
            //지정된 초마다 1번씩만 호출
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                //에너미 변수에 받아온 인자의 에너미를 넣는다
                enemy = target.gameObject.GetComponent<Enemy>();

                //몬스터가 칼이랑 부딪힌 위치
                hitPoint = target.transform.position;

                //에너미가 살아있을때만 이펙트 생성
                if (!enemy.isDead)
                {
                    MakeEffect(hitPoint);
                }

                //싱글일때
                if (!PhotonNetwork.inRoom)
                {
                    enemy.Hit_Master(stat.ATK);
                }

                //멀티 일때
                else if (pv.isMine)
                {
                    enemy.GetComponent<PhotonView>().RPC("Hit_Master", PhotonNetwork.masterClient, stat.ATK);
                }
                // 현재 시간을 마지막 공격 시간으로 업데이트
                lastAttackTime = Time.time;
            }
        }
    }


    //상자 열기
    public void VrOpenChest(Collision chest)
    {
        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            if (chest != null && chest.gameObject.tag == "Chest")
                chest.gameObject.GetComponent<Chest>().Open();
        }
    }

    //아이템 줍기
    public void VrPickupItem(Collision item)
    {
        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            if (item != null && item.gameObject.tag == "Item")
                item.gameObject.GetComponent<Item>().Pickup();
        }
    }

    [PunRPC] public void Hit_Owner(float dmg)
    {
        //죽었다면 리턴
        if (isDead) return;

        //데미지 만큼 체력 깎음
        curHP -= dmg;

        //체력바 찾아서 퍼센트 비율로 체력 깍이는거 반영
        FindObjectOfType<GameUI>().hpBar.fillAmount = curHP / stat.HP;

        //체력이 0이하 되면
        if (curHP <= 0)
        {
            //멀티일때
            if (PhotonNetwork.inRoom)
            {   
                //멀티상대에게 죽는모션 보여줌
                pv.RPC("Dead_All", PhotonTargets.All);
            }
            //싱글일때
            else Dead_All();
            

            isDead = true;

            StartCoroutine(DeadPlayer());
        }
        //맞았지만 체력이 1이상일때
        else
        {
            if (PhotonNetwork.inRoom)
                pv.RPC("Hit_All", PhotonTargets.All);

            else Hit_All();
        }
    }

    //맞았을때
    [PunRPC]public void Hit_All()
    {
        anim.SetTrigger("hit");
    }


    //죽었을때
    [PunRPC]void Dead_All()
    {
        anim.applyRootMotion = true;
        anim.SetTrigger("dead");
    }

    
    IEnumerator DeadPlayer()
    {
        yield return new WaitForSeconds(1);

        SceneManager.LoadSceneAsync("GameEndScene", LoadSceneMode.Additive);
    }

  

    public override void AttackAction()
    {

    }

    //네트워크 상에서 오브젝트의 상태를 동기화하는 함수
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(isDead);
            stream.SendNext(roomNum);
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            //curPos = (Vector3)stream.ReceiveNext();
            //curRot = (Quaternion)stream.ReceiveNext();
            //animMove = (bool)stream.ReceiveNext();

            isDead = (bool)stream.ReceiveNext();
            roomNum = (int)stream.ReceiveNext();
        }
    }


}
