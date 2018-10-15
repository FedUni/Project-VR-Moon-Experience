using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class loadScene : MonoBehaviour {

	public void sceneLoad () {
        Debug.Log("Loading the scene...");
        SceneManager.LoadScene("moonSceneMain");
	}

	// Use this for initialization
	/*void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnClick () {
		SceneManager.LoadScene(moonSceneMain);
		Debug.Log ("Loading Scene");
	}*/

	public void setSceneTimeTo90 () {
		PlayerPrefs.SetInt("MaxSceneTime" , 90);
	}
		public void setSceneTimeTo180 () {
		PlayerPrefs.SetInt("MaxSceneTime" , 180);
	}
		public void setSceneTimeUnlimited () {
		PlayerPrefs.SetInt("MaxSceneTime" , 3000);
	}
}