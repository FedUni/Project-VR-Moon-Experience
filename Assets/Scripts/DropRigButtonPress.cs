using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]
// Modified by Wayland Bishop for The Moon VR 3.0 project
public class DropRigButtonPress : MonoBehaviour
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
            anim.Play(gameObject.name, -1, 0);
            anim.Play(gameObject.name);
            //Debug.Log("Begin animating: " + gameObject.name + ", using animation: " + anim.name);
        }

    }

}

