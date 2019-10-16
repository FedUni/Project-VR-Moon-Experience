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
        warning = gameObject.GetComponentInChildren<Canvas>();
        originalScale = warning.GetComponent<RectTransform>().localScale;
        scale = warning.GetComponent<RectTransform>().localScale;
    }

    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();
        if (startingGrabType != GrabTypes.None)
        {
            warning.enabled = true; //Enables the canvas with the warning message
            warning.GetComponent<RectTransform>().localScale = new Vector3(0,0,0);
            StartCoroutine(waitForCanvasScaleUp());
        }
        else
        {
            warning.enabled = false;
        }
       
    }

    public IEnumerator waitForCanvasScaleUp() //scales the canvas size
    {
        scale = originalScale;
        yield return new WaitForSeconds(8.0f);
        scale = new Vector3(0, 0, 0);
        yield return new WaitForSeconds(1.0f);
        warning.enabled = false;
    }
    private void Update()
    {
        warning.GetComponent<RectTransform>().localScale = Vector3.Lerp(warning.GetComponent<RectTransform>().localScale, scale, speed * Time.deltaTime);
    }

}

