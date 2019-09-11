using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using System;

[RequireComponent(typeof(Interactable))]
[RequireComponent(typeof(AudioSource))]
// Created by Wayland Bishop for The Moon VR 3.0 project
public class CatapultFire : MonoBehaviour
{
    Animator anim;
    public float launchAngle;
    GameObject planetSettings;
    public AudioClip launchSound;
    bool isInterping = false;
    AnimatorStateInfo animationState;
    float aniLocation = 0;
    public float speed = 10f;
    float animateAngle;
    bool isDoneLaunch = false;
    bool beenPressed = false;
    
    void Start()
    {
        GameObject catapult = GameObject.Find("Catapult");
        anim = catapult.GetComponent<Animator>(); // Get animation controller from the object
        planetSettings = GameObject.Find("PlanetSettings"); // Get the planet settings
        AnimatorStateInfo animationState = anim.GetCurrentAnimatorStateInfo(0); // Used Get the current animation playtime
        GetComponent<AudioSource>().playOnAwake = false; // Dont play this object strait away
        GetComponent<AudioSource>().clip = launchSound; // Assign the button sound
        anim.Play("CatapultAnimate", 0, 0);
    }
    //Called every Update() while a Hand is hovering over this object
   
    private void Update()
    {
        if (isInterping)
        {
            animationState = anim.GetCurrentAnimatorStateInfo(0);
            aniLocation = animationState.normalizedTime % 1;
            float playAmount = Mathf.Lerp(aniLocation, animateAngle, (speed * Time.deltaTime)/animateAngle);

            anim.Play("CatapultAnimate", 0, playAmount);
            StartCoroutine(WaitFire());
        }
        if (isDoneLaunch)
        {
            isInterping = true;
            animationState = anim.GetCurrentAnimatorStateInfo(0);
            aniLocation = animationState.normalizedTime % 1;
            float playAmount = Mathf.Lerp(aniLocation, 0, speed * Time.deltaTime);
            anim.Play("CatapultAnimate", 0, playAmount);
            StartCoroutine(WaitReturn());

        }
    }
    IEnumerator WaitFire()
    {
        yield return new WaitForSeconds(0.5f);
        isDoneLaunch = true;
        isInterping = false;
    }
    IEnumerator WaitReturn()
    {
        yield return new WaitForSeconds(0.5f);
        isDoneLaunch = false;
        isInterping = false;
        beenPressed = false;
    }

    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();
        if (startingGrabType != GrabTypes.None)
        {
            if (!beenPressed) {
                beenPressed = true;
                animateAngle = launchAngle;
                isInterping = true;
            }

            if (planetSettings.GetComponent<PlanetSettings>().hasAtmos) // If this planet has an atmos the sound should be played
            {
                GetComponent<AudioSource>().Play(); // Play the sound
                GetComponent<AudioSource>().pitch = (UnityEngine.Random.value * 0.5f + 0.5f); // Change the pitch randomly to get a better effect
            }
        }
    }
}

