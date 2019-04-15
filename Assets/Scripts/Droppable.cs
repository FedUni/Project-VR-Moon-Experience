using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Created by Wayland Bishop for The Moon VR 3.0 project
[RequireComponent(typeof(AudioSource))]
public class Droppable : MonoBehaviour
{
    public bool hasDropped = false;
    bool isFalling = false;
    float dropTime = 0.0f;
    public float timeFalling = 0.0f;
    GameObject planetSettings;
    public AudioClip collisionSound;
    bool complete = false;
    // Start is called before the first frame update
    void Start()
    {
        planetSettings = GameObject.Find("PlanetSettings"); // Get the planet settings
        GetComponent<AudioSource>().playOnAwake = false; // Dont play this object strait away
        GetComponent<AudioSource>().clip = collisionSound; // Assign the button sound
        if (planetSettings.GetComponent<PlanetSettings>().isMoon == true) { // These set the drag up for eatch planet setting
            GetComponent<Rigidbody>().drag = 0.0f;
        }
        if (planetSettings.GetComponent<PlanetSettings>().isMars == true)
        {
            GetComponent<Rigidbody>().drag = 0.01f;
        }
        if (planetSettings.GetComponent<PlanetSettings>().isEarth == true)
        {
            GetComponent<Rigidbody>().drag = 1.0f;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (hasDropped && isFalling == false) { // See if the object has been set to drop and ensure its not curently falling
            dropTime = Time.time; // Set the current time as the time the object began to fall
            isFalling = true; // Set the object flag to be falling the the time isnt reset on next frame
        }
        if (isFalling && !complete) { // Output the current falltime to a display of some kind (LCD timer)
            Debug.Log(gameObject.name + "Falling for " + (Time.time - dropTime) + " Seconds"); // Temp outputting to the console
        }
    }

    private void OnTriggerEnter(Collider other) // When the object collides with the trigger collider
    {
        if (hasDropped && other.name == "DropRig" && !complete) // Only do this if the object has been dropped to prevent this from playing if the played knocks the objects off the drig or they fall off it
        {
            timeFalling = Time.time - dropTime; // Get the time since the object was falgged as falling
            Debug.Log("The object was falling for " + (Time.time - dropTime) + " Seconds"); // Temp output to console of the total falling time
            complete = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (planetSettings.GetComponent<PlanetSettings>().hasAtmos && collision.relativeVelocity.magnitude > 0.5) { // If this planet has an atmos the sound should be played and the collsion was hard enoguh to generate a sound

            GetComponent<AudioSource>().Play(); // Play the sound effect
            GetComponent<AudioSource>().volume = (collision.relativeVelocity.magnitude / 5f + 0.5f); // Change the volume based on how hard it hit
            GetComponent<AudioSource>().pitch = (Random.value * 0.5f + 0.5f); // Change the pitch randomly to get a better effect
        }
        if (collision.GetContact(0).otherCollider.name == "Terrain") { // Check to see if the collider was the ground
            Vector3 contactPoint = collision.GetContact(0).point;  // Get the codinates of the contact point
            Debug.Log("Right now the puff of dust would be happeing if we had one.");
            Debug.Log("It should be spawned at " + contactPoint);
        }
    }

}
    

