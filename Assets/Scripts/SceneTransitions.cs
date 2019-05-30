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
        player = GameObject.Find("Player");
        if (PlayerPrefs.GetInt("FirstLoad") == 1) {
            RecallPlayerPosition();
        }
        animators = player.GetComponentsInChildren<Animator>();
        transitionAnim = animators[2];
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(LoadScene());
        }
    }
    IEnumerator LoadScene()
    {
        
        transitionAnim.SetTrigger("end");
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(sceneName);
        
    }

    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();
        if (startingGrabType != GrabTypes.None)
        {
            SavePlayerPosition();
            PlayerPrefs.SetInt("FirstLoad", 1);
            StartCoroutine(LoadScene());

        }
    }

    private void SavePlayerPosition() {
        PlayerPrefs.SetFloat("X", player.transform.position.x);
        PlayerPrefs.SetFloat("Y", player.transform.position.y);
        PlayerPrefs.SetFloat("Z", player.transform.position.z);
    }

    private void RecallPlayerPosition() {
        player.transform.position = new Vector3(PlayerPrefs.GetFloat("X"), PlayerPrefs.GetFloat("Y"), PlayerPrefs.GetFloat("Z"));
    }
    void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("FirstLoad", 0);
    }
}

