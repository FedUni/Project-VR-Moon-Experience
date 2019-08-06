using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR.InteractionSystem;

public class SceneTransitions : MonoBehaviour
{
    public Animator transitionAnim;
    public string sceneName;
    GameObject player;
    Animator[] animators;
    

    private void Start()
    {
        player = GameObject.Find("Player"); // Get the player
        if (PlayerPrefs.GetInt("FirstLoad") == 1) { // We dont want to load the cords on the first run only after they have teleported once before
            RecallPlayerPosition(); // read the data for the player postion
        }
        animators = player.GetComponentsInChildren<Animator>(); // Get the animator object
        transitionAnim = animators[0]; // Its the third item // changed to 0
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // The space bar is used by the operator the change scenes
        {
            StartCoroutine(LoadScene());
        }
    }
    IEnumerator LoadScene()
    {
        
        transitionAnim.SetTrigger("end"); // Set the animation up
        yield return new WaitForSeconds(1.5f); // Wait for the animation to play
        SceneManager.LoadScene(sceneName); // Load the scene
        
    }

    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();
        if (startingGrabType != GrabTypes.None)
        {
            SavePlayerPosition(); // Save the players postion
            PlayerPrefs.SetInt("FirstLoad", 1); // Save that the player has teleported once since opening the game
            StartCoroutine(LoadScene()); // Load the next scene

        }
    }

    private void SavePlayerPosition() { // Save the players cords
        PlayerPrefs.SetFloat("X", player.transform.position.x);
        PlayerPrefs.SetFloat("Y", player.transform.position.y);
        PlayerPrefs.SetFloat("Z", player.transform.position.z);
    }

    private void RecallPlayerPosition() { // This put the player into the same location they were in in the previous scene
        player.transform.position = new Vector3(PlayerPrefs.GetFloat("X"), PlayerPrefs.GetFloat("Y"), PlayerPrefs.GetFloat("Z"));
    }
    void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("FirstLoad", 0); // When the player quites the game the flag needs to be reset
    }
}

