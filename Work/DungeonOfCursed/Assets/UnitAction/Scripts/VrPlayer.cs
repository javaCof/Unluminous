using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class VrPlayer : Player
{
    private AudioSource audioSource;
    private AudioSource swordAudioSource;

    public float attackCooldown = 0.3f;     //���� ��Ÿ��

    private float lastAttackTime;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        map = FindObjectOfType<MapGenerator>();
        pv = GetComponent<PhotonView>();
        ui = FindObjectOfType<GameUI>();

        audioSource = gameObject.GetComponent<AudioSource>();
        swordAudioSource = GetComponentInChildren<AudioSource>();
    }
    private void Start()
    {
        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            roomNum = -1;
            map.mainCam.gameObject.SetActive(false);
        }
        Reset();
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
    private void FixedUpdate() { }
    private void LateUpdate() { }

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
    public void VrAttackAction(Collision target, Vector3 hitPos)
    {
        if (target != null && target.gameObject.tag == "Enemy")
        {
            //Į�Ҹ� ���
            SoundManager.instance.PlaySwrod(swordAudioSource);

            //������ �ʸ��� 1������ ȣ��
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                //���ʹ� ������ �޾ƿ� ������ ���ʹ̸� �ִ´�
                Enemy enemy = target.gameObject.GetComponent<Enemy>();

                //���Ͱ� Į�̶� �ε��� ��ġ
                Vector3 vrhitPoint = hitPos;

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
       // Debug.Log("���� ����");
        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            if (chest != null && chest.gameObject.tag == "Chest")
            {
                chest.gameObject.GetComponent<Chest>().Open();
                SoundManager.instance.PlaySfx("chest");
            }
        }
    }

    //������ �ݱ�
    public void VrPickupItem(Collision item)
    {
        //Debug.Log("������ ����");
        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            if (item != null && item.gameObject.tag == "Item")
            {
                item.gameObject.GetComponent<Item>().Pickup();
                SoundManager.instance.PlaySfx("item");
            }
        }
    }

    public override void Attack() { } //call by anim
    [PunRPC]
    public override void OnHit(float dmg)
    {
        if (isDead) return;

        curHP -= dmg;
        FindObjectOfType<GameUI>().hpBar.fillAmount = curHP / stat.HP;

        //�´¼Ҹ� ���
        SoundManager.instance.PlayHit(audioSource);

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
    [PunRPC]
    void Hit_All()
    {
        anim.SetTrigger("hit");
    }

    [PunRPC]
    void Dead_All()
    {
        anim.applyRootMotion = true;
        anim.SetTrigger("dead");
    }
    protected override IEnumerator Dead(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameManager.Instance.LoadingScene("GameEndScene");
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

    private void OnDrawGizmosSelected() { }
}
