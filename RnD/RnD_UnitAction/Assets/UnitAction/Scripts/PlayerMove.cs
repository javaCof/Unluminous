using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private PlayerAction action;
    private PhotonView pv;
    private Animator anim;
    private CharacterController ctl;
    private Transform cam;

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
    }
    private void Start()
    {
        if (!PhotonNetwork.inRoom || pv.isMine)
        {
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
            ctl.Move(moveVec * Time.deltaTime);
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
            float inpH = Input.GetAxisRaw("Horizontal");
            float inpV = Input.GetAxisRaw("Vertical");

            float mSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : speed;

            Vector3 inpDir = new Vector3(inpH, 0, inpV).normalized;
            Vector3 camDir = cam.TransformDirection(inpDir);

            anim.SetBool("move", inpDir.sqrMagnitude > 0.01f);

            if (camDir.x == 0 && camDir.z == 0)
            {
                moveVec = transform.TransformDirection(inpDir).normalized * mSpeed;
            }
            else
            {
                camDir.y = 0;
                moveVec = camDir.normalized * mSpeed;
            }

            if (jumpable && Input.GetKey(KeyCode.Space))
            {
                moveVec.y += jumpForce;
            }
        }
        moveVec.y -= gravity * Time.deltaTime;
    }
    void Look()
    {
        float dMouseX = Input.GetAxisRaw("Mouse X");
        float dMouseY = Input.GetAxisRaw("Mouse Y");

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
