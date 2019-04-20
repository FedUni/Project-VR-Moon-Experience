﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]
// Created by Wayland Bishop for The Moon VR 3.0 project
public class DropRigSpawner : MonoBehaviour
{
    public Vector3 translationOffset; // Ofset distance from the wing pair location
    public Vector3 rotationOffset; // Rotation offset from the wing pair rotation 
    Transform tFormR;
    Transform tFormL;
    public GameObject[] objectsToDrop;
    GameObject leftObject;
    GameObject rightObject;
    public bool crazyMode = false;
    void Start()
    {
        leftObject = objectsToDrop[0];
        rightObject = objectsToDrop[1];
    }
    //Called every Update() while a Hand is hovering over this object
    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();
        if (startingGrabType != GrabTypes.None)
        {
            GameObject rightDroppedObject = GameObject.Find("rightDroppedObject(Clone)"); // Find the dropped objects that have been dropped form the rig
            GameObject leftDroppedObject = GameObject.Find("leftDroppedObject(Clone)");

            if (rightDroppedObject != null && leftDroppedObject != null && !crazyMode) // This code will stop hundreds of objects from being spawned. 
            {
                Destroy(rightDroppedObject);
                Destroy(leftDroppedObject);
            }

            tFormR = transform.parent.parent.Find("RightArm").Find("RightVerticalPillar").Find("RightWings").Find("BackArm 1").GetComponent<Transform>(); // Get the tranaform XYZ of the left pair of drop wings
            tFormL = transform.parent.parent.Find("LeftArm").Find("LeftVerticalPillar").Find("LeftWings").Find("BackArm").GetComponent<Transform>(); // Get the tranaform XYZ of the right pair of drop wings
            Instantiate(leftObject); // Spawn the obejcts
            Instantiate(rightObject);
            leftObject.transform.position = new Vector3(tFormL.position.x + translationOffset.x, tFormL.position.y + translationOffset.y, tFormL.position.z + translationOffset.z); // Set the location using the drop wings as a starting location
            rightObject.transform.position = new Vector3(tFormR.position.x + translationOffset.x, tFormR.position.y + translationOffset.y, tFormR.position.z + translationOffset.z);
            leftObject.transform.rotation = Quaternion.Euler(tFormL.rotation.x + rotationOffset.x, tFormL.rotation.y + rotationOffset.y, tFormL.rotation.z + rotationOffset.z); // Set the rotation information
            rightObject.transform.rotation = Quaternion.Euler(tFormR.rotation.x + rotationOffset.x, tFormR.rotation.y + rotationOffset.y, tFormR.rotation.z + rotationOffset.z);
            rightObject.gameObject.name = "rightDroppedObject"; // Set the name for the right object 
            leftObject.gameObject.name = "leftDroppedObject"; // Set the name for the left object 
        }

    }

}
