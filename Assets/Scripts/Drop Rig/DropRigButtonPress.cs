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
            //anim.Play("Button", -1, 0); // Reset the animaiton the the button clicks again
            //anim.Play("Button"); // play the button aniamtion

            if (planetSettings.GetComponent<PlanetSettings>().hasAtmos)
            { // If this planet has an atmos the sound should be played

                GetComponent<AudioSource>().Play(); // Play the sound
                GetComponent<AudioSource>().pitch = (UnityEngine.Random.value * 0.5f + 0.5f); // Change the pitch randomly to get a better effect
            }
            AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo(0);

            float playbackTime = currentState.normalizedTime % 1;
            while (playbackTime < 0.5f)
            {
                anim.Play("Button"); // play the button aniamtion
                StartCoroutine(MyCoroutine());
            }
            anim.StopPlayback();

        }
        GrabTypes endingGrabType = hand.GetGrabEnding();
        if (endingGrabType != GrabTypes.None)
        {
            anim.Play("Button"); // play the button aniamtion
        }

    }

    IEnumerator MyCoroutine()
    {
        yield return 2;    //Wait one frame
    }

}

