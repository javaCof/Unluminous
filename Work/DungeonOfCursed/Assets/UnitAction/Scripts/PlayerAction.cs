using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerAction : UnitAction
{
    public float attackDist = 2f;
    public float actionDist = 2f;

    [HideInInspector] public bool controllable = true;

    private Animator anim;
    private CharacterController ctl;
    private MapGenerator map;
    private PhotonView pv;

    private Button actionBtn;

    private Collider target;

    private bool inpAction;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        ctl = GetComponent<CharacterController>();
        map = GameObject.FindObjectOfType<MapGenerator>();
        pv = GetComponent<PhotonView>();

        //actionBtn = GameObject.Find("ActionBtn").GetComponent<Button>();
        //actionBtn.onClick.AddListener(() => inpAction = true);
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

#if UNITY_STANDALONE_WIN
            inpAction = Input.GetMouseButtonDown(0);
#elif UNITY_ANDROID
#endif
            if (controllable && inpAction)
            {
                if (PhotonNetwork.inRoom) pv.RPC("Action_All", PhotonTargets.All);
                else Action_All();

                inpAction = false;
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

    [PunRPC]
    void Action_All()
    {
        if (target == null)
        {
            anim.SetTrigger("attack");
            return;
        }

        switch (target.tag)
        {
            case "Enemy":
                Attack_Owner();
                break;
            case "Chest":
                //target.GetComponent<Chest>().Open();
                break;
        }
    }
    public void Attack_Owner()      //call by anim
    {
        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            if (target != null && target.tag == "Enemy")
            {
                if (PhotonNetwork.inRoom)
                {
                    target.GetComponent<PhotonView>().RPC("Hit_All", PhotonTargets.All, stat.ATK);
                }
                else
                {
                    target.GetComponent<EnemyAction>().Hit_All(stat.ATK);
                }
            }
        }
    }
    [PunRPC] public void Hit_All(float dmg)
    {
        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            curHP -= dmg;

            if (curHP <= 0)
            {
                if (PhotonNetwork.inRoom)
                {
                    pv.RPC("Dead_All", PhotonTargets.All);
                }
                else
                {
                    Dead_All();
                }

                return;
            }
        }
        anim.SetTrigger("hit");
    }
    [PunRPC] void Dead_All()
    {
        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            isDead = true;
            controllable = false;
            ctl.enabled = false;
            StartCoroutine(DeadOwner());
        }

        anim.applyRootMotion = true;
        anim.SetTrigger("dead");
    }
    IEnumerator DeadOwner()
    {
        yield return new WaitForSeconds(1);

        SceneManager.LoadSceneAsync("LoadingScene", LoadSceneMode.Additive);
    }
}
