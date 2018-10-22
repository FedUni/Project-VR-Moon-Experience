using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent( typeof( Interactable ) )]
public class OnClickAnimate : MonoBehaviour {

    Animator anim;

    void Awake()
    {
        anim = gameObject.GetComponent<Animator>();
    }
    //Called every Update() while a Hand is hovering over this object
    private void HandHoverUpdate( Hand hand )
    {
 			if ( hand.GetStandardInteractionButtonDown() || ( ( hand.controller != null ) && hand.controller.GetPressDown( Valve.VR.EVRButtonId.k_EButton_Grip ) ) )
			{
                Debug.Log("Begin animating: " + gameObject.name + ", using animation: " + anim.name);
                //anim.StartPlayback();
                anim.Play("LaserAnimate");
            }
    }

}
