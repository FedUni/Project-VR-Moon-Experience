using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Interactable))]
// Created by Wayland Bishop for The Moon VR 3.0 project
public class DropRigIncreaseHeight : MonoBehaviour
{
    Animator anim;
    AudioSource sound;
    AnimatorStateInfo animationState;
    GameObject planetSettings;
    GameObject DropRig;
    Text[] text;
    public double dropHeight;
    void Start()
    {
        anim = transform.parent.parent.GetComponentInParent<Animator>(); // Get animation controller from the object
        sound = transform.parent.parent.GetComponent<AudioSource>(); // Get the sound source from the correct place in the object
        AnimatorStateInfo animationState = anim.GetCurrentAnimatorStateInfo(0); // Used Get the current animation playtime
        planetSettings = GameObject.Find("PlanetSettings"); // Get the planet settings
        sound.loop = true;
        DropRig = GameObject.Find("DropRig"); // Get the drop rig
        text = DropRig.GetComponentsInChildren<Text>(); // Get all the text elements in the drop rig
    }

    private void Update()
    {
        if (anim.GetBool("heightHasPlayed"))
        {
            text[2].text = "The current drop is " + System.Math.Round(anim.GetFloat("wingHeight"), 0) + " Metres"; // Set the drop rig LCD text
            text[2].color = Color.green;
        }

    }

    //Called every Update() while a Hand is hovering over this object
    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();
        if (startingGrabType != GrabTypes.None)
        {
            anim.SetBool("heightHasPlayed" , true);
            anim.StopPlayback(); // Stop any current playback
            anim.SetFloat("Direction", 10); // Set the direction and in this case the speed
            anim.Play("DropRigHeight"); // Play the animation
            text[3].text = ""; // Clear the instructions
            if (planetSettings.GetComponent<PlanetSettings>().hasAtmos)
            {
                sound.Play(); // Play the sound effect
            }
            AnimatorStateInfo animationState = anim.GetCurrentAnimatorStateInfo(0); // Used Get the current animation playtime
            // This next section is to fix a delay between playing the animation in reverse becase the animation counter keep counting even when the animation is finished
            if (animationState.normalizedTime < 0 ) { // If is less than zero is rewound too far

                anim.Play("DropRigHeight", -1, 0); // Play it back from the start postion
            }

        }
        GrabTypes endingGrabType = hand.GetGrabEnding();
        if (endingGrabType != GrabTypes.None)
        {
            anim.SetFloat("Direction", 0); // effectilty stops the animaiton for the hight ajustment
            sound.Stop();
            dropHeight = System.Math.Truncate(animationState.normalizedTime * 100); // calaulate the hight of the drop rig based on the animation playthrough time
            text[2].text = "The current drop is " + System.Math.Round(anim.GetFloat("wingHeight"), 0) + " Meters"; // Set the drop rig LCD text
        }


    }

}