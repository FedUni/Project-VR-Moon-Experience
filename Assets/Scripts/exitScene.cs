using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class exitScene : MonoBehaviour {

	public void SceneExit () {
        Debug.Log("Exiting the Application");
        Application.Quit();
    }
}
