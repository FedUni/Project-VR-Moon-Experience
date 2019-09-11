using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using System;

[RequireComponent(typeof(Interactable))]
// Created by Hein Reimert for The Moon VR 3.0 project
public class SpeedSetting3 : MonoBehaviour
{
    CatapultFire script;
    void Start()
    {
        GameObject catapultFireButton = GameObject.Find("CatapultFireButton");
        script = catapultFireButton.GetComponentInChildren<CatapultFire>(); // Get animation controller from the object
    }
    //Called every Update() while a Hand is hovering over this object
    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();
        if (startingGrabType != GrabTypes.None)
        {
            script.speed = 500f;
        }

    }

}

