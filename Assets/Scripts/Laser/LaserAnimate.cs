using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using UnityEngine.UI;

public class LaserAnimate : MonoBehaviour {

    Animator anim;
    public MeshRenderer laserBeam;
    GameObject planetSettings;
    public MoonDate moonDate;

    void Start()
    {
        anim = GameObject.Find("LaserExperiment").GetComponent<Animator>();
        planetSettings = GameObject.Find("PlanetSettings"); // Get the planet settings
    }

    IEnumerator TurnOnLaser()
    {
        yield return new WaitForSeconds(3.0f);
        laserBeam.enabled = true;

        if (GameObject.Find("LaserActive") != null) // This is for when the controller are turned off
        {
            GameObject.Find("LaserDist").GetComponent<Text>().text = "384,499km"; // Set the text
            GameObject.Find("LaserTime").GetComponent<Text>().text = "1.3 Seconds"; // Set the text
        }
        GameObject.Find("LaserSignExp").GetComponent<Text>().text = "Lunar Laser Ranging Experiment Distacne to earth is <color=#00FFFF>384,499km</color>"; // Set the text
        GameObject.Find("LaserSignGolf").GetComponent<Text>().text = "Distance to Earth  <color=#00FFFF>384,499km</color> See how far you can get!"; // Set the text        

    }

    public void laserAni()
    {
        anim = GameObject.Find("LaserExperiment").GetComponent<Animator>();
        anim.Play("LaserAnimate");
        StartCoroutine(TurnOnLaser());
        if (GameObject.Find("LaserActive") != null) // This is for when the controller are turned off
        {
            GameObject.Find("LaserActive").GetComponent<Text>().text = "True"; // Set the text
        }
    }

}
