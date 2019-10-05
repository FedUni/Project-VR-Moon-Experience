using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

// Created by Wayland Bishop for The Moon VR 3.0 project
[RequireComponent(typeof(AudioSource))]

public class Droppable : MonoBehaviour
{
    public bool hasDropped = false;
    bool isFalling = false;
    float dropTime = 0.0f;
    public float timeFalling = 0.0f;
    GameObject planetSettings;
    GameObject DropRig;
    ParticleSystem dust;
    ParticleSystem.TrailModule trails;
    ParticleSystem.MainModule main;
    public AudioClip collisionSound;
    bool complete = false;
    Text[] text;
    Color dustColor;
    float mass;

    private Rigidbody rBody;
    private int forcePushPullPower = 1;
    private float moveScale;

    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        planetSettings = GameObject.Find("PlanetSettings"); // Get the planet settings
        GetComponent<AudioSource>().playOnAwake = false; // Dont play this object strait away
        GetComponent<AudioSource>().clip = collisionSound; // Assign the button sound
        DropRig = GameObject.Find("DropRig"); // Get the Drop rig
        text = DropRig.GetComponentsInChildren<Text>(); // Get all the text elements in the drop rig
        mass = transform.GetComponentInChildren<Rigidbody>().mass;
        //dust = gameObject.GetComponentInChildren<ParticleSystem>();
        dust = GetComponent<ParticleSystem>();
        main = dust.main;
        trails = dust.trails;
        if (planetSettings.GetComponent<PlanetSettings>().isMoon == true) { // These set the drag up for eatch planet setting
            GetComponent<Rigidbody>().drag = GetComponent<Rigidbody>().drag * 0;
            dustColor = new Color(84 / 255f, 84 / 255f, 84 / 255f, 255 / 255f);
            //transform.GetComponentInChildren<Rigidbody>().mass = 1;
        }
        if (planetSettings.GetComponent<PlanetSettings>().isMars == true)
        {
            GetComponent<Rigidbody>().drag = GetComponent<Rigidbody>().drag * 0.5f;
            dustColor = new Color(132 / 255f, 87 / 255f, 39 / 255f, 255 / 255f);
        } else { 
            GetComponent<Rigidbody>().drag = GetComponent<Rigidbody>().drag * 1;
            dustColor = new Color(0.2f, 0.2f, 0.15f, 1f);
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
            //Debug.Log(gameObject.name + "Falling for " + Math.Round(Time.time - dropTime, 1) + " Seconds"); // Temp outputting to the console

            text[3].text = "";

            if (transform.name.Contains("left")) { // If this is a left object
                text[0].text = "Left object fell for " + Math.Round(Time.time - dropTime, 1) + " Seconds"; // Set the drop rig LCD text
                if (GameObject.Find("WatchDropLeft") != null)
                {
                    GameObject.Find("WatchDropLeft").GetComponent<Text>().text = Math.Round(Time.time - dropTime, 1).ToString() + " Sec"; // Set the text
                }

            }
            if (transform.name.Contains("right")) { // if this is a right object
                text[1].text = "Right object fell for " + Math.Round(Time.time - dropTime, 1) + " Seconds"; // Set the drop rig LCD text
                if (GameObject.Find("WatchDropRight") != null)
                {
                    GameObject.Find("WatchDropRight").GetComponent<Text>().text = Math.Round(Time.time - dropTime, 1).ToString() + " Sec"; // Set the text
                }
            }

        }

    }

    private void OnTriggerEnter(Collider other) // When the object collides with the trigger collider
    {
        if (hasDropped && other.name == "DropRig" && !complete) // Only do this if the object has been dropped to prevent this from playing if the played knocks the objects off the drig or they fall off it
        {
            timeFalling = Time.time - dropTime; // Get the time since the object was falgged as falling
            transform.GetComponentInChildren<Text>().text = Math.Round(Time.time - dropTime, 1) + " Seconds at " + Math.Round(mass * planetSettings.GetComponent<PlanetSettings>().gravity, 2) + "kg" ;
            //Debug.Log("The object was falling for " + Math.Round(Time.time - dropTime, 1) + " Seconds"); // Temp output to console of the total falling time
            complete = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (planetSettings.GetComponent<PlanetSettings>().hasAtmos && collision.relativeVelocity.magnitude > 0.5) { // If this planet has an atmos the sound should be played and the collsion was hard enoguh to generate a sound

            GetComponent<AudioSource>().Play(); // Play the sound effect
            GetComponent<AudioSource>().volume = (collision.relativeVelocity.magnitude / 5f + 0.5f); // Change the volume based on how hard it hit
            GetComponent<AudioSource>().pitch = (UnityEngine.Random.value * 0.5f + 0.5f); // Change the pitch randomly to get a better effect
        }
        if (collision.GetContact(0).otherCollider.name == "Terrain" && dust != null) { // Check to see if the collider was the ground
            main.startColor = dustColor;
            trails.colorOverTrail = dustColor;
            dust.Stop();
            dust.Play();
        }
    }
    public void Grab(bool shouldGrab)
    {
        rBody.isKinematic = shouldGrab;
    }

    public void Move(Vector3 curHandPos, Vector3 lastHandPos, Quaternion difference, Quaternion currentHandRotation)
    {
        //rBody.AddForce((curHandPos - transform.position) * 1000, ForceMode.Impulse);

        //transform.position = (Vector3.Lerp(rBody.position, rBody.position + (curHandPos - lastHandPos) * moveScale * 10, 10 * Time.deltaTime));
        rBody.MovePosition(Vector3.Lerp(rBody.position , rBody.position + (curHandPos - lastHandPos) * moveScale * 10, 10 * Time.deltaTime));
        //rBody.MoveRotation(Quaternion.RotateTowards(rBody.rotation, difference * currentHandRotation, 50f * Time.deltaTime));
        //Quaternion newRot = Quaternion.Lerp(transform.rotation, difference * currentHandRotation, 50f * Time.deltaTime);
        //rBody.MoveRotation(newRot);
        //Quaternion torque = difference * Quaternion.Inverse(currentHandRotation); 
        //rBody.AddRelativeTorque(torque.eulerAngles / 500);
        //rBody.
    }

    public void SetMoveScale(Vector3 handPostion)
    {
        Vector3 origin = GameObject.FindGameObjectWithTag("MainCamera").transform.position;
        moveScale = Vector3.Magnitude(transform.position - origin) / Vector3.Magnitude(handPostion - origin);
    }

    public void ForcePush(Vector3 direction, int power)
    {
        rBody.AddForce(direction * power, ForceMode.Acceleration);
    }

}
    

