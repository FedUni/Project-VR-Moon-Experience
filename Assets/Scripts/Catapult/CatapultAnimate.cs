using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent( typeof( Interactable ) )]
public class CatapultAnimate : MonoBehaviour {

    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        //Debug.Log("Got animator" + anim);
    }
    //Called every Update() while a Hand is hovering over this object
    private void HandHoverUpdate(Hand hand)
    {
        //Debug.Log("Hovering");
        GrabTypes startingGrabType = hand.GetGrabStarting();
        if (startingGrabType != GrabTypes.None) {
            /*        
                    if ( hand.GetStandardInteractionButtonDown() || ( ( hand.controller != null ) && hand.controller.GetPressDown( Valve.VR.EVRButtonId.k_EButton_Grip ) ) )
                    {
            */
            //Debug.Log("Grabbing");
            //Debug.Log("Begin animating: " + gameObject.name + ", using animation: " + anim.name);
            //anim.StartPlayback();
            anim.Play("CatapultAnimate", 0, 0);
        }

    }

}
