using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]
// Modified by Wayland Bishop for The Moon VR 3.0 project
public class DropRigHeightStop : MonoBehaviour
{
    Animator anim;
    void Start()
    {
        anim = transform.parent.parent.GetComponentInParent<Animator>();
    }
    //Called every Update() while a Hand is hovering over this object
    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();
        if (startingGrabType != GrabTypes.None)
        {
            anim.SetFloat("Direction", 0);
            AnimatorStateInfo animationState = anim.GetCurrentAnimatorStateInfo(0);
        }

    }

}