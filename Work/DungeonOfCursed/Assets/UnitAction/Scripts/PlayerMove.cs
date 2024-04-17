using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    public float speed = 7f;
    public float sprintSpeed = 10f;
    public bool jumpable = true;
    public float jumpForce = 8f;
    public float gravity = 20f;
    public float mouseSensitivityX = 10f;
    public float mouseSensitivityY = 10f;
    public float maxCamRotateAngle = 45f;

    public Vector3 camOffset;

    private Button jumpBtn;

    private PlayerAction action;
    
    private Animator anim;
    private CharacterController ctl;
    private Transform cam;
    private PhotonView pv;

    private bool inpJump;
    private Vector3 moveVec;

    private float camRotateX;
    private float camRotateY;

    private bool animMove;
    private Vector3 curPos;
    private Quaternion curRot;

    private void Awake()
    {
        action = GetComponent<PlayerAction>();
        pv = GetComponent<PhotonView>();
        anim = GetComponentInChildren<Animator>();
        ctl = GetComponent<CharacterController>();
        cam = Camera.main.transform;

        //jumpBtn = GameObject.Find("JumpBtn").GetComponent<Button>();
        //jumpBtn.onClick.AddListener(() => inpJump = true);
    }
    private void Start()
    {
        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            Cursor.lockState = CursorLockMode.Locked;
            
            cam.parent = transform;
            cam.localPosition = camOffset;
        }
    }
    private void Update()
    {
        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            if (action.controllable)
            {
                Move();
                Look();
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
            if (!action.isDead)
            {
                ctl.Move(moveVec * Time.deltaTime);
                inpJump = false;
            }
        }
    }
    private void LateUpdate()
    {
        if (!PhotonNetwork.inRoom || pv.isMine)
            CameraUpdate();
    }

    void Move()
    {
        if (ctl.isGrounded)
        {
#if UNITY_STANDALONE_WIN
            float inpH = Input.GetAxisRaw("Horizontal");
            float inpV = Input.GetAxisRaw("Vertical");

            float mSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : speed;
            inpJump = Input.GetKey(KeyCode.Space);
#elif UNITY_ANDROID || UNITY_EDITOR
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
            }
        }
        moveVec.y -= gravity * Time.deltaTime;
    }
    void Look()
    {
#if UNITY_STANDALONE_WIN
        float dMouseX = Input.GetAxisRaw("Mouse X");
        float dMouseY = Input.GetAxisRaw("Mouse Y");
#elif UNITY_ANDROID
        float dMouseX = UltimateJoystick.GetHorizontalAxis( "rightJoyStick" )/5;
        float dMouseY = UltimateJoystick.GetVerticalAxis( "rightJoyStick" )/5;
#endif
        camRotateY = dMouseX * 100f * mouseSensitivityX * Time.deltaTime;
        camRotateX = -dMouseY * 100f * mouseSensitivityY * Time.deltaTime;
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
        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
            curRot = (Quaternion)stream.ReceiveNext();
            animMove = (bool)stream.ReceiveNext();
        }
    }
}
