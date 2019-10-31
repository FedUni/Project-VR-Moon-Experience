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
    private List<float> userScoresArray = new List<float>();
    private Canvas GolfHighScores;
    private Text GolfHighscoreText;
    float highestUserScore;
    private float TotalPoints;
    private string TempPointsString;
    public bool resetScore;
    private RectTransform rect;
    private RectTransform highScoreRect;
    private Vector3 inPostion;
    private Vector3 outPostion;
    private bool clubHit = false;
    private GolfWarning club;

    void Start()
    {

        golfClub = GameObject.Find("GolfClubFace");//Find item Golf Club Face

        if (resetScore)
        {
            PlayerPrefs.SetFloat("AllPointsSave", 0); //Reset score for testing
        }

        if (GameObject.Find("GolfScoreBoard") != null)
        {           
            golfScoreBoard = GameObject.Find("GolfScoreBoard").GetComponent<Canvas>();
            scoreText = GameObject.Find("GolfScoreBoard").GetComponentInChildren<Text>();            
            rect = golfScoreBoard.GetComponent<RectTransform>();
            rect.localScale = new Vector3(0, 0, 0);
            golfScoreBoard.enabled = false;
            hasScoreBoard = true;

            club = GameObject.Find("GolfClubFace").GetComponent<GolfWarning>();

            float TempPointsString = PlayerPrefs.GetFloat("AllPointsSave");//Loads the string
            highestUserScore = TempPointsString;
            userScoresArray.Add(highestUserScore); //user score system add to array
            
            //prints out highscores before the ball is hit.
            GolfHighScores = GameObject.Find("GolfHighScores").GetComponent<Canvas>();
            GolfHighscoreText = GameObject.Find("GolfHighScores").GetComponentInChildren<Text>();
            highScoreRect = GolfHighScores.GetComponent<RectTransform>();
            GolfHighscoreText.text = "<Color=red>" + "CURRENT HIGHSCORE: " + "</color>" + "<Color=#0000FF>" + highestUserScore + "</color>";
        }
            
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "GolfClubFace")
        {
            //lastPosition = other.transform.position;
            hasBeenHit = true;
            isFalling = true;
        }
    }

    void FixedUpdate()
    {
        if (hasBeenHit && isFalling)
        {
            hitTime = Time.time;
            isFalling = false;
        }

        if (hasScoreBoard) // 0.4f, 0.5f, 0.2f // 0.09f, 0.09f, 0.06f
        {

            scale.x = Mathf.Clamp(scale.x, 0f, 0.4f);
            scale.y = Mathf.Clamp(scale.y, 0f, 0.5f);
            scale.z = Mathf.Clamp(scale.z, 0f, 0.2f);

            highScoreScale.x = Mathf.Clamp(highScoreScale.x, 0f, 0.07f);
            highScoreScale.y = Mathf.Clamp(highScoreScale.y, 0f, 0.07f);
            highScoreScale.z = Mathf.Clamp(highScoreScale.z, 0f, 0.07f);

            rect.localScale =   Vector3.Lerp(rect.localScale, scale, speed * Time.deltaTime);
            highScoreRect.localScale = Vector3.Lerp(highScoreRect.localScale, highScoreScale, speed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {


        if (collision.GetContact(0).otherCollider.name == "Terrain" && hasBeenHit == true && GameObject.Find("GolfScoreBoard") != null && Time.time - club.hitTime > 5f)
        {
            club.hitTime = Time.time;
            scoreText.text = "<Color=red>" + "Goodjob!\n " + "Your score was " + "</color>" + "<Color=#0000FF>" + Math.Round(Time.time - hitTime, 1) + "</color>";
            hasBeenHit = false;
            StartCoroutine(waitForScoreCanvasScaleUp()); // Start to wait function

            //update the score board
            userScoresArray.Add(Mathf.Round(Time.time - hitTime)); //user score system add to array
            float highestUserScore = userScoresArray.Max(); //sorts the array for the max and makes max highestUserScore


            GolfHighScores = GameObject.Find("GolfHighScores").GetComponent<Canvas>(); //finds canvas for highscores
            GolfHighscoreText = GameObject.Find("GolfHighScores").GetComponentInChildren<Text>();

            GolfHighscoreText.text = "<Color=red>" + "CURRENT HIGHSCORE: " + "</color>" + "<Color=#0000FF>" + highestUserScore + "</color>"; //Prints highscore!
            StartCoroutine(highScoreCanvasWiggle()); // Start to wait function

            GolfHighScores.enabled = true;
            highScoreScale = new Vector3(0.07f, 0.07f, 0.04f); // Set the scale var used to lerp to to the orginal scale the canvas was

            float TotalPoints = highestUserScore; //Convert to string
            PlayerPrefs.SetFloat("AllPointsSave", TotalPoints); //Save as a string
        }
    }

    public IEnumerator waitForScoreCanvasScaleUp()
    {
        golfScoreBoard.enabled = true;
        scale = new Vector3(0.4f,0.5f,0.2f); // Set the scale var used to lerp to to the orginal scale the canvas was
        yield return new WaitForSeconds(3.0f); // Show the scoreborad for 10 seconds
        scale = new Vector3(0, 0, 0);
    }
    public IEnumerator highScoreCanvasWiggle() // Just a visual wiggle for interest
    {
        for (int i = 0; i < 3; i++)
        {
            highScoreScale = new Vector3(0.09f, 0.09f, 0.06f);
            yield return new WaitForSeconds(0.25f);
            highScoreScale = new Vector3(0.07f, 0.07f, 0.007f);
            yield return new WaitForSeconds(0.25f);           
        }

        yield return new WaitForSeconds(3f);
        highScoreScale = new Vector3(0.0f, 0.0f, 0.0f);
    }

}




