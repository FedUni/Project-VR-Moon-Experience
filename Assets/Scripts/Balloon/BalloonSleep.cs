using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BalloonSleep : MonoBehaviour
{
    public float floatStrength; // change to your liking 
    GameObject planetSettings;
    public AudioClip popSound;
    bool playedOnce = false;
    bool fireIsNeeded = false;
    public bool isHydrogen = false;
    public bool isOxygen = false;
    public bool isHelium = false;
    public bool isCO2 = false;
    public ParticleSystem smoke;
    public ParticleSystem smoke3;

    // Start is called before the first frame update
    private void Awake()
    {
        gameObject.GetComponent<Rigidbody>().Sleep();
        
    }
    void Start()
    {


        if (GetComponent<AudioSource>() != null)
        {
            GetComponent<AudioSource>().playOnAwake = false; // Dont play this object strait away
            GetComponent<AudioSource>().clip = popSound; // Assign the button sound
        }
        planetSettings = GameObject.Find("PlanetSettings"); // Get the planet settings
        if (planetSettings.GetComponent<PlanetSettings>().isEarth) // If this planet has an atmos the sound should be played
        {
            floatStrength = floatStrength * 1f; // Set the upward float amount
            if (isHydrogen) {
                fireIsNeeded = true; // Should burn
            }
            if (isOxygen)
            {
                fireIsNeeded = true; // Should burn
            }
        }
        if (planetSettings.GetComponent<PlanetSettings>().isMoon) // If this planet has an atmos the sound should be played
        {
            floatStrength = floatStrength * 0f; // Set the upward float amount
            if (isOxygen)
            {
                fireIsNeeded = true; // Should burn
            }
        }
        if (planetSettings.GetComponent<PlanetSettings>().isMars) // If this planet has an atmos the sound should be played
        {
            floatStrength = floatStrength * 0.2f; // Set the upward float amount
            if (isOxygen)
            {
                fireIsNeeded = true; // Should burn
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
            GetComponent<Rigidbody>().AddForce(new Vector3(0, 0.0005f, 0) * floatStrength); // continually add the float force
    }
    private void OnTriggerEnter(Collider other) // When the object collides with the trigger collider
    {


        if (other.name == "Lighter") // The lighter was touched tot eh balloon
        {

            Rigidbody[] strings = gameObject.transform.parent.GetComponentsInChildren<Rigidbody>(); // The the string pieces on this balloon
            foreach (Rigidbody str in strings)
            {
                str.useGravity = true; // Make them fall down
            }
            floatStrength = 0f; // Stop the ballon flating
            if (planetSettings.GetComponent<PlanetSettings>().hasAtmos && gameObject.GetComponent<AudioSource>() != null && playedOnce == false)
            { // If this planet has an atmos the sound should be played

                playedOnce = true;
                gameObject.GetComponent<AudioSource>().Play(); // Play the sound effect
             
            }
            if (fireIsNeeded)
            {
                smoke.Play(); // Play the smoke
                smoke3.Play(); // Play the smoke
            }
            gameObject.GetComponent<ParticleSystem>().Play(); // PLay the ballon pop
            gameObject.GetComponent<MeshRenderer>().enabled = false; // Hide the ballon mesh
            gameObject.GetComponentInChildren<Canvas>().enabled = false; // Turn the canvas for the gas details off
            Destroy(gameObject, 0.5f); // Destroy the ballon
        }
    }
}
