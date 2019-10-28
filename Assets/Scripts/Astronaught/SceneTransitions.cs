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
    float shadowAmount = 0.813f;

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
            panelLights = DropRig.GetComponentsInChildren<Light>(); // Get all the lights elements in the drop rig
        }
        
        if (PlayerPrefs.GetInt("FirstLoad") == 1) { // We dont want to load the cords on the first run only after they have teleported once before
            RecallPlayerPosition(); // read the data for the player postion
        }
        animators = player.GetComponentsInChildren<Animator>(); // Get the animator object
        transitionAnim = animators[0]; // Its the third item // changed to 0
        disolveMat.SetFloat("_DissolveEmission", 500);
        Material disMat = dissolveMat[0];
        disMat.SetFloat("_DissolveAmount", 0.3f);
    }

    void Update()
    {
        if (shouldDissolve) // The disolve animation should begin
        {


            Light[] lights = GameObject.FindObjectsOfType<Light>(); // Get all the light in the scene
            MeshRenderer[] everything = GameObject.FindObjectsOfType<MeshRenderer>(); // Get all the meshes
            SkinnedMeshRenderer[] skinnedMeshes = GameObject.FindObjectsOfType<SkinnedMeshRenderer>(); //Get the player model
            Material disMat = dissolveMat[0]; // Set the disolve mat
            
            foreach (Light light in lights) // For every light in the scene
            {
                light.shadowStrength = shadowAmount; // Set the shadow amount to a new value

            }
            disMat.SetFloat("_DissolveAmount", Mathf.Lerp(disolveMat.GetFloat("_DissolveAmount"), 1, 1f * Time.deltaTime)); // Lerp the effect
            shadowAmount = Mathf.Lerp(shadowAmount, 0, 5f * Time.deltaTime); // Lerp the shadow amount

        }

    }
    public IEnumerator LoadScene(String sceneName)
    {    
        transitionAnim.SetTrigger("end"); // Set the animation up for the fade to black
        disolveMat.SetFloat("_DissolveAmount", 0.3f); // Set the disove mat to start a third threw


        mesh = GameObject.Find("AstronautShoeL").GetComponent<SkinnedMeshRenderer>(); // Find the player mesh by name

        MeshRenderer[] everything = GameObject.FindObjectsOfType<MeshRenderer>(); // Get every mesh rendering in the scene
        SkinnedMeshRenderer[] skinnedMeshes = GameObject.FindObjectsOfType<SkinnedMeshRenderer>(); // and the player mech
        Canvas[] canvasEverything = GameObject.FindObjectsOfType<Canvas>(); // Get all the canvases

        foreach (SkinnedMeshRenderer skinnedStuff in skinnedMeshes)
        {
            skinnedStuff.materials = dissolveMat; // Apply the disove mat
            
        }

        foreach (MeshRenderer stuff in everything)
        {
            if (stuff.gameObject.name != "WaterProNighttime") {
                stuff.materials = dissolveMat; // Apply the disove mat
            }
        }

        foreach (Canvas canvasStuff in canvasEverything)
        {
            canvasStuff.GetComponentInChildren<Canvas>().enabled = false; // Turn the canvases off

        }

        Light[] lights = GameObject.FindObjectsOfType<Light>(); // Get all the light in the scene

        foreach (Light light in lights)
        {
            if (light.name != "Sunlight")
            {
                light.intensity = 0f;
            }
        }
        if (DropRig != null) { //  Turn off the drop rig light

            
            panelLights[0].intensity = 0;
            panelLights[1].intensity = 0;
        }

        mesh.materials = dissolveMat;
        //GameObject.Find("Watch").SetActive(false);
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

