﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabObject : MonoBehaviour
{
    private Rigidbody rBody;
    public int forcePushPullPower;
    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public void Grab(bool shouldGrab) {
        rBody.isKinematic = shouldGrab;
    }

    public void Move(Vector3 curHandPos, Vector3 lastHandPos)
    {
        rBody.MovePosition(rBody.position + (curHandPos - lastHandPos));
    }

    public void ForcePush(Vector3 direction, int power)
    {
        rBody.AddForce(direction * power, ForceMode.Acceleration);
    }
}
