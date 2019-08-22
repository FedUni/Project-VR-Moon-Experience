using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

//[RequireComponent(typeof(Interactable))]
// Created by Wayland Bishop for The Moon VR 3.0 project
public class DropRigReset : MonoBehaviour
{

    Animator anim;
    AudioSource sound;
    GameObject planetSettings;
    void Start()
    {
        anim = GameObject.Find("Drop Wings").GetComponent<Animator>(); // Get animation controller from the object
        sound = GameObject.Find("Drop Wings").GetComponent<AudioSource>(); // Get the sound source from the correct place in the object
        planetSettings = GameObject.Find("PlanetSettings"); // Get the planet settings
    }

    public void resetPressed()
    {
        anim.StopPlayback(); // Stop any current playback
        anim.SetFloat("Direction", -1); // Set the direction to reverse the animation
        anim.Play("DropRigDropObjects"); // Play the animation (in the case backwards)
        if (planetSettings.GetComponent<PlanetSettings>().hasAtmos)
        { // If this planet has an atmos the sound should be played

            GetComponent<AudioSource>().Play(); // Play the sound
            GetComponent<AudioSource>().pitch = (Random.value * 0.5f + 0.5f); // Change the pitch randomly to get a better effect

        }
        AnimatorStateInfo animationState = anim.GetCurrentAnimatorStateInfo(0); // Get the current animation playtime
        float myTime = animationState.normalizedTime; // Get the time in nomalized time
                                                      // This next section is to fix a delay between playing the animation in reverse becase the animation counter keep counting even when the animation is finished
        if (animationState.normalizedTime > 1 && anim.GetBool("dropHasPlayed")) // If is more then 1 its played too far past the end also need to make sure it has been played at least once
        {
            anim.Play("DropRigDropObjects", -1, 1); // Play it from the start of the animation
        }
    }
    //Called every Update() while a Hand is hovering over this object
    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();
        if (startingGrabType != GrabTypes.None)
        {
            resetPressed();
        }

    }

}

