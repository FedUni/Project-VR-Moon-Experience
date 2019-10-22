using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using System;

[RequireComponent(typeof(Interactable))]
// Created by Hein Reimert for The Moon VR 3.0 project
public class launch45 : MonoBehaviour
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
            script.launchAngle = 0.5f; // If this is changed the script for the control screen will need changing (CatapultInfoText)
        }

    }
    public void setAngle() // Callable method so other object can the the angle
    {
        script.launchAngle = 0.5f; // If this is changed the script for the control screen will need changing (CatapultInfoText)
    }

}
