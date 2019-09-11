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
    //float playAmount = 0;
    public float speed = 10f;
    float animateAngle;
    bool isDoneLaunch = false;
    bool isDoneReturn = false;
    bool beenPressed = false;
    
    void Start()
    {
        GameObject catapult = GameObject.Find("Catapult");
        anim = catapult.GetComponent<Animator>(); // Get animation controller from the object
        //anim.StopPlayback();
        planetSettings = GameObject.Find("PlanetSettings"); // Get the planet settings
        AnimatorStateInfo animationState = anim.GetCurrentAnimatorStateInfo(0); // Used Get the current animation playtime
        GetComponent<AudioSource>().playOnAwake = false; // Dont play this object strait away
        GetComponent<AudioSource>().clip = launchSound; // Assign the button sound
        anim.Play("CatapultAnimate", 0, 0);
    }
    //Called every Update() while a Hand is hovering over this object
   
    private void Update()
    {
        //Debug.Log("launchAngle set to " + launchAngle);
        //Debug.Log("launchAngle set to " + launchAngle);
        if (isInterping)
        {
            Debug.Log(animateAngle);
            animationState = anim.GetCurrentAnimatorStateInfo(0);
            aniLocation = animationState.normalizedTime % 1;
            float playAmount = Mathf.Lerp(aniLocation, animateAngle, (speed * Time.deltaTime)/animateAngle);
            //Debug.Log("playAmount set to " + playAmount);
            //Debug.Log("aniLocation set to" + aniLocation);
            //Debug.Log("aniLocation set to " + aniLocation);
            //Debug.Log("launchAngle set to " + launchAngle);

            //float playAmount = Mathf.Lerp(aniLocation, 0.75f, 2f * Time.deltaTime);

            anim.Play("CatapultAnimate", 0, playAmount);
            StartCoroutine(WaitFire());
            //if (!isDoneLaunch) {
                //StartCoroutine(WaitFire());
                //isDoneLaunch = true;
                
            //}
            //Debug.Log(isDoneLaunch);

            //launchAngle = 0;
            //StartCoroutine(WaitReturn());
            //isInterping = false;

            //launchAngle = 0;
            //StartCoroutine(Wait());
            //isInterping = false;
            //launchAngle = oldAngle;

        }
        if (isDoneLaunch)
        {
            //animateAngle = 0;
            isInterping = true;
            animationState = anim.GetCurrentAnimatorStateInfo(0);
            aniLocation = animationState.normalizedTime % 1;
            float playAmount = Mathf.Lerp(aniLocation, 0, speed * Time.deltaTime);
            anim.Play("CatapultAnimate", 0, playAmount);
            StartCoroutine(WaitReturn());

        }
        //if (isDoneReturn)
        //{
            //animateAngle = launchAngle;
            
        //}

    }
    IEnumerator WaitFire()
    {
        //Debug.Log("Going to wait 3 seconds");
        yield return new WaitForSeconds(0.5f);
        //StartCoroutine(WaitReturn());
        //launchAngle = oldAngle;
        isDoneLaunch = true;
        isDoneReturn = false;
        isInterping = false;
        
    }
    IEnumerator WaitReturn()
    {
        //Debug.Log("Going to wait 3 seconds");
        yield return new WaitForSeconds(0.5f);
        //StartCoroutine(WaitReturn());
        //launchAngle = oldAngle;
        isDoneLaunch = false;
        isDoneReturn = true;
        isInterping = false;
        beenPressed = false;
    }

    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();
        if (startingGrabType != GrabTypes.None)
        {
            if (!beenPressed) {
                Debug.Log("Pressed");
                beenPressed = true;
                animateAngle = launchAngle;
                //launchAngle = oldAngle;
                isInterping = true;
            }

            
            //anim.Play("CatapultAnimate");
            //anim.Play("CatapultAnimate"); // Play The animation so the button goes down.

            if (planetSettings.GetComponent<PlanetSettings>().hasAtmos) // If this planet has an atmos the sound should be played
            {
                GetComponent<AudioSource>().Play(); // Play the sound
                GetComponent<AudioSource>().pitch = (UnityEngine.Random.value * 0.5f + 0.5f); // Change the pitch randomly to get a better effect
            }
        }
    }

    public void setLaunchAngle(float launchAngle)
    {
        this.launchAngle = launchAngle;
    }
}

