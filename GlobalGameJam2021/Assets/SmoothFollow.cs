using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
    Transform currentTarget;
    public Transform target;
    public Vector3 offset;
    public float followSpeed = 2f;
    public float rotationFollowSpeed = 2f;
    public bool matchRotation = false;

    Quaternion initialRot;
    Quaternion targetRot;

    private void Start() {
        initialRot = transform.rotation;
        targetRot = initialRot;
        currentTarget = target;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, currentTarget.position + offset, Time.deltaTime * followSpeed);
        if(matchRotation){
            targetRot = target.rotation;
        }
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * rotationFollowSpeed);
    }
}
