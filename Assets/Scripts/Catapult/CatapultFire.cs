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
    public Animator anim;
    public float launchAngle = 1f;
    GameObject planetSettings;
    public AudioClip launchSound;
    bool isInterping = false;
    //AnimatorStateInfo animationState;
    float aniLocation = 0;
    public float speed = 5f;
    float animateAngle;
    bool isDoneLaunch = false;
    bool beenPressed = false;
    
    void Start()
    {
        //GameObject catapult = GameObject.Find("Catapult"); // Get the catapult
        //anim = catapult.GetComponent<Animator>(); // Get animation controller from the object
        planetSettings = GameObject.Find("PlanetSettings"); // Get the planet settings
        //AnimatorStateInfo animationState = anim.GetCurrentAnimatorStateInfo(0); // Used Get the current animation playtime
        GetComponent<AudioSource>().playOnAwake = false; // Dont play this object strait away
        GetComponent<AudioSource>().clip = launchSound; // Assign the button sound
        anim.Play("CatapultAnimate", 0, 0);
        speed = 5f;
        animateAngle = 1f;
    }
    //Called every Update() while a Hand is hovering over this object
   
    private void Update()
    {
        if (isInterping) // Only do this if we need to
        {
            AnimatorStateInfo animationState = anim.GetCurrentAnimatorStateInfo(0); // Used Get the current animation playtime
            animationState = anim.GetCurrentAnimatorStateInfo(0);
            aniLocation = animationState.normalizedTime % 1;
            StartCoroutine(EaseinFire()); // Wait for it to move
            float playAmount = Mathf.Lerp(aniLocation, animateAngle, ((speed * Time.deltaTime)/animateAngle) * 0.2f); // Lerp from were it is to where we want it to stop
            anim.Play("CatapultAnimate", 0, playAmount); // Play it 
            StartCoroutine(WaitFire()); // Wait for it to move
        }
        if (isDoneLaunch)
        {
            AnimatorStateInfo animationState = anim.GetCurrentAnimatorStateInfo(0); // Used Get the current animation playtime
            isInterping = true;
            animationState = anim.GetCurrentAnimatorStateInfo(0);
            aniLocation = animationState.normalizedTime % 1;
            float playAmount = Mathf.Lerp(aniLocation, 0, (speed * Time.deltaTime)); // Make it go from were it is back to home
            anim.Play("CatapultAnimate", 0, playAmount); // Play it
            StartCoroutine(WaitReturn()); // wait for it to get back to home

        }
    }

    IEnumerator EaseinFire()
    {
        float playAmount = Mathf.Lerp(aniLocation, animateAngle, ((speed * Time.deltaTime) / animateAngle) * 0.05f); // Lerp from were it is to where we want it to stop
        yield return new WaitForSeconds(1f);
    }
    IEnumerator WaitFire()
    {
        yield return new WaitForSeconds(0.5f);
        isDoneLaunch = true; // Lanch completed
        isInterping = false; // No longer need to interp
    }
    IEnumerator WaitReturn()
    {
        yield return new WaitForSeconds(0.5f);
        isDoneLaunch = false; // Curenlty launching
        isInterping = false; // No longer need to interp
        beenPressed = false; // No longer pressed
    }

    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();
        if (startingGrabType != GrabTypes.None)
        {
            fireCatapult(); // Fire the catapult
        }
    }

    public void fireCatapult() // Externally called method so other object can operate the catapult
    {
        if (!beenPressed)
        {
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

