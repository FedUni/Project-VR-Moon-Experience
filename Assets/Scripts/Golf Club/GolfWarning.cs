using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
//Created by Hein for the Moon VR 3.0 Project
public class GolfWarning : MonoBehaviour
{
    Canvas Warning1;
    Canvas Warning2;
    private Vector3 scale;
    private Vector3 originalScale;
    public float speed;
    private Vector3 inPostion;
    private Vector3 inVel;
    private Vector3 outPostion;
    private Rigidbody ballHit;
    private bool ballWasHit = false;
    public float hitPower;
    public float hitTime = Time.time;

    void Start()
    {
        Warning1 = gameObject.GetComponentsInChildren<Canvas>()[0]; // Get the canvas on the club
        Warning2 = gameObject.GetComponentsInChildren<Canvas>()[1]; // Get the canvas on the club
        originalScale = Warning1.GetComponent<RectTransform>().localScale; // Store the original scale
        scale = Warning1.GetComponent<RectTransform>().localScale; // Store the original scale
    }

    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();
        if (startingGrabType != GrabTypes.None)
        {
            Warning1.enabled = true; //Enables the canvas with the warning message
            Warning2.enabled = true; //Enables the canvas with the warning message
            Warning1.GetComponent<RectTransform>().localScale = new Vector3(0,0,0); // Set the scale to zero to hide it
            Warning2.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0); // Set the scale to zero to hide it
            StartCoroutine(waitForCanvasScaleUp()); // wait for the canvas to scale up
        }
        else
        {
            Warning1.enabled = false; // Turn the warning off then the club is dropped
            Warning2.enabled = false; // Turn the warning off then the club is dropped
        }
       
    }

    public IEnumerator waitForCanvasScaleUp() //scales the canvas size
    {
        scale = originalScale;
        yield return new WaitForSeconds(8.0f); // Wait a bit
        scale = new Vector3(0, 0, 0);
        yield return new WaitForSeconds(1.0f); // Wait again
        Warning1.enabled = false; // Turn it off
        Warning2.enabled = false; // Turn it off
    }
    private void Update()
    {
        Warning1.GetComponent<RectTransform>().localScale = Vector3.Lerp(Warning1.GetComponent<RectTransform>().localScale, scale, speed * Time.deltaTime); // Scale the canvas
        Warning2.GetComponent<RectTransform>().localScale = Vector3.Lerp(Warning2.GetComponent<RectTransform>().localScale, scale, speed * Time.deltaTime); // Scale the canvas
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.name == "GolfBall")
        {
            collision.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * hitPower, ForceMode.VelocityChange);
        }

    }

    private void OnDetachedFromHand(Hand hand) // Stop when dropped
    {
        Warning1.enabled = false; // Turn the warning off then the club is dropped
        Warning2.enabled = false; // Turn the warning off then the club is dropped
    }

}

