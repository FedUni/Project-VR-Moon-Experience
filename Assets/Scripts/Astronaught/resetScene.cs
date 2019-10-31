using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

public class resetScene : MonoBehaviour {

    public double MAX_SCENE_TIME;
    public double TIME_REMAINING;
    public AudioClip as10SecsRemainingClip;
    public AudioClip as30SecsRemainingClip;

    private DateTime sceneStartTime;


    void Awake () {
        if(PlayerPrefs.GetInt("MaxSceneTime") == 0)
        {
            MAX_SCENE_TIME = 500;
        }
    }
    
	// Use this for initialization
	void Start () {
        sceneStartTime = DateTime.Now;
	}

    // Update is called once per frame
    void Update()
    {
        if (MAX_SCENE_TIME != 30000) { 
            TIME_REMAINING = (sceneStartTime.AddSeconds(MAX_SCENE_TIME) - DateTime.Now).TotalSeconds;
            if (DateTime.Now >= sceneStartTime.AddSeconds(MAX_SCENE_TIME))
            {
                //SteamVR_LoadLevel.Begin("LaunchScene");
            }
            //audioTimeSFXplay();//check to see if time remaining needs to be played
        }


    }

    void audioTimeSFXplay()
    {
        int tr = (int)TIME_REMAINING;

        if(tr == 32)
        {
            Debug.Log("30 seconds remaining");
            GetComponent<AudioSource>().clip = as30SecsRemainingClip;
            GetComponent<AudioSource>().Play();
            
        }

        if(tr == 12)
        {
            Debug.Log("10 Seconds Remaining");
            GetComponent<AudioSource>().clip = as10SecsRemainingClip;
            GetComponent<AudioSource>().Play();
        }
    }
}
