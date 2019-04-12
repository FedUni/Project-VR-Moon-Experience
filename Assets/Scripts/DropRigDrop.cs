using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]
// Modified by Wayland Bishop for The Moon VR 3.0 project
public class DropRigDrop : MonoBehaviour
{
    Animator anim;
    void Start()
    {
        anim = transform.parent.parent.Find("RightArm").Find("RightVerticalPillar").Find("RightWings").Find("Drop Wings").GetComponent<Animator>();
    }
    //Called every Update() while a Hand is hovering over this object
    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();
        if (startingGrabType != GrabTypes.None)
        {
            anim.SetBool("dropHasPlayed", true);
            anim.StopPlayback();
            anim.SetFloat("Direction", 1);
            anim.Play("DropRigDropObjects");
            AnimatorStateInfo animationState = anim.GetCurrentAnimatorStateInfo(0);
            float myTime = animationState.normalizedTime;
            if (animationState.normalizedTime < 0)
            {
                anim.Play("DropRigDropObjects", -1, 0);
            }

        }

    }

}
