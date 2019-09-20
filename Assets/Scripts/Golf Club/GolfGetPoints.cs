using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    private Vector3 highScoreScale;
    private Vector3 newScale = new Vector3(0, 0, 0);
    private Vector3 originalScale;
    private Vector3 lastPosition = Vector3.zero;
    private Vector3 clubSpeed;
    public float speed;
    private GameObject golfClub;
    private bool scaleOn = false;
    private bool hasScoreBoard = false;
    private List<double> userScoresArray = new List<double>();
    private Canvas GolfHighScores;
    private Text GolfHighscoreText;
    double highestUserScore;
    private string TotalPoints;
    private string TempPointsString;
    public bool resetScore;
    private RectTransform rect;
    private RectTransform highScoreRect;

    void Start()
    {

        golfClub = GameObject.Find("GolfClubFace");

        if (resetScore)
        {
            PlayerPrefs.SetString("AllPointsSave", "0"); //Reset score for testing
        }

        if (GameObject.Find("GolfScoreBoard") != null)
        {           
            golfScoreBoard = GameObject.Find("GolfScoreBoard").GetComponent<Canvas>();
            scoreText = GameObject.Find("GolfScoreBoard").GetComponentInChildren<Text>();            
            rect = golfScoreBoard.GetComponent<RectTransform>();
            originalScale = rect.localScale;
            rect.localScale = newScale;
            golfScoreBoard.enabled = false;
            hasScoreBoard = true;

            string TempPointsString = PlayerPrefs.GetString("AllPointsSave");//Loads the string
            highestUserScore = double.Parse(TempPointsString, System.Globalization.CultureInfo.InvariantCulture); //Converts to double
            userScoresArray.Add(highestUserScore); //user score system add to array
            
            //prints out highscores before the ball is hit.
            GolfHighScores = GameObject.Find("GolfHighScores").GetComponent<Canvas>();
            GolfHighscoreText = GameObject.Find("GolfHighScores").GetComponentInChildren<Text>();
            highScoreRect = GolfHighScores.GetComponent<RectTransform>();
            GolfHighscoreText.text = "<Color=red>" + "CURRENT HIGHSCORE: " + "</color>" + "<Color=#0000FF>" + highestUserScore + "</color>";
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
            rect.localScale = Vector3.Lerp(rect.localScale, scale, speed * Time.deltaTime);
            highScoreRect.localScale = Vector3.Lerp(highScoreRect.localScale, highScoreScale, speed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.collider.name == "GolfClubFace")
        {
            GetComponent<Rigidbody>().AddForce(clubSpeed * 10000);
        }
        if (collision.GetContact(0).otherCollider.name == "Terrain" && hasBeenHit == true && GameObject.Find("GolfScoreBoard") != null)
        {
            scoreText.text = "<Color=red>" + "Goodjob!\n " + "Your score was " + "</color>" + "<Color=#0000FF>" + Math.Round(Time.time - hitTime, 1) + "</color>";
            hasBeenHit = false;
            StartCoroutine(waitForScoreCanvasScaleUp()); // Start to wait function

            //update the score board
            userScoresArray.Add(Math.Round(Time.time - hitTime, 1)); //user score system add to array
            double highestUserScore = userScoresArray.Max(); //sorts the array for the max and makes max highestUserScore


            GolfHighScores = GameObject.Find("GolfHighScores").GetComponent<Canvas>(); //finds canvas for highscores
            GolfHighscoreText = GameObject.Find("GolfHighScores").GetComponentInChildren<Text>();

            GolfHighscoreText.text = "<Color=red>" + "CURRENT HIGHSCORE: " + "</color>" + "<Color=#0000FF>" + highestUserScore + "</color>";//Prints highscore!
            StartCoroutine(highScoreCanvasWiggle()); // Start to wait function

            GolfHighScores.enabled = true;
            highScoreScale = new Vector3(0.07f, 0.07f, 0.04f); // Set the scale var used to lerp to to the orginal scale the canvas was

            string TotalPoints = highestUserScore.ToString(); //Convert to string
            PlayerPrefs.SetString("AllPointsSave", TotalPoints); //Save as a string
        }
    }

    public IEnumerator waitForScoreCanvasScaleUp()
    {
        golfScoreBoard.enabled = true;
        scale = new Vector3(0.4f,0.5f,0.2f); // Set the scale var used to lerp to to the orginal scale the canvas was
        yield return new WaitForSeconds(5.0f); // Show the scoreborad for 10 seconds
        scale = newScale;
    }
    public IEnumerator waitForHighScoreCanvasScaleUp()
    {
        GolfHighScores.enabled = true;
        scale = new Vector3(0.4f, 0.5f, 0.2f); // Set the scale var used to lerp to to the orginal scale the canvas was
        yield return new WaitForSeconds(5.0f); // Show the scoreborad for 10 seconds
        scale = newScale;
    }
    public IEnumerator highScoreCanvasWiggle()
    {
        for (int i = 0; i < 10; i++)
        {
            highScoreScale = new Vector3(0.09f, 0.09f, 0.06f);
            yield return new WaitForSeconds(1f);
            highScoreScale = new Vector3(0.07f, 0.07f, 0.04f);
            yield return new WaitForSeconds(1f);           
        }

        yield return new WaitForSeconds(30f);
        highScoreScale = new Vector3(0.0f, 0.0f, 0.0f);
    }
}




