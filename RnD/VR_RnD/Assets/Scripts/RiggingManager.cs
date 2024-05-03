using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiggingManager : MonoBehaviour
{
    public Transform leftHandIk;
    public Transform rightHandIk;
    public Transform HeadIK;

    public Transform leftHandController;
    public Transform rightHandController;
    public Transform hmd;

    public Vector3[] leftOffset;
    public Vector3[] rightOffset;
    public Vector3[] headOffset;

    public float smoothValue = 0.1f;
    public float modelHeight = 1.67f;

    private void LateUpdate()
    {
        MappingHandTransform(leftHandIk, leftHandController, true);
        MappingHandTransform(rightHandIk, rightHandController, false);
        MappingBodyTransform(HeadIK, hmd);
        MappingHeadTransform(HeadIK, hmd);
    }

    void MappingHandTransform(Transform ik, Transform con, bool isLeft)
    {
        var offset = isLeft ? leftOffset : rightOffset;
        ik.position = con.TransformPoint(offset[0]);
        ik.rotation = con.rotation * Quaternion.Euler(offset[1]);
    }

    void MappingBodyTransform(Transform ik,Transform hmd)
    {
        this.transform.position = new Vector3(hmd.position.x, hmd.position.y - modelHeight, hmd.position.z-0.12f);
        float yaw = hmd.eulerAngles.y;
        var targetRotation = new Vector3(this.transform.eulerAngles.x, yaw, this.transform.eulerAngles.z);
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.Euler(targetRotation), smoothValue);

    }


    void MappingHeadTransform(Transform ik, Transform hmd)
    {
        ik.position = hmd.TransformPoint(headOffset[0]);

        ik.rotation = hmd.rotation * Quaternion.Euler(headOffset[1]);
    }
}
