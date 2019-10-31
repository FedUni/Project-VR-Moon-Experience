using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using System;

public class LaserButtonTrigger : MonoBehaviour
{
    LaserAnimate laserExperiment;
    // Start is called before the first frame update
    void Start()
    {
        laserExperiment = GameObject.Find("LaserExperiment").GetComponent<LaserAnimate>();
    }

    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();
        if (startingGrabType != GrabTypes.None)
        {

            laserExperiment.laserAni(); // Animate the laser

        }
    }
}
