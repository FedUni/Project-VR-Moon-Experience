using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent( typeof( Interactable ) )]
public class CameraPolaroidController : MonoBehaviour {

    Animator anim;
    public GameObject polariod;
    public GameObject moveToPolariod;
    private Vector3 velocity = Vector3.zero;
    private bool isPrinting = false;
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
                anim.Play("CameraButtonAnim");
                printPolaroid();
                //TODO: Add method to output polariod image
            }
            if(isPrinting)
                printPolaroid();
    }

    private void printPolaroid()
    {
        Vector3 currentPos = polariod.GetComponent<Transform>().position;
        //polariod.GetComponent<Transform>().position = Vector3.SlerpUnclamped(new Vector3(10.0f, 0.0f, 10.0f), polariod.GetComponent<Transform>().position, 1.0f);
        //polariod.GetComponent<Transform>().position = Vector3.Lerp(currentPos, moveToPolariod.GetComponent<Transform>().position, 1.0f);
        if(currentPos != moveToPolariod.GetComponent<Transform>().position)
           { 
               isPrinting = true;
           }
           else {
                isPrinting = false;
           }
        polariod.GetComponent<Transform>().position = Vector3.SmoothDamp(currentPos, moveToPolariod.GetComponent<Transform>().position, ref velocity, 1.0f, 0.3f);
    }

}
