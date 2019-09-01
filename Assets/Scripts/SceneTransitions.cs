using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR.InteractionSystem;
using UnityEngine.Rendering;

public class SceneTransitions : MonoBehaviour
{
    public Animator transitionAnim;
    public string sceneName;
    GameObject player;
    Animator[] animators;
    public Material[] dissolveMat;
    SkinnedMeshRenderer mesh;
    bool shouldDissolve = false;
    public Material disolveMat;
    GameObject DropRig;
    Light[] panelLights;

    public void teleportViaWatchUI(String sceneName) {
        SavePlayerPosition(); // Save the players postion
        PlayerPrefs.SetInt("FirstLoad", 1); // Save that the player has teleported once since opening the game
        StartCoroutine(LoadScene(sceneName)); // Load the next scene
    }

    private void Start()
    {
        player = GameObject.Find("Player"); // Get the player
        DropRig = GameObject.Find("DropRig"); // Get the drop rig
        if (DropRig != null)
        {
            panelLights = DropRig.GetComponentsInChildren<Light>(); // Get all the text elements in the drop rig
        }
        
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
            StartCoroutine(LoadScene(sceneName));
        }
        if (shouldDissolve)
        {
            disolveMat.SetFloat("_DissolveAmount", Mathf.Lerp(disolveMat.GetFloat("_DissolveAmount"), 1, 0.5f * Time.deltaTime));

        }
    }
    public IEnumerator LoadScene(String sceneName)
    {    
        transitionAnim.SetTrigger("end"); // Set the animation up
        disolveMat.SetFloat("_DissolveAmount", 0f);


        mesh = GameObject.Find("AstronautShoeL").GetComponent<SkinnedMeshRenderer>();

        MeshRenderer[] everything = GameObject.FindObjectsOfType<MeshRenderer>();
        SkinnedMeshRenderer[] skinnedMeshes = GameObject.FindObjectsOfType<SkinnedMeshRenderer>();
        Canvas[] canvasEverything = GameObject.FindObjectsOfType<Canvas>();

        foreach (SkinnedMeshRenderer skinnedStuff in skinnedMeshes)
        {
            skinnedStuff.materials = dissolveMat;
            skinnedStuff.shadowCastingMode = ShadowCastingMode.Off;
            
        }

        foreach (MeshRenderer stuff in everything)
        {
            stuff.materials = dissolveMat;
            stuff.shadowCastingMode = ShadowCastingMode.Off;
        }
        foreach (Canvas canvasStuff in canvasEverything)
        {
            canvasStuff.GetComponentInChildren<Canvas>().enabled = false;

        }
        if (DropRig != null) {
            panelLights[0].intensity = 0;
            panelLights[1].intensity = 0;
        }

        mesh.materials = dissolveMat;
        GameObject.Find("Watch").SetActive(false);
        shouldDissolve = true;
        yield return new WaitForSeconds(2.0f); // Wait for the animation to play
        SceneManager.LoadScene(sceneName); // Load the scene   
    }

    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();
        if (startingGrabType != GrabTypes.None)
        {
            SavePlayerPosition(); // Save the players postion
            PlayerPrefs.SetInt("FirstLoad", 1); // Save that the player has teleported once since opening the game
            StartCoroutine(LoadScene(sceneName)); // Load the next scene

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

