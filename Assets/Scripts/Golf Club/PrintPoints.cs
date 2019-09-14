using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using UnityEngine.UI;
using System;

//Created by Hein for the Moon VR 3.0 Project

public class PrintPoints : MonoBehaviour
{
    public GolfPoints golfPoints;
    Text scoreText;


    void Start()
    {
        golfPoints = GetComponent<GolfPoints>();
        scoreText = gameObject.GetComponentInChildren<Text>();
    }

    float dropTime = Time.time;
    double congrats = 50;


    private void OnCollisionEnter(Collision collision)
    {
        

        if (GolfPoints.hit == true)
        {
            scoreText.enabled = true;
            StartCoroutine(waitForCanvas());
            scoreText.text = "Goodjob!\n " + "Your score was " + "<Color=#00FFFF>" + GolfPoints.hitScore + "</color>";

        }
    }
    public IEnumerator waitForCanvas()
    {

        yield return new WaitForSeconds(10.0f);
        scoreText.enabled = false;

    }
}



