using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent( typeof( Interactable ) )]
public class LaserAnimate : MonoBehaviour {

    Animator anim;
    public MeshRenderer laserBeam;
    GameObject planetSettings;

    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        planetSettings = GameObject.Find("PlanetSettings"); // Get the planet settings
    }
    //Called every Update() while a Hand is hovering over this object
    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();
        if (startingGrabType != GrabTypes.None) {
            Debug.Log("Begin animating: " + gameObject.name + ", using animation: " + anim.name);
            anim.Play("LaserAnimate");
            StartCoroutine(TurnOnLaser());
        }

    }
    IEnumerator TurnOnLaser()
    {

        yield return new WaitForSeconds(3.0f);
        laserBeam.enabled = true;
        if (planetSettings.GetComponent<PlanetSettings>().isMoon)
        { // These set the drag up for eatch planet setting
            Debug.Log("The distacne between the moon and the earth is 384,400 km");
        }
        if (planetSettings.GetComponent<PlanetSettings>().isMars)
        {
            Debug.Log("The distacne between the moon and the earth is 54.6 million km");
        }
        if (planetSettings.GetComponent<PlanetSettings>().isEarth)
        {
            Debug.Log("The distacne between the moon and the earth is 384,400 km");
        }


    }

}
