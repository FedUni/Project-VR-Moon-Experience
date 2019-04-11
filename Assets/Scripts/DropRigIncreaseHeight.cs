using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]
// Modified by Wayland Bishop for The Moon VR 3.0 project
public class DropRigIncreaseHeight : MonoBehaviour
{

    Animator anim;

    void Start()
    {
        anim = gameObject.GetComponentInParent<Animator>();
    }
    //Called every Update() while a Hand is hovering over this object
    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();
        if (startingGrabType != GrabTypes.None)
        {
            anim.StopPlayback();
            anim.SetFloat("Direction", 1);
            anim.Play("DropRigChangeHeight");
            Debug.Log("Begin animating: " + gameObject.name + ", using animation: " + anim.name);
        }

    }

}