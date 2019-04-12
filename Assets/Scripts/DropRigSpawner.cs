using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]
// Modified by Wayland Bishop for The Moon VR 3.0 project
public class DropRigSpawner : MonoBehaviour
{
    public Vector3 translationOffset;
    public Vector3 rotationOffset;
    public float spaceBetween;
    Transform tFormR;
    Transform tFormL;
    public GameObject leftObject;
    public GameObject rightObject;
    void Start()
    {
        
    }
    //Called every Update() while a Hand is hovering over this object
    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();
        if (startingGrabType != GrabTypes.None)
        {
            tFormR = transform.parent.parent.Find("RightArm").Find("RightVerticalPillar").Find("RightWings").Find("BackArm 1").GetComponent<Transform>();
            tFormL = transform.parent.parent.Find("LeftArm").Find("LeftVerticalPillar").Find("LeftWings").Find("BackArm").GetComponent<Transform>();
            Instantiate(leftObject);
            Instantiate(rightObject);
            leftObject.transform.position = new Vector3(tFormL.position.x + translationOffset.x, tFormL.position.y + translationOffset.y, tFormL.position.z + translationOffset.z);
            rightObject.transform.position = new Vector3(tFormR.position.x + translationOffset.x, tFormR.position.y + translationOffset.y, tFormR.position.z + translationOffset.z);
            leftObject.transform.rotation = Quaternion.Euler(tFormL.rotation.x + rotationOffset.x, tFormL.rotation.y + rotationOffset.y, tFormL.rotation.z + rotationOffset.z);
            rightObject.transform.rotation = Quaternion.Euler(tFormR.rotation.x + rotationOffset.x, tFormR.rotation.y + rotationOffset.y, tFormR.rotation.z + rotationOffset.z);
        }

    }

}
