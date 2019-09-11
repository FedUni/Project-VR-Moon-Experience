using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;
using UnityEngine.UI;

[RequireComponent( typeof( Interactable ) )]
public class CameraPolaroidController : MonoBehaviour {

    Animator anim;
    public GameObject polariodPlaceHolder;
    public GameObject polariodPrefab;
    //    public MeshRenderer photoMeshRenderer;
    //    public GameObject moveToPolariod;
    //    public Canvas flashEffect;
    public Animator flashEffectAnimator;
    public RenderTexture renderTexture;
    private GameObject polariod;
//    private Vector3 velocity = Vector3.zero;
//    private bool isPrinting = false;
//    private Image flashEffectImage;
//    private bool beginFlash = false;
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
            //anim.Play("CameraButtonAnim");

            Texture2D tex = new Texture2D(1024, 1024, TextureFormat.RGB24, false);
            RenderTexture.active = renderTexture;
            tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            tex.Apply();

            polariod = Instantiate(polariodPrefab, polariodPlaceHolder.transform);

            polariod.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material.mainTexture = tex;

            byte[] bytes = tex.EncodeToPNG();
            System.IO.File.WriteAllBytes("VR-Moon-Polaroid_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png", bytes);


            anim.SetBool("print", true);
            flashEffectAnimator.SetBool("flash", true);
//           cameraFlash();
//            printPolaroid();

        }
//        if(isPrinting)
//            printPolaroid();
//        if(beginFlash)
//            cameraFlash();
}

/*
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
*/
    public void printEnded()
    {
        anim.SetBool("print", false);

        Transform polaroidTransform = polariod.GetComponent<Transform>();
        Vector3 worldPos = polaroidTransform.position;
        polariod.GetComponent<Transform>().SetParent(null);
        polaroidTransform.position = worldPos;

        Throwable throwable = polariod.AddComponent<Throwable>();
        throwable.onPickUp = new UnityEvent();
        throwable.onDetachFromHand = new UnityEvent();
        //throwable.onHeldUpdate = new UnityEvent();
        polariod.GetComponent<VelocityEstimator>().estimateOnAwake = true;
        Interactable interactable = polariod.GetComponent<Interactable>();
        interactable.hideHighlight = new GameObject[0];
        interactable.hideHandOnAttach = false;
        interactable.handFollowTransform = false;
        interactable.useHandObjectAttachmentPoint = false;
        //polariod.GetComponent<Throwable>().releaseVelocityStyle = ReleaseStyle.ShortEstimation;

        polariod.GetComponent<Rigidbody>().isKinematic = false;

    }
/*
    private void cameraFlash()
    {   
        
        flashEffectImage = flashEffect.GetComponentInChildren<Image>();
        var color = flashEffectImage.color;
        if(!beginFlash)
        {
//        Debug.Log(flashEffect.GetComponentInChildren<Image>().color.a);
        color.a = 1.0f;
        beginFlash = true;
        } 
        else if(beginFlash || color.a == 1.0f)
        {
            color.a = color.a - Time.deltaTime;
//            Debug.Log(color.a);
        }
        if(color.a <= 0.0f)
            beginFlash = false;

        flashEffectImage.color = color;//do it last

    }
*/
}
