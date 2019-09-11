using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BalloonSleep : MonoBehaviour
{
    public float floatStrength; // change to your liking 
    GameObject planetSettings;
    public AudioClip popSound;
    bool playedOnce = false;

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
            floatStrength = floatStrength * 1f;
        }
        if (planetSettings.GetComponent<PlanetSettings>().isMoon) // If this planet has an atmos the sound should be played
        {
            floatStrength = floatStrength * 0f;
        }
        if (planetSettings.GetComponent<PlanetSettings>().isMars) // If this planet has an atmos the sound should be played
        {
            floatStrength = floatStrength * 0.2f;
        }
    }

    // Update is called once per frame
    void Update()
    {
            GetComponent<Rigidbody>().AddForce(new Vector3(0, 0.0005f, 0) * floatStrength);
        
        //Debug.Log(new Vector3(0,0.0001f,0) * floatStrength);
    }
    private void FixedUpdate()
    {
        
    }
    private void OnTriggerEnter(Collider other) // When the object collides with the trigger collider
    {


        if (other.name == "Lighter") // Only do this if the object has been dropped to prevent this from playing if the played knocks the objects off the drig or they fall off it
        {
           
            if (planetSettings.GetComponent<PlanetSettings>().hasAtmos && gameObject.GetComponent<AudioSource>() != null && playedOnce == false)
            { // If this planet has an atmos the sound should be played

                playedOnce = true;
                gameObject.GetComponent<AudioSource>().Play(); // Play the sound effect
             
            }
            gameObject.GetComponent<ParticleSystem>().Play();
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            gameObject.GetComponentInChildren<Canvas>().enabled = false;
            Rigidbody[] balloonString = gameObject.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody stringPiece in balloonString) {
                stringPiece.useGravity = true;
            }
            Destroy(gameObject,30f);
        }
    }
}
