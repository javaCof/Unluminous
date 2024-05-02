using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : UnitObject
{
    public float actionDist = 2f;
    public GameObject attackEffect;

    [HideInInspector] public bool controllable = true;

    private Animator anim;
    private MapGenerator map;
    private PhotonView pv;

    private Collider target;
    private Vector3 hitPoint;
    private bool inpAction;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        map = FindObjectOfType<MapGenerator>();
        pv = GetComponent<PhotonView>();

        FindObjectOfType<GameUI>().actionButton.onClick.AddListener(() => inpAction = true);
    }
    private void Start()
    {
        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            roomNum = -1;
        }
    }
    private void Update()
    {
        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            UpdateRoomNum();
            SetLookTarget();
            ShowLookTarget();

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            inpAction = Input.GetMouseButtonDown(0);
#endif
            if (controllable && inpAction)
            {
                if (PhotonNetwork.inRoom) pv.RPC("Action_All", PhotonTargets.All);
                else Action_All();

                inpAction = false;
            }
        }
    }
    public void Reset()
    {
        curHP = stat.HP;
        isDead = false;
        controllable = true;
        anim.applyRootMotion = false;
    }

    void UpdateRoomNum()
    {
        roomNum = map.FindRoom(transform.position);
    }
    void SetLookTarget()
    {
        Vector3 camPos = Camera.main.transform.position;
        Vector3 lookVec = Camera.main.transform.forward * actionDist;

        Debug.DrawRay(Camera.main.transform.position, lookVec, Color.red, 0.01f);

        RaycastHit hit;
        Ray ray = new Ray(camPos, lookVec);

        target = null;
        hitPoint = Vector3.zero;

        if (Physics.Raycast(ray, out hit, actionDist, 1 << LayerMask.NameToLayer("LookTarget")))
        {
            target = hit.collider;
            hitPoint = hit.point;
        }
    }
    void ShowLookTarget() { }

    [PunRPC] void Action_All()
    {
        if (target == null)
        {
            anim.SetTrigger("attack");
            return;
        }

        switch (target.tag)
        {
            case "Enemy":
                anim.SetTrigger("attack");
                break;
            case "Chest":
                OpenChest();
                break;
            case "Trader":
                Trade();
                break;
        }
    }
    public override void AttackAction()
    {
        if (target != null && target.tag == "Enemy")
            MakeEffect(hitPoint);

        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            if (target != null && target.tag == "Enemy")
            {
                if (PhotonNetwork.inRoom)
                {
                    target.GetComponent<PhotonView>().RPC("Hit_Master", PhotonNetwork.masterClient, stat.ATK);
                }
                else
                {
                    target.GetComponent<Enemy>().Hit_Master(stat.ATK);
                }
            }
        }
    }
    public void Trade()
    {
        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            if (target != null && target.tag == "Trader")
                target.GetComponent<Trader>().Trade();
        }
    }
    public void OpenChest()
    {
        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            if (target != null && target.tag == "Chest")
                target.GetComponent<Chest>().Open();
        }
    }

    [PunRPC] public void Hit_Owner(float dmg)
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

            StartCoroutine(DeadPlayer());
        }
        else
        {
            if (PhotonNetwork.inRoom)
                pv.RPC("Hit_All", PhotonTargets.All);
            else Hit_All();
        }
    }
    [PunRPC] public void Hit_All()
    {
        anim.SetTrigger("hit");
    }
    [PunRPC] void Dead_All()
    {
        anim.applyRootMotion = true;
        anim.SetTrigger("dead");
    }
    IEnumerator DeadPlayer()
    {
        yield return new WaitForSeconds(1);

        SceneManager.LoadSceneAsync("GameEndScene", LoadSceneMode.Additive);
    }

    void MakeEffect(Vector3 pos)
    {
        Instantiate(attackEffect, pos, Quaternion.identity);
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(isDead);
            stream.SendNext(roomNum);
        }
        else
        {
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, actionDist);
    }
}
