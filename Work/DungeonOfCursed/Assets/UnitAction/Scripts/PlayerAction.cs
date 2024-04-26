using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerAction : UnitAction, IPhotonPoolObject
{
    public float actionDist = 2f;

    [HideInInspector] public bool controllable = true;

    private Animator anim;
    private MapGenerator map;
    private PhotonView pv;

    private Collider target;

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
            isDead = false;
            roomNum = -1;

            SetSampleData();
        }
    }
    private void Update()
    {
        UpdateRoomNum();

        if (!PhotonNetwork.inRoom || pv.isMine)
        {
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

    //임시 데이터 설정
    void SetSampleData()
    {
        stat = new UnitStatInfo();
        stat.HP = 30;
        stat.ATK = 5;
        stat.DEF = 5;
        stat.SPD = 1;
        curHP = stat.HP;
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

        if (Physics.Raycast(ray, out hit, actionDist, 1 << LayerMask.NameToLayer("LookTarget")))
            target = hit.collider;
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
                break;
        }
    }
    public override void AttackAction()
    {
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
                    target.GetComponent<EnemyAction>().Hit_Master(stat.ATK);
                }
            }
        }
    }

    [PunRPC] public void Hit_Owner(float dmg)
    {
        if (isDead) return;

        curHP -= dmg;

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

    [PunRPC] public void OnPoolCreate()
    {
        if (pv.isMine)
            pv.RPC("OnPoolCreate", PhotonTargets.Others);

        transform.parent = map.poolPos;
        gameObject.SetActive(false);
    }
    [PunRPC] public void OnPoolEnable(Vector3 pos, Quaternion rot)
    {
        if (pv.isMine)
            pv.RPC("OnPoolEnable", PhotonTargets.Others, pos, rot);

        transform.parent = map.objectPos;
        transform.localPosition = pos;
        transform.localRotation = rot;
        gameObject.SetActive(true);
    }
    [PunRPC] public void OnPoolDisable()
    {
        if (pv.isMine)
            pv.RPC("OnPoolDisable", PhotonTargets.Others);

        transform.parent = map.poolPos;
        gameObject.SetActive(false);
    }
}
