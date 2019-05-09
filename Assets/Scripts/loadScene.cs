using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class loadScene : MonoBehaviour {
    public Rocket rocket;

    void Start()
    {

    }

    public void sceneLoad () {
        //Debug.Log("Loading the scene...");
        //SceneManager.LoadScene("moonSceneMain");
        //SteamVR_LoadLevel.Begin("moonSceneMain");
        
        GetComponent<AudioSource>().Play();
        rocket.launch();
    }

	public void setSceneTimeTo90 () {
		PlayerPrefs.SetInt("MaxSceneTime" , 11180);
	}
		public void setSceneTimeTo180 () {
		PlayerPrefs.SetInt("MaxSceneTime" , 11360);
	}
		public void setSceneTimeUnlimited () {
		PlayerPrefs.SetInt("MaxSceneTime" , 116000);
	}

	public void setGraphicsLow (){
		QualitySettings.SetQualityLevel(0, true);
		Debug.Log("Graphics changed to: " + QualitySettings.GetQualityLevel());
	}
	public void setGraphicsMedium (){
		QualitySettings.SetQualityLevel(1, true);
		Debug.Log("Graphics changed to: " + QualitySettings.GetQualityLevel());
	}
	public void setGraphicsHigh (){
		QualitySettings.SetQualityLevel(2, true);
		Debug.Log("Graphics changed to: " + QualitySettings.GetQualityLevel());
	}

    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            SteamVR_LoadLevel.Begin("moonSceneMain");
        }

        for (int handIndex = 0; handIndex < Player.instance.hands.Length; handIndex++)
        {
            Hand hand = Player.instance.hands[handIndex];
            if (hand != null)
            {
                hand.HideSkeleton();
            }
        }
    }
}