using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

//[RequireComponent(typeof(Interactable))]
// Created by Wayland Bishop for The Moon VR 3.0 project
public class DropRigDrop : MonoBehaviour
{
    Animator anim;
    AudioSource sound;
    GameObject planetSettings;
    Light[] panelLights;
    GameObject DropRig;
    void Start()
    {
        DropRig = GameObject.Find("DropRig"); // Get the drop rig
                                              //anim = transform.parent.parent.Find("RightArm").Find("RightVerticalPillar").Find("RightWings").Find("Drop Wings").GetComponent<Animator>(); // Get the animation controller from the correct place in the object
                                              //sound = transform.parent.parent.Find("RightArm").Find("RightVerticalPillar").Find("RightWings").Find("Drop Wings").GetComponent<AudioSource>(); // Get the sound source from the correct place in the object
        if (DropRig != null)
        {
            anim = GameObject.Find("Drop Wings").GetComponent<Animator>(); // Get the animation controller from the correct place in the object
            sound = GameObject.Find("Drop Wings").GetComponent<AudioSource>(); // Get the sound source from the correct place in the object
            planetSettings = GameObject.Find("PlanetSettings"); // Get the planet settings

            panelLights = DropRig.GetComponentsInChildren<Light>(); // Get all the light elements in the drop rig
        }
    }

    public void dropPressed() {
        panelLights[2].color = Color.red;
        anim.SetBool("dropHasPlayed", true); // Set the animation as played for the first time
        anim.StopPlayback(); // Stop any current playback
        anim.SetFloat("Direction", 5); // Set the direction of the aniamtion playback
        anim.Play("DropRigDropObjects"); // Play the animation
        AnimatorStateInfo animationState = anim.GetCurrentAnimatorStateInfo(0); // Get the current animation playtime
        float myTime = animationState.normalizedTime; // Get the time in nomalized time
                                                      // This next section is to fix a delay between playing the animation in reverse becase the animation counter keep counting even when the animation is finished
        if (animationState.normalizedTime < 0) // If is less than zero is rewound too far
        {
            anim.Play("DropRigDropObjects", -1, 0); // Play it back from the start postion
        }
        if (planetSettings.GetComponent<PlanetSettings>().hasAtmos)
        { // If this planet has an atmos the sound should be played

            GetComponent<AudioSource>().Play(); // Play the sound
            GetComponent<AudioSource>().pitch = (Random.value * 0.5f + 0.5f); // Change the pitch randomly to get a better effect

        }
        GameObject rightDroppedObject = GameObject.Find("rightDroppedObject(Clone)"); // Find the dropped objects that have been dropped form the rig
        GameObject leftDroppedObject = GameObject.Find("leftDroppedObject(Clone)");

        if (rightDroppedObject != null && leftDroppedObject != null)
        { // Make sure there are objects to address first
            rightDroppedObject.GetComponent<Droppable>().hasDropped = true; // Set the timer off for the dropped objects
            leftDroppedObject.GetComponent<Droppable>().hasDropped = true;
        }
    }
    //Called every Update() while a Hand is hovering over this object
    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();
        if (startingGrabType != GrabTypes.None)
        {
            dropPressed();

        }

    }

}
