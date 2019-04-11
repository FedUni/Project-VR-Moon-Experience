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
        anim = gameObject.GetComponent<Animator>();
    }
    //Called every Update() while a Hand is hovering over this object
    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();
        if (startingGrabType != GrabTypes.None)
        {
            //float animationTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
            anim.SetFloat("Direction", 0);
            //anim.StopPlayback();

            //anim.Play("DropRigChangeHeight", 0 , 0);
            Debug.Log("Stop animating: " + gameObject.name + ", using animation: " + anim.name);
        }

    }

}