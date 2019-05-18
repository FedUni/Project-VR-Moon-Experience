using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using System;

[RequireComponent(typeof(Interactable))]
[RequireComponent(typeof(AudioSource))]
// Created by Wayland Bishop for The Moon VR 3.0 project
public class DropRigButtonPress : MonoBehaviour
{
    Animator anim;
    GameObject planetSettings;
    public AudioClip buttonSound;
    void Start()
    {
        anim = gameObject.GetComponentInParent<Animator>(); // Get animation controller from the object
        planetSettings = GameObject.Find("PlanetSettings"); // Get the planet settings
        GetComponent<AudioSource>().playOnAwake = false; // Dont play this object strait away
        GetComponent<AudioSource>().clip = buttonSound; // Assign the button sound
    }
    //Called every Update() while a Hand is hovering over this object
    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();
        if (startingGrabType != GrabTypes.None)
        {
            anim.Play("ButtonDown"); // Play The animation so the button goes down.

            if (planetSettings.GetComponent<PlanetSettings>().hasAtmos) // If this planet has an atmos the sound should be played
            { 

                GetComponent<AudioSource>().Play(); // Play the sound
                GetComponent<AudioSource>().pitch = (UnityEngine.Random.value * 0.5f + 0.5f); // Change the pitch randomly to get a better effect
            }
            
        }

        GrabTypes endingGrabType = hand.GetGrabEnding();
        if (endingGrabType != GrabTypes.None)
        {
            anim.Play("ButtonUp"); // play the button aniamtion so the button goes up
        }
    }

}

