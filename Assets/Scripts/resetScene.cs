using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class resetScene : MonoBehaviour {

    public double MAX_SCENE_TIME;
    public double TIME_REMAINING;
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
             
	}
}
