using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]
// Modified by Wayland Bishop for The Moon VR 3.0 project
public class DropRigDecreaseHeight : MonoBehaviour
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
            anim.StopPlayback();
            anim.SetFloat("Direction", -10);
            anim.Play("DropRigHeight");
            AnimatorStateInfo animationState = anim.GetCurrentAnimatorStateInfo(0);
            if (animationState.normalizedTime > 1 && anim.GetBool("heightHasPlayed"))
            {

                anim.Play("DropRigHeight", -1, 1);
            }

        }

    }

}
