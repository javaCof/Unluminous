using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class VrPlayer : Player
{
    public Transform actor;
    public Transform avatar;

    public float attackCooldown = 0.3f;     //���� ��Ÿ��

    private Rigidbody rig;

    private Enemy enemy;
    private Vector3 vrhitPoint;               //�ǰ� ��ġ
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

    //���ΰŷ�
    public void VrTrade(Collision trade)
    {
        Debug.Log("����");
        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            if (trade != null && trade.gameObject.tag == "Trade")
                trade.gameObject.GetComponent<Trader>().Trade();
        }
    }

    //���ʹ� ����
    public void VrAttackAction(Collision target,Vector3 hitPos)
    {
        if (target != null && target.gameObject.tag == "Enemy")
        {
            //������ �ʸ��� 1������ ȣ��
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                //���ʹ� ������ �޾ƿ� ������ ���ʹ̸� �ִ´�
                enemy = target.gameObject.GetComponent<Enemy>();

                //���Ͱ� Į�̶� �ε��� ��ġ
                vrhitPoint = hitPos;

                //���ʹ̰� ����������� ����Ʈ ����
                if (!enemy.isDead)
                {
                    MakeEffect(vrhitPoint);
                }

                //�̱��϶�
                if (!PhotonNetwork.inRoom)
                {
                    enemy.OnHit(stat.ATK);
                }

                //��Ƽ �϶�
                else if (pv.isMine)
                {
                    enemy.GetComponent<PhotonView>().RPC("Hit_Master", PhotonNetwork.masterClient, stat.ATK);
                }
                // ���� �ð��� ������ ���� �ð����� ������Ʈ
                lastAttackTime = Time.time;
            }
        }
    }


    //���� ����
    public void VrOpenChest(Collision chest)
    {
        Debug.Log("���� ����");
        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            if (chest != null && chest.gameObject.tag == "Chest")
                chest.gameObject.GetComponent<Chest>().Open();
        }
    }

    //������ �ݱ�
    public void VrPickupItem(Collision item)
    {
        Debug.Log("������ ����");
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
