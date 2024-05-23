using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : UnitObject
{
    [Header("PLAYER MOVE")]
    public float speed = 7f;
    public float sprintSpeed = 10f;
    public bool jumpable = true;
    public float jumpForce = 8f;
    public float gravity = 20f;

    [Header("PLAYER LOOK")]
    public float maxCamRotateAngle = 45f;

    [Header("PLAYER ACTION")]
    public float actionDist = 2f;
    public GameObject attackEffect;

    [Header("MODEL")]
    public Transform model;
    public Transform weapon;
    public Vector3 modelOffset;
    public Vector3 modelRotate;

    [Header("CAMERA")]
    public Vector3 camOffset;

    [HideInInspector] public bool controllable = true;

    protected Animator anim;
    protected CharacterController ctl;
    protected Transform cam;
    protected MapGenerator map;
    protected PhotonView pv;
    protected GameUI ui;

    private Renderer[] renderers;
    private Renderer[] weapon_renderers;

    private bool inpJump;
    private Vector3 moveVec;

    private float camRotateX;
    private float camRotateY;

    protected bool animMove;
    protected Vector3 curPos;
    protected Quaternion curRot;

    private Collider target;
    private Vector3 hitPoint;
    private bool inpAction;

    //플레이어 오디오 소스
    private AudioSource audioSource;

    //플레이어 칼 오디오 소스
    private AudioSource swordAudioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        swordAudioSource = GetComponentInChildren<AudioSource>();
        anim = GetComponentInChildren<Animator>();
        ctl = GetComponent<CharacterController>();
        cam = FindObjectOfType<MapGenerator>().mainCam.transform;
        map = FindObjectOfType<MapGenerator>();
        pv = GetComponent<PhotonView>();
        ui = FindObjectOfType<GameUI>();

        renderers = GetComponentsInChildren<Renderer>();
        weapon_renderers = weapon.GetComponentsInChildren<Renderer>();

        ui.jumpButton.onClick.AddListener(() => inpJump = true);
        ui.actionButton.onClick.AddListener(() => inpAction = true);
    }
    private void Start()
    {
        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            roomNum = -1;

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            Cursor.lockState = CursorLockMode.Locked;
#endif

            cam.parent = transform;
            cam.localPosition = camOffset;

            foreach (var rend in renderers)
                rend.enabled = false;
            foreach (var w_rend in weapon_renderers)
                w_rend.enabled = true;

            model.parent = cam;
            model.localPosition = modelOffset;
            model.localEulerAngles = modelRotate;
        }
        Reset();
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
            //Cursor.lockState = Input.GetKey(KeyCode.LeftControl) ? CursorLockMode.None : CursorLockMode.Locked;
#endif
            if (controllable)
            {
                Move();
                Look();

                if (inpAction)
                {
                    if (PhotonNetwork.inRoom) pv.RPC("Action_All", PhotonTargets.All);
                    else Action_All();

                    inpAction = false;
                }
            }
        }
        else
        {
            UpdatePos();
            UpdateAnimMove(animMove);
        }
    }
    private void FixedUpdate()
    {
        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            if (!isDead && controllable)
            {
                ctl.Move(moveVec * Time.deltaTime);
                inpJump = false;
            }
        }
    }
    private void LateUpdate()
    {
        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            if (!isDead && controllable)
                CameraUpdate();
        }
    }
    public void Reset()
    {
        curHP = stat.HP;
        isDead = false;
        controllable = true;
        anim.applyRootMotion = false;
    }

    void Move()
    {
        if (ctl.isGrounded)
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            float inpH = Input.GetAxisRaw("Horizontal");
            float inpV = Input.GetAxisRaw("Vertical");

            float mSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : speed;

            inpJump = Input.GetKey(KeyCode.Space);

#elif UNITY_ANDROID
            float inpH = UltimateJoystick.GetHorizontalAxis("leftJoyStick");
            float inpV = UltimateJoystick.GetVerticalAxis("leftJoyStick");

            float mSpeed = speed;
#endif

            Vector3 inpDir = new Vector3(inpH, 0, inpV).normalized;
            Vector3 camDir = cam.TransformDirection(inpDir);

            UpdateAnimMove(inpDir.sqrMagnitude > 0.01f);

            if (camDir.x == 0 && camDir.z == 0)
            {
                moveVec = transform.TransformDirection(inpDir).normalized * mSpeed;
            }
            else
            {
                camDir.y = 0;
                moveVec = camDir.normalized * mSpeed;
            }

            if (jumpable && inpJump)
            {
                moveVec.y += jumpForce;
                SoundManager.instance.PlayJump(audioSource);
            }
        }
        moveVec.y -= gravity * Time.deltaTime;
    }
    void Look()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        float dMouseX = Input.GetAxisRaw("Mouse X");
        float dMouseY = Input.GetAxisRaw("Mouse Y");
