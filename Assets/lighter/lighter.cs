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
    bool hasAtmos = false;
    AudioSource flameOn;
    bool hasGripped = false;
    // Start is called before the first frame update
    void Start()
    {
        planetSettings = GameObject.Find("PlanetSettings"); // Get the planet settings
        GetComponent<AudioSource>().playOnAwake = false; // Dont play this object strait away
        GetComponent<AudioSource>().clip = collisionSound; // Assign the button sound
        particles = gameObject.GetComponents<ParticleSystem>();
        dust = particles[0];
        flameOn = GameObject.Find("FlameOn").GetComponent<AudioSource>();
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
            hasAtmos = true;
        }

    }

    private void OnDetachedFromHand(Hand hand) // Stop when dropped
    {
        flame.SetActive(false);
        flameOn.Stop();
    }

    private void OnAttachedToHand(Hand hand) // PLay when picked up
    {
        flame.SetActive(true);
        if (hasAtmos)
        {
            flameOn.Play();
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
        }
    }
}
