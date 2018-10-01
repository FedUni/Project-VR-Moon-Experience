using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class resetScene : MonoBehaviour {

    public double MAX_SCENE_TIME;
    public double TIME_REMAINING;
    public AudioSource as10SecsRemaining;
    public AudioSource as30SecsRemaining;

    private DateTime sceneStartTime;



    
	// Use this for initialization
	void Start () {
        sceneStartTime = DateTime.Now;
	}
	
	// Update is called once per frame
	void Update () {
        //check if scene time is different to max time allowed. 
        TIME_REMAINING = (sceneStartTime.AddSeconds(MAX_SCENE_TIME) - DateTime.Now).TotalSeconds;
        if(DateTime.Now >= sceneStartTime.AddSeconds(MAX_SCENE_TIME))
        {
            Debug.Log("We have reached the max scene time.");
            //Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene("moonSceneMenu");
        }
        audioTimeSFXplay();//check to see if time remaining needs to be played


    }

    void audioTimeSFXplay()
    {
        int tr = (int)TIME_REMAINING;

        if(tr == 32)
        {
            Debug.Log("30 seconds remaining");
            as30SecsRemaining.Play();
        }

        if(tr == 12)
        {
            Debug.Log("10 Seconds Remaining");
            as10SecsRemaining.Play();
        }
    }
}
