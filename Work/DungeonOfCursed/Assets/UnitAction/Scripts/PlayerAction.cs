using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerAction : UnitAction
{
    public float attackDist = 2f;
    public float actionDist = 2f;

    [HideInInspector] public bool controllable = true;

    private PhotonView pv;
    private Animator anim;
    private CharacterController ctl;
    private MapGenerator map;

    private Collider target;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        anim = GetComponentInChildren<Animator>();
        ctl = GetComponent<CharacterController>();
        map = GameObject.FindObjectOfType<MapGenerator>();
    }
    private void Start()
    {
        isDead = false;
        roomNum = -1;

        SetSampleData();
    }
    private void Update()
    {
        UpdateRoomNum();

        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            SetLookTarget();
            ShowLookTarget();

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            bool inpAction = Input.GetMouseButtonDown(0);
#elif UNITY_ANDROID
            //>>>
#endif
            if (controllable && inpAction)
            {
                if (PhotonNetwork.inRoom) pv.RPC("Action", PhotonTargets.All);
                else Action();
            }
        }
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

    [PunRPC] void Action()
    {
        if (target == null)
        {
            anim.SetTrigger("attack");
            return;
        }

        switch (target.tag)
        {
            case "Enemy":
                {
                    anim.SetTrigger("attack");
                    AttackDamage();
                }
                break;
            case "Chest":
                //target.GetComponent<Chest>().Open();
                break;
        }
    }
    public void AttackDamage()      //call by anim
    {
        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            if (target != null && target.tag == "Enemy")
            {
                if (PhotonNetwork.inRoom)
                {
                    target.GetComponent<PhotonView>().RPC("Hit", PhotonTargets.All, stat.ATK);
                }
                else
                {
                    target.GetComponent<EnemyAction>().Hit(stat.ATK);
                }
            }
        }
    }
    [PunRPC] public void Hit(float dmg)
    {
        curHP -= dmg;

        if (curHP <= 0) Dead();
        else anim.SetTrigger("hit");
    }
    void Dead()
    {
        isDead = true;
        controllable = false;

        anim.applyRootMotion = true;
        anim.SetTrigger("dead");
        ctl.enabled = false;

        SceneManager.LoadSceneAsync("LoadingScene", LoadSceneMode.Additive);
    }

}
