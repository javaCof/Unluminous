using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class VrPlayer : Player
{
    public Transform actor;
    public Transform avatar;

    public float attackCooldown = 0.3f;     //공격 쿨타임

    private Rigidbody rig;

    private Enemy enemy;
    private Vector3 vrhitPoint;               //피격 위치
    private float lastAttackTime;

    private bool animMove;
    private Vector3 curPos;
    private Quaternion curRot;

    private void Awake()
    {
        anim = actor.GetComponentInChildren<Animator>();
        rig = actor.GetComponent<Rigidbody>();
        map = FindObjectOfType<MapGenerator>();
        pv = GetComponent<PhotonView>();
    }
    private void Start()
    {
        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            actor.gameObject.SetActive(true);
            avatar.gameObject.SetActive(false);

            roomNum = -1;
            map.mainCam.gameObject.SetActive(false);
        }
        else
        {
            anim = avatar.GetComponentInChildren<Animator>();

            actor.gameObject.SetActive(false);
            avatar.gameObject.SetActive(true);
        }
    }


    private void Update()
    {
        UpdateRoomNum();

        if (!PhotonNetwork.inRoom || pv.isMine) { }
        else
        {
            UpdatePos();
            UpdateAnimMove(animMove);
        }

    }

    //상인거래
    public void VrTrade(Collision trade)
    {
        Debug.Log("상인");
        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            if (trade != null && trade.gameObject.tag == "Trade")
                trade.gameObject.GetComponent<Trader>().Trade();
        }
    }

    //에너미 공격
    public void VrAttackAction(Collision target,Vector3 hitPos)
    {
        if (target != null && target.gameObject.tag == "Enemy")
        {
            //지정된 초마다 1번씩만 호출
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                //에너미 변수에 받아온 인자의 에너미를 넣는다
                enemy = target.gameObject.GetComponent<Enemy>();

                //몬스터가 칼이랑 부딪힌 위치
                vrhitPoint = hitPos;

                //에너미가 살아있을때만 이펙트 생성
                if (!enemy.isDead)
                {
                    MakeEffect(vrhitPoint);
                }

                //싱글일때
                if (!PhotonNetwork.inRoom)
                {
                    enemy.OnHit(stat.ATK);
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
        Debug.Log("상자 오픈");
        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            if (chest != null && chest.gameObject.tag == "Chest")
                chest.gameObject.GetComponent<Chest>().Open();
        }
    }

    //아이템 줍기
    public void VrPickupItem(Collision item)
    {
        Debug.Log("아이템 줏음");
        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            if (item != null && item.gameObject.tag == "Item")
                item.gameObject.GetComponent<Item>().Pickup();
        }
    }


    public override void Attack() { } //call by anim
    [PunRPC] public override void OnHit(float dmg)
    {
        if (isDead) return;

        curHP -= dmg;
        FindObjectOfType<GameUI>().hpBar.fillAmount = curHP / stat.HP;

        if (curHP <= 0)
        {
            if (PhotonNetwork.inRoom)
                pv.RPC("Dead_All", PhotonTargets.All);
            else Dead_All();

            isDead = true;
            controllable = false;

            StartCoroutine(Dead(1));
        }
        else
        {
            if (PhotonNetwork.inRoom)
                pv.RPC("Hit_All", PhotonTargets.All);
            else Hit_All();
        }
    }
    [PunRPC] void Hit_All()
    {
        //anim.SetTrigger("hit");
    }

    [PunRPC] void Dead_All()
    {
        anim.applyRootMotion = true;
        anim.SetTrigger("dead");
    }

   new void UpdateRoomNum()
    {
        roomNum = map.FindRoom(actor.position);
    }

    protected override IEnumerator Dead(float delay)
    {
        yield return new WaitForSeconds(delay);

        SceneManager.LoadSceneAsync("GameEndScene", LoadSceneMode.Additive);
    }

    void UpdatePos()
    {
        transform.position = Vector3.Lerp(transform.position, curPos, 3.0f * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, curRot, 3.0f * Time.deltaTime);
    }
    void UpdateAnimMove(bool isMove)
    {
        animMove = isMove;
        anim.SetBool("move", animMove);
    }
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(animMove);

            stream.SendNext(isDead);
            stream.SendNext(roomNum);
        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
            curRot = (Quaternion)stream.ReceiveNext();
            animMove = (bool)stream.ReceiveNext();

            isDead = (bool)stream.ReceiveNext();
            roomNum = (int)stream.ReceiveNext();
        }
    }

    [PunRPC] public override void OnPoolCreate(int id)
    {
        this.id = id;
        SetStat();

        if (PhotonNetwork.inRoom)
        {
            if (pv.isMine) pv.RPC("OnPoolCreate", PhotonTargets.Others, id);
            transform.parent = map.poolPos;
            gameObject.SetActive(false);
        }
    }
    [PunRPC] public override void OnPoolEnable(Vector3 pos, Quaternion rot)
    {
        if (PhotonNetwork.inRoom)
        {
            if (pv.isMine) pv.RPC("OnPoolEnable", PhotonTargets.Others, pos, rot);
            transform.parent = map.objectPos;
            transform.position = pos;
            transform.rotation = rot;
            gameObject.SetActive(true);
        }

        Reset();
    }
    [PunRPC] public override void OnPoolDisable()
    {
        if (PhotonNetwork.inRoom)
        {
            if (pv.isMine) pv.RPC("OnPoolDisable", PhotonTargets.Others);
            transform.parent = map.poolPos;
            gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected() { }
}
