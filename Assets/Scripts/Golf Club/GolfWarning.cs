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
    void Start()
    {
        warning = gameObject.GetComponentInChildren<Canvas>();
    }

    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();
        if (startingGrabType != GrabTypes.None)
        {
            warning.enabled = true;
            StartCoroutine(waitForCanvas());
        }
        else
        {
            warning.enabled = false;
        }
       
    }
    public IEnumerator waitForCanvas()
    {

        yield return new WaitForSeconds(10.0f);
        warning.enabled = false;

    }
  
}

