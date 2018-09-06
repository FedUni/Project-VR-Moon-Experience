using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class loadScene : MonoBehaviour {

	public void sceneLoad () {
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
}