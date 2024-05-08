using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class VrPlayer : Player
{
    //���Ͱ� �´� ��ġ

    Rigidbody rigidbody;

    private Enemy enemy;
    public GameObject sword;

    float lastAttackTime;

    //���� ��Ÿ��
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

    //�����̴� �ִϸ��̼� ����ȭ
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

    //���ΰŷ�
    public void VrTrade(Collision trade)
    {
        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            if (trade != null && trade.gameObject.tag == "Trade")
                trade.gameObject.GetComponent<Trader>().Trade();
        }
    }

    //���ʹ� ����
    public void VrAttackAction(Collision target)
    {
        if (target != null && target.gameObject.tag == "Enemy")
        {
            //������ �ʸ��� 1������ ȣ��
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                //���ʹ� ������ �޾ƿ� ������ ���ʹ̸� �ִ´�
                enemy = target.gameObject.GetComponent<Enemy>();

                //���Ͱ� Į�̶� �ε��� ��ġ
                hitPoint = target.transform.position;

                //���ʹ̰� ����������� ����Ʈ ����
                if (!enemy.isDead)
                {
                    MakeEffect(hitPoint);
                }

                //�̱��϶�
                if (!PhotonNetwork.inRoom)
                {
                    enemy.Hit_Master(stat.ATK);
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
        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            if (chest != null && chest.gameObject.tag == "Chest")
                chest.gameObject.GetComponent<Chest>().Open();
        }
    }

    //������ �ݱ�
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
        //�׾��ٸ� ����
        if (isDead) return;

        //������ ��ŭ ü�� ����
        curHP -= dmg;

        //ü�¹� ã�Ƽ� �ۼ�Ʈ ������ ü�� ���̴°� �ݿ�
        FindObjectOfType<GameUI>().hpBar.fillAmount = curHP / stat.HP;

        //ü���� 0���� �Ǹ�
        if (curHP <= 0)
        {
            //��Ƽ�϶�
            if (PhotonNetwork.inRoom)
            {   
                //��Ƽ��뿡�� �״¸�� ������
                pv.RPC("Dead_All", PhotonTargets.All);
            }
            //�̱��϶�
            else Dead_All();
            

            isDead = true;

            StartCoroutine(DeadPlayer());
        }
        //�¾����� ü���� 1�̻��϶�
        else
        {
            if (PhotonNetwork.inRoom)
                pv.RPC("Hit_All", PhotonTargets.All);

            else Hit_All();
        }
    }

    //�¾�����
    [PunRPC]public void Hit_All()
    {
        anim.SetTrigger("hit");
    }


    //�׾�����
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

    //��Ʈ��ũ �󿡼� ������Ʈ�� ���¸� ����ȭ�ϴ� �Լ�
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
