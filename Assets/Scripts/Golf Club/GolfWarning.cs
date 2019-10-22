using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
//Created by Hein for the Moon VR 3.0 Project
public class GolfWarning : MonoBehaviour
{
    Canvas warning;
    private Vector3 scale;
    private Vector3 originalScale;
    public float speed;
    void Start()
    {
        warning = gameObject.GetComponentInChildren<Canvas>(); // Get the canvas on the club
        originalScale = warning.GetComponent<RectTransform>().localScale; // Store the original scale
        scale = warning.GetComponent<RectTransform>().localScale; // Store the original scale
    }

    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();
        if (startingGrabType != GrabTypes.None)
        {
            warning.enabled = true; //Enables the canvas with the warning message
            warning.GetComponent<RectTransform>().localScale = new Vector3(0,0,0); // Set the scale to zero to hide it
            StartCoroutine(waitForCanvasScaleUp()); // wait for the canvas to scale up
        }
        else
        {
            warning.enabled = false; // Turn the warning off then the club is dropped
        }
       
    }

    public IEnumerator waitForCanvasScaleUp() //scales the canvas size
    {
        scale = originalScale;
        yield return new WaitForSeconds(8.0f); // Wait a bit
        scale = new Vector3(0, 0, 0);
        yield return new WaitForSeconds(1.0f); // Wait again
        warning.enabled = false; // Turn it off
    }
    private void Update()
    {
        warning.GetComponent<RectTransform>().localScale = Vector3.Lerp(warning.GetComponent<RectTransform>().localScale, scale, speed * Time.deltaTime); // Scale the canvas
    }

}

