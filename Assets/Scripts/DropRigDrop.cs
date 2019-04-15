using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]
// Created by Wayland Bishop for The Moon VR 3.0 project
public class DropRigDrop : MonoBehaviour
{
    Animator anim;
    AudioSource sound;
    void Start()
    {
        anim = transform.parent.parent.Find("RightArm").Find("RightVerticalPillar").Find("RightWings").Find("Drop Wings").GetComponent<Animator>(); // Get the animation controller from the correct place in the object
        sound = transform.parent.parent.Find("RightArm").Find("RightVerticalPillar").Find("RightWings").Find("Drop Wings").GetComponent<AudioSource>(); // Get the sound source from the correct place in the object 
    }
    //Called every Update() while a Hand is hovering over this object
    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();
        if (startingGrabType != GrabTypes.None)
        {
            anim.SetBool("dropHasPlayed", true); // Set the animation as played for the first time
            anim.StopPlayback(); // Stop any current playback
            anim.SetFloat("Direction", 1); // Set the direction of the aniamtion playback
            anim.Play("DropRigDropObjects"); // Play the animation
            AnimatorStateInfo animationState = anim.GetCurrentAnimatorStateInfo(0); // Get the current animation playtime
            float myTime = animationState.normalizedTime; // Get the time in nomalized time
            // This next section is to fix a delay between playing the animation in reverse becase the animation counter keep counting even when the animation is finished
            if (animationState.normalizedTime < 0) // If is less than zero is rewound too far
            {
                anim.Play("DropRigDropObjects", -1, 0); // Play it back from the start postion
            }            
            sound.pitch = (Random.value * 0.5f + 0.5f); // Change the pitch randomly to get a better effect
            sound.Play(); // Play the sound effect
            GameObject rightDroppedObject = GameObject.Find("rightDroppedObject(Clone)"); // Find the dropped objects that have been dropped form the rig
            GameObject leftDroppedObject = GameObject.Find("leftDroppedObject(Clone)");

            if (rightDroppedObject != null && leftDroppedObject != null) { // Make sure there are objects to address first
                rightDroppedObject.GetComponent<Droppable>().hasDropped = true; // Set the timer off for the dropped objects
                leftDroppedObject.GetComponent<Droppable>().hasDropped = true;
            }

        }

    }

}
