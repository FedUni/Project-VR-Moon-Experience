using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]
// Modified by Wayland Bishop for The Moon VR 3.0 project
public class DropRigReset : MonoBehaviour
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
            anim.StopPlayback();
            anim.SetFloat("Direction", -1);
            anim.Play("DropRigDropObjects");


            AnimatorStateInfo animationState = anim.GetCurrentAnimatorStateInfo(0);
            //AnimatorClipInfo[] myAnimatorClip = anim.GetCurrentAnimatorClipInfo(0); // This output the 
            float myTime = animationState.normalizedTime;
            Debug.Log(myTime);
            if (animationState.normalizedTime > 1 && anim.GetBool("dropHasPlayed"))
            {

                anim.Play("DropRigDropObjects", -1, 1);
            }

            //Debug.Log("Begin animating: " + gameObject.name + ", using animation: " + anim.name);
        }

    }

}

