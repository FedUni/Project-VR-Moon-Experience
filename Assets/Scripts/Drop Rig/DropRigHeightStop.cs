using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using UnityEngine.UI;

//[RequireComponent(typeof(Interactable))]
// Created by Wayland Bishop for The Moon VR 3.0 project
public class DropRigHeightStop : MonoBehaviour
{
    public double dropHeight;
    Animator anim;
    AudioSource sound;
    AnimatorStateInfo animationState;
    GameObject DropRig;
    Text[] text;
    void Start()
    {
        DropRig = GameObject.Find("DropRig"); // Get the drop rig
        anim = DropRig.GetComponentInParent<Animator>(); // Get animation controller from the object
        sound = transform.parent.parent.GetComponent<AudioSource>(); // Get the sound source from the correct place in the object
        AnimatorStateInfo animationState = anim.GetCurrentAnimatorStateInfo(0); // Used to Get the current animation playtime
        text = DropRig.GetComponentsInChildren<Text>(); // Get all the text elements in the drop rig
    }


    //Called every Update() while a Hand is hovering over this object
    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();
        if (startingGrabType != GrabTypes.None)
        {
            text[3].text = ""; // Clear the instructions
            anim.SetFloat("Direction", 0); // effectilty stops the animaiton for the hight ajustment
            sound.Stop();
            dropHeight = System.Math.Truncate(animationState.normalizedTime * 100); // calaulate the hight of the drop rig based on the animation playthrough time
            
            text[2].text = "The current drop is " + System.Math.Round(anim.GetFloat("wingHeight"), 2) + " Meters"; // Set the drop rig LCD text

            Debug.Log("The current height of the drop rig is " + System.Math.Round(anim.GetFloat("wingHeight"), 2) + " Meters"); // Output the animaiton playback percentage as a mean of knowing how hight the drop wings were set to when the user pressed stop
        }


    }

}