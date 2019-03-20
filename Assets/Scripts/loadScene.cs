using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using Valve.VR;

public class loadScene : MonoBehaviour {
    public Rocket rocket;

	public void sceneLoad () {
        //Debug.Log("Loading the scene...");
        //SceneManager.LoadScene("moonSceneMain");
        //SteamVR_LoadLevel.Begin("moonSceneMain");
        
        GetComponent<AudioSource>().Play();
        rocket.launch();
    }

	public void setSceneTimeTo90 () {
		PlayerPrefs.SetInt("MaxSceneTime" , 90);
	}
		public void setSceneTimeTo180 () {
		PlayerPrefs.SetInt("MaxSceneTime" , 180);
	}
		public void setSceneTimeUnlimited () {
		PlayerPrefs.SetInt("MaxSceneTime" , 3000);
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
}