using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using UnityEngine.UI;

[RequireComponent( typeof( Interactable ) )]
public class CameraPolaroidController : MonoBehaviour {

    Animator anim;
    public GameObject polariod;
    public GameObject moveToPolariod;
    public Canvas flashEffect;
    private Vector3 velocity = Vector3.zero;
    private bool isPrinting = false;
    private Image flashEffectImage;
    private bool beginFlash = false;
    void Awake()
    {
        anim = gameObject.GetComponent<Animator>();
    }
    //Called every Update() while a Hand is hovering over this object
    private void HandHoverUpdate( Hand hand )
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();
        if (startingGrabType != GrabTypes.None)
        {
/*
        if ( hand.GetStandardInteractionButtonDown() || ( ( hand.controller != null ) && hand.controller.GetPressDown( Valve.VR.EVRButtonId.k_EButton_Grip ) ) )
        {
*/
            Debug.Log("Begin animating: " + gameObject.name + ", using animation: " + anim.name);
            //anim.StartPlayback();//doesnt work?
            anim.Play("CameraButtonAnim");
            cameraFlash();
            printPolaroid();

        }
        if(isPrinting)
            printPolaroid();
        if(beginFlash)
            cameraFlash();
}

    private void printPolaroid()
    {
        Vector3 currentPos = polariod.GetComponent<Transform>().position;

        if(currentPos != moveToPolariod.GetComponent<Transform>().position)
        { 
            isPrinting = true;
        }
        else 
        {
            isPrinting = false;
        }
        polariod.GetComponent<Transform>().position = Vector3.SmoothDamp(currentPos, moveToPolariod.GetComponent<Transform>().position, ref velocity, 1.0f, 0.3f);
    }

    private void cameraFlash()
    {   
        
        flashEffectImage = flashEffect.GetComponentInChildren<Image>();
        var color = flashEffectImage.color;
        if(!beginFlash)
        {
        Debug.Log(flashEffect.GetComponentInChildren<Image>().color.a);
        color.a = 1.0f;
        beginFlash = true;
        } 
        else if(beginFlash || color.a == 1.0f)
        {
            color.a = color.a - Time.deltaTime;
            Debug.Log(color.a);
        }
        if(color.a == 0.0f)
            beginFlash = false;

        flashEffectImage.color = color;//do it last

    }

}
