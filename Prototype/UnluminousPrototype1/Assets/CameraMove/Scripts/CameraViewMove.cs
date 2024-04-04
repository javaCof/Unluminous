using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraViewMove : MonoBehaviour
{
    public float speed = 10f;
    public float sprintSpeed = 15f;
    public float jumpForce = 8f;
    public float gravity = 20f;
    public float mouseSensitivityX = 10f;
    public float mouseSensitivityY = 10f;
    public float camRotateAngleX = 45f;

    public Vector3 camOffset;

    private PhotonView pv;
    private Animator anim;
    private CharacterController ctl;
    private Transform cam;

    private Vector3 moveVec;

    private float camRotateX;
    private float camRotateY;

    private void Awake()
    {
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

            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    private void Update()
    {
        if (!PhotonNetwork.inRoom || pv.isMine)
        {
            Move();
            Look();
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

            if (camDir.x == 0 && camDir.z == 0)
            {
                moveVec = transform.TransformDirection(inpDir).normalized * mSpeed;
            }
            else
            {
                camDir.y = 0;
                moveVec = camDir.normalized * mSpeed;
            }

            if (Input.GetKey(KeyCode.Space))
                moveVec.y += jumpForce;
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
        if (rot < 180f && rot > camRotateAngleX)
            rot = camRotateAngleX;
        else if (rot >= 180f && rot < 360f - camRotateAngleX)
            rot = 360f - camRotateAngleX;
        cam.localEulerAngles = new Vector3(rot, 0, 0);
    }
}
