using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using UnityEngine.UI;

[RequireComponent( typeof( Interactable ) )]
public class LaserAnimate : MonoBehaviour {

    Animator anim;
    public MeshRenderer laserBeam;
    GameObject planetSettings;

    void Start()
    {
        anim = GameObject.Find("LaserExperiment").GetComponent<Animator>();
        planetSettings = GameObject.Find("PlanetSettings"); // Get the planet settings
        if (!planetSettings.GetComponent<PlanetSettings>().isMoon)
        {
            //GameObject.Find("UnderExpLaser").SetActive(false);
        }
    }
    //Called every Update() while a Hand is hovering over this object
    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();
        if (startingGrabType != GrabTypes.None) {
            Debug.Log("Begin animating: " + gameObject.name + ", using animation: " + anim.name);
            anim.Play("LaserAnimate");
            StartCoroutine(TurnOnLaser());
            GameObject.Find("LaserActive").GetComponent<Text>().text = "True"; // Set the text
        }

    }
    IEnumerator TurnOnLaser()
    {

        yield return new WaitForSeconds(3.0f);
        laserBeam.enabled = true;
        if (planetSettings.GetComponent<PlanetSettings>().isMoon)
        { // These will display the correct info based on the planet we are on
            Debug.Log("The distacne between the moon and the earth is 384,400km");
            GameObject.Find("LaserDist").GetComponent<Text>().text = "384,400km"; // Set the text
            GameObject.Find("LaserTime").GetComponent<Text>().text = "1.3 Seconds"; // Set the text
        }
        if (planetSettings.GetComponent<PlanetSettings>().isMars)
        {
            GameObject.Find("UnderExpLaser").SetActive(false);
            Debug.Log("The distacne between the moon and the earth is 54.6 million km");
            GameObject.Find("LaserDist").GetComponent<Text>().text = "54.6 million km"; // Set the text
            GameObject.Find("LaserTime").GetComponent<Text>().text = "3.03 minutes"; // Set the text
        }
        if (planetSettings.GetComponent<PlanetSettings>().isEarth)
        {
            GameObject.Find("UnderExpLaser").SetActive(false);
            Debug.Log("The distacne between the moon and the earth is 384,400 km");
            GameObject.Find("LaserDist").GetComponent<Text>().text = "384,400km"; // Set the text
            GameObject.Find("LaserTime").GetComponent<Text>().text = "1.3 Seconds"; // Set the text
        }


    }

    public void laserAni()
    {
        Debug.Log("Begin animating: " + gameObject.name + ", using animation: " + anim.name);
        anim = GameObject.Find("LaserExperiment").GetComponent<Animator>();
        anim.Play("LaserAnimate");
        StartCoroutine(TurnOnLaser());
        GameObject.Find("LaserActive").GetComponent<Text>().text = "True"; // Set the text
    }

}