#elif UNITY_ANDROID
        //float dMouseX = UltimateJoystick.GetHorizontalAxis("rightJoyStick")/5;
        //float dMouseY = UltimateJoystick.GetVerticalAxis("rightJoyStick")/5;

        //float dMouseX = Mathf.Abs(inpX) > 0.1f ? Mathf.Sign(inpX) / 5 : 0;
        //float dMouseY = Mathf.Abs(inpY) > 0.1f ? Mathf.Sign(inpY) / 5 : 0;

        float dMouseX = ui.touchPad.dTouchPoint.x / 10;
        float dMouseY = ui.touchPad.dTouchPoint.y / 10;
#endif

        camRotateY = dMouseX * 500f * GameManager.Instance.InputSensitivity * Time.deltaTime;
        camRotateX = -dMouseY * 500f * GameManager.Instance.InputSensitivity * Time.deltaTime;
    }
    void CameraUpdate()
    {
        transform.Rotate(0, camRotateY, 0);
        cam.Rotate(camRotateX, 0, 0);

        float rot = cam.localEulerAngles.x;
        if (rot < 180f && rot > maxCamRotateAngle)
            rot = maxCamRotateAngle;
        else if (rot >= 180f && rot < 360f - maxCamRotateAngle)
            rot = 360f - maxCamRotateAngle;
        cam.localEulerAngles = new Vector3(rot, 0, 0);
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
    void ShowLookTarget()
    {
        ILookEvent look;
        if (target != null && (look = target.GetComponent<ILookEvent>()) != null)
        {
            look.OnLook(this);
        }
    }

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
            case "Item":
                PickupItem();
                break;
        }
    }

    void Trade()
    {
        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            if (target != null && target.tag == "Trade")
                target.GetComponent<Trader>().Trade();
        }
    }
    void OpenChest()
    {
        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            if (target != null && target.tag == "Chest")
            {
                target.GetComponent<Chest>().Open();
                //상자 사운드 재생
                SoundManager.instance.PlaySfx("chest");
            }
        }
    }
    void PickupItem()
    {
        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            if (target != null && target.tag == "Item")
            {
                target.GetComponent<Item>().Pickup();
                //아이템 픽업 사운드 재생
                SoundManager.instance.PlaySfx("item");
            }
        }
    }

    public override void Attack()
    {
        //사운드 매니저의 플레이어히트 소리 재생 함수 실행
        SoundManager.instance.PlaySwrod(swordAudioSource);
        if (target != null && target.tag == "Enemy")
        {
            Enemy enemy = target.GetComponent<Enemy>();

            if (!enemy.isDead) MakeEffect(hitPoint);

            if (!PhotonNetwork.inRoom)
                target.GetComponent<Enemy>().OnHit(stat.ATK);
            else if (pv.isMine)
                target.GetComponent<PhotonView>().RPC("OnHit", PhotonNetwork.masterClient, stat.ATK);
        }
    }
    [PunRPC] public override void OnHit(float dmg)
    {
        if (isDead) return;

        curHP -= dmg;
        FindObjectOfType<GameUI>().hpBar.fillAmount = curHP / stat.HP;

        //사운드 매니저의 플레이어히트 소리 재생 함수 실행
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
    [PunRPC] void Hit_All()
    {
        anim.SetTrigger("hit");
    }
    [PunRPC] void Dead_All()
    {
        anim.applyRootMotion = true;
        anim.SetTrigger("dead");
    }
    protected override IEnumerator Dead(float delay)
    {
        yield return new WaitForSeconds(delay);

        yield return GameManager.Instance.MoveToScene("GameEndScene");
    }

    protected void MakeEffect(Vector3 pos)
    {
        Instantiate(attackEffect, pos, Quaternion.identity);
    }

    protected void UpdateRoomNum()
    {
        roomNum = map.FindRoom(transform.position);
    }
    
    void UpdatePos()
    {
        transform.position = Vector3.Lerp(transform.position, curPos, 3.0f * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, curRot, 3.0f * Time.deltaTime);
    }
    void UpdateAnimMove(bool isMove)
    {
        animMove = isMove;
        if (isMove) SoundManager.instance.PlayRun(audioSource);
        //weapon.gameObject.SetActive(!isMove);
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

        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            GameManager.Instance.player = this;
        }
    }
    [PunRPC] public override void OnPoolDisable()
    {
        if (PhotonNetwork.inRoom)
        {
            if (pv.isMine) pv.RPC("OnPoolDisable", PhotonTargets.Others);
            transform.parent = map.poolPos;
            gameObject.SetActive(false);
        }

        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            GameManager.Instance.player = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, actionDist);
    }
}
