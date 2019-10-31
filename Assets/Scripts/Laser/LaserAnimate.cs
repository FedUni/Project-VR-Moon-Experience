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
        moonDate.Calculate();  
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
