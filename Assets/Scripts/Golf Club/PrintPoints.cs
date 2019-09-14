using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using UnityEngine.UI;
using System;

//Created by Hein for the Moon VR 3.0 Project

public class PrintPoints : MonoBehaviour
{
   
    Text scoreText;


    void Start()
    {

     
        scoreText = gameObject.GetComponentInChildren<Text>();
        scoreText.enabled = false;

        if (GolfGetPoints.hit == true)
        {
            Debug.Log("ON");

            scoreText.enabled = true;
            StartCoroutine(waitForCanvas());
            scoreText.text = "Goodjob!\n " + "Your score was " + "<Color=#00FFFF>" + GolfGetPoints.hitScore + "</color>";

        }
        if (GolfGetPoints.hit == false)
        {
            Debug.Log("off");
          
            scoreText.enabled = true;
            StartCoroutine(waitForCanvas());
            scoreText.text = "Goodjob!\n " + "Your score was " + "<Color=#00FFFF>" + GolfGetPoints.hitScore + "</color>";

        }


        float dropTime = Time.time;
    double congrats = 50;


  

       
    }
    public IEnumerator waitForCanvas()
    {

        yield return new WaitForSeconds(10.0f);
        scoreText.enabled = false;

    }
}



