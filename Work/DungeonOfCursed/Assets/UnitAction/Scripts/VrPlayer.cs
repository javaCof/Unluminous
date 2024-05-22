using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class VrPlayer : Player
{
    
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource swordAudioSource;

    [SerializeField] private Transform hand;

    public float attackCooldown = 0.3f;     //공격 쿨타임

    private float lastAttackTime;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        map = FindObjectOfType<MapGenerator>();
        pv = GetComponent<PhotonView>();
        ui = FindObjectOfType<GameUI>();

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
    public void VrAttackAction(Collision target, Vector3 hitPos)
    {
        if (target != null && target.gameObject.tag == "Enemy")
        {
            //칼소리 재생
            SoundManager.instance.PlaySwrod(swordAudioSource);

            //지정된 초마다 1번씩만 호출
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                //에너미 변수에 받아온 인자의 에너미를 넣는다
                Enemy enemy = target.gameObject.GetComponent<Enemy>();

                //몬스터가 칼이랑 부딪힌 위치
                Vector3 vrhitPoint = hitPos;

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
        // Debug.Log("상자 오픈");
        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            if (chest != null && chest.gameObject.tag == "Chest")
            {
                chest.gameObject.GetComponent<Chest>().Open();
                SoundManager.instance.PlaySfx("chest");
            }
        }
    }

    //아이템 줍기
    public void VrPickupItem(Collision item)
    {
        //Debug.Log("아이템 줏음");
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

        //맞는소리 재생
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

    [PunRPC]
    public override void OnPoolEnable(Vector3 pos, Quaternion rot)
    {
        base.OnPoolEnable(pos, rot);
        GameObject hud = GameObject.Find("GameUI");


        //크로스헤어 끔
        hud.transform.GetChild(1).gameObject.SetActive(false);

        //미니맵
        RectTransform miniMap = hud.transform.GetChild(0).GetComponent<RectTransform>();

        //셋팅버튼 
        RectTransform set = hud.transform.GetChild(6).GetComponent<RectTransform>();

        //인벤 버튼
        RectTransform inven = hud.transform.GetChild(7).GetComponent<RectTransform>();

        //hp바
        RectTransform hp = hud.transform.GetChild(8).GetComponent<RectTransform>();

        //미니맵 위치 조절
        miniMap.anchoredPosition = new Vector3(230, -850, 0);

        //셋팅버튼 위치 조절
        set.anchoredPosition = new Vector3(-1380, -937, 0);
        set.localScale = new Vector3(1.5f, 1.5f, 1.5f);

        //인벤 버튼 위치 조절
        inven.anchoredPosition = new Vector3(-1380, -734, 0);
        inven.localScale = new Vector3(1.5f, 1.5f, 1.5f);

        //hp바 위치 조절
        hp.anchoredPosition = new Vector3(-646, -44,0);




        VRCanvas vrCanvas = FindObjectOfType<GameUI>().GetComponent<VRCanvas>();
        vrCanvas.vrInteracter = gameObject;
        vrCanvas.transform.parent = hand;
        

        GameManager.Instance.UpdateVRUI();
    }

    private void OnDrawGizmosSelected() { }
}
