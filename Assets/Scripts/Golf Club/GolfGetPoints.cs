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
    private Canvas golfScoreBoard;
    private Vector3 scale;
    private Vector3 originalScale;
    private Vector3 originalPostion;
    private Vector3 postion;
    private Vector3 lastPosition = Vector3.zero;
    private Vector3 clubSpeed;
    public float speed;
    private GameObject golfClub;
    bool scaleOn = false;
    bool hasScoreBoard = false;
    private List<double> userScoresArray = new List<double>();
    private Canvas GolfHighScores;
    private Text GolfHighscoreText;
    double highestUserScore;
    string TotalPoints;
    string TempPointsString;
    public bool resetScore;

    void Start()
    {

        golfClub = GameObject.Find("GolfClubFace");

        if (resetScore)
        {
            PlayerPrefs.SetString("AllPointsSave", "0"); //Reset score for testing
        }

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
            //grabbing highscores from playerprefs

            string TempPointsString = PlayerPrefs.GetString("AllPointsSave");//Loads the string
            highestUserScore = double.Parse(TempPointsString, System.Globalization.CultureInfo.InvariantCulture); //Converts to double
            userScoresArray.Add(highestUserScore); //user score system add to array

            //prints out highscores before the ball is hit.
            GolfHighScores = GameObject.Find("GolfHighScores").GetComponent<Canvas>();
            GolfHighscoreText = GameObject.Find("GolfHighScores").GetComponentInChildren<Text>();
            GolfHighscoreText.text = "<Color=red>" + "CURRENT HIGHSCORE: " + "</color>" + "<Color=#0000FF>" + highestUserScore + "</color>";
            //GolfHighScores.enabled = false;
        }               
    }

    void FixedUpdate()
    {
        clubSpeed = (golfClub.transform.position - lastPosition);
        lastPosition = golfClub.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "GolfClubFace")
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

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.collider.name == "GolfClubFace")
        {
            GetComponent<Rigidbody>().AddForce(clubSpeed * 5000);
        }
        if (collision.GetContact(0).otherCollider.name == "Terrain" && hasBeenHit == true && GameObject.Find("GolfScoreBoard") != null)
        {
            golfScoreBoard.GetComponent<RectTransform>().position = originalPostion;
            scoreText.text = "<Color=red>" + "Goodjob!\n " + "Your score was " + "</color>" + "<Color=#0000FF>" + Math.Round(Time.time - hitTime, 1) + "</color>";
            hasBeenHit = false;
            golfScoreBoard.enabled = true;
            golfScoreBoard.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
            scaleOn = true;

            //update the score board
            userScoresArray.Add(Math.Round(Time.time - hitTime, 1)); //user score system add to array
            double highestUserScore = userScoresArray.Max(); //sorts the array for the max and makes max highestUserScore


            GolfHighScores = GameObject.Find("GolfHighScores").GetComponent<Canvas>(); //finds canvas for highscores
            GolfHighscoreText = GameObject.Find("GolfHighScores").GetComponentInChildren<Text>();

            GolfHighscoreText.text = "<Color=red>" + "CURRENT HIGHSCORE: " + "</color>" + "<Color=#0000FF>" + highestUserScore + "</color>";//Prints highscore!
            GolfHighScores.enabled = true;

            string TotalPoints = highestUserScore.ToString(); //Convert to string
            PlayerPrefs.SetString("AllPointsSave", TotalPoints); //Save as a string
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




