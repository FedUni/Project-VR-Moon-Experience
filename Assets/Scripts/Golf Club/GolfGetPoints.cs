using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using UnityEngine.UI;
using System;
using System.Linq;

//Created by Hein for the Moon VR 3.0 Project

public class GolfGetPoints : MonoBehaviour
{
    private bool hasBeenHit = false;
    public float hitTime;
    private Text scoreText;
    private bool isFalling = false;
    Canvas golfScoreBoard;
    private Vector3 scale;
    private Vector3 originalScale;
    private Vector3 originalPostion;
    private Vector3 postion;
    public float speed;
    bool scaleOn = false;
    bool hasScoreBoard = false;
    List<double> userScoresArray = new List<double>();

    void Start()
    {

        if (GameObject.Find("GolfScoreBoard") != null) {
            hasScoreBoard = true;
        }

        if (hasScoreBoard)
        {
            golfScoreBoard = GameObject.Find("GolfScoreBoard").GetComponent<Canvas>();
            scoreText = GameObject.Find("GolfScoreBoard").GetComponentInChildren<Text>();
            golfScoreBoard.enabled = false;
            originalScale = golfScoreBoard.GetComponent<RectTransform>().localScale;
            scale = golfScoreBoard.GetComponent<RectTransform>().localScale;
            originalPostion = golfScoreBoard.GetComponent<RectTransform>().position;
            postion = golfScoreBoard.GetComponent<RectTransform>().position;
        }
    }


    
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "GolfClub")
        {
            hasBeenHit = true;
            isFalling = true;
        }
    }

    void Update()
    {
        if (hasBeenHit && isFalling)
        {
            hitTime = Time.time;
            isFalling = false;
        }

        if (hasScoreBoard)
        {
            golfScoreBoard.GetComponent<RectTransform>().localScale = Vector3.Lerp(golfScoreBoard.GetComponent<RectTransform>().localScale, scale, speed * Time.deltaTime);
            golfScoreBoard.GetComponent<RectTransform>().position = Vector3.Lerp(golfScoreBoard.GetComponent<RectTransform>().position, postion, speed * Time.deltaTime);
        }

        

        if (scaleOn) {
            StartCoroutine(waitForCanvasScaleUp());
        }

    }

    private void OnCollisionStay(Collision collision)
    {
        
    

            if (collision.GetContact(0).otherCollider.name == "Terrain" && hasBeenHit == true && GameObject.Find("GolfScoreBoard") != null && Math.Round(Time.time - hitTime, 1) > 1.5f)
        {
            golfScoreBoard.GetComponent<RectTransform>().position = originalPostion;
            scoreText.text = "<Color=red>"+"Goodjob!\n " + "Your score was " + "</color>" + "<Color=#0000FF>" + Math.Round(Time.time - hitTime, 1) + "</color>";
            hasBeenHit = false;
            golfScoreBoard.enabled = true;
            golfScoreBoard.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
            scaleOn = true;
            //update the score board
            userScoresArray.Add(Math.Round(Time.time - hitTime, 1)); //user score system add to array
            double highestUserScore = userScoresArray.Max(); //sorts the array for the max and makes max highestUserScore
            //TODO make a canavas and print text to canvas.

        }
    }

    public IEnumerator waitForCanvasScaleUp()
    {
        scaleOn = false;
        scale = originalScale;
        postion = originalPostion;
        yield return new WaitForSeconds(10.0f);
        postion = new Vector3(-0.5699921f, 798f, -1751f);
        yield return new WaitForSeconds(1.0f);
        golfScoreBoard.enabled = false;
    }

}




