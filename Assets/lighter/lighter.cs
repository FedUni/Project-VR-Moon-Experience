using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
[RequireComponent(typeof(AudioSource))]

public class lighter : MonoBehaviour
{
    public GameObject flame;
    ParticleSystem[] particles;
    ParticleSystem dust;
    ParticleSystem.TrailModule trails;
    ParticleSystem.MainModule main;
    GameObject planetSettings;
    public AudioClip collisionSound;
    Color dustColor;
    // Start is called before the first frame update
    void Start()
    {
        planetSettings = GameObject.Find("PlanetSettings"); // Get the planet settings
        GetComponent<AudioSource>().playOnAwake = false; // Dont play this object strait away
        GetComponent<AudioSource>().clip = collisionSound; // Assign the button sound
        particles = gameObject.GetComponents<ParticleSystem>();
        dust = particles[0];
        //flame = gameObject.GetComponentInChildren<GameObject>();
        main = dust.main;
        trails = dust.trails;
        if (planetSettings.GetComponent<PlanetSettings>().isMoon == true)
        { // These set the drag up for eatch planet setting
            GetComponent<Rigidbody>().drag = GetComponent<Rigidbody>().drag * 0;
            dustColor = new Color(84 / 255f, 84 / 255f, 84 / 255f, 255 / 255f);
        }
        if (planetSettings.GetComponent<PlanetSettings>().isMars == true)
        {
            GetComponent<Rigidbody>().drag = GetComponent<Rigidbody>().drag * 0.5f;
            dustColor = new Color(132 / 255f, 87 / 255f, 39 / 255f, 255 / 255f);
        }
        if (planetSettings.GetComponent<PlanetSettings>().isEarth)
        {
            GetComponent<Rigidbody>().drag = GetComponent<Rigidbody>().drag * 1;
            dustColor = new Color(140 / 255f, 126 / 255f, 111 / 255f, 255 / 255f);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();
        if (startingGrabType != GrabTypes.None)
        {
            flame.SetActive(true);
        }
        else {
            flame.SetActive(false);
        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (planetSettings.GetComponent<PlanetSettings>().hasAtmos && collision.relativeVelocity.magnitude > 0.5)
        { // If this planet has an atmos the sound should be played and the collsion was hard enoguh to generate a sound

            GetComponent<AudioSource>().Play(); // Play the sound effect
            GetComponent<AudioSource>().volume = (collision.relativeVelocity.magnitude / 5f + 0.5f); // Change the volume based on how hard it hit
            GetComponent<AudioSource>().pitch = (UnityEngine.Random.value * 0.5f + 0.5f); // Change the pitch randomly to get a better effect
        }
        if (collision.GetContact(0).otherCollider.name == "Terrain" && dust != null)
        { // Check to see if the collider was the ground
            Vector3 contactPoint = collision.GetContact(0).point;  // Get the codinates of the contact point
            main.startColor = dustColor;
            trails.colorOverTrail = dustColor;
            dust.Play();
            Debug.Log("Trying to play: " + dust.name);
            //Debug.Log("Right now the puff of dust would be happeing if we had one.");
            //Debug.Log("It should be spawned at " + contactPoint);
        }
    }
}
