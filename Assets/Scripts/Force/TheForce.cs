using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using System;

public class TheForce : MonoBehaviour
{
    private SteamVR_Controller.Device controller;
    [Tooltip("The line renderer used to find the object the player is pointing at")]
    public LineRenderer lRender;
    private Vector3[] postions; // Postions used for the line renderer
    private Droppable grabbable; // Used as the object we hit object must have droppable script to be able to use the force on it
    private bool grabbed; // Toggle for being grabbed
    [Tooltip("The tracked object we are using as the hand")]
    public SteamVR_TrackedObject trackedObj;
    [Tooltip("The hadn the player is using for the force")]
    public Hand hand;
    private Vector3 lastHandPos;
    private PlanetSettings planetsettings;
    [Tooltip("The effect used to visually show the beam to the object")]
    public ParticleSystem laserBeam; // Beam type effect
    private ParticleSystem ZP; // Hand effect
    private ParticleSystem EP; // Endpoint effect
    [Tooltip("The effect used to visually end of the beam")]
    public GameObject endpointSpark; // gameobject that has the partilce attached
    Quaternion curHandRotation;
    Quaternion difference;
    [Tooltip("The gameObject used that houses the zero points partilce")]
    public GameObject ZeroPoint;
    private float endWidth = 0.07f;
    private bool holding = false;


    // Start is called before the first frame update
    void Start()
    {
        controller = SteamVR_Controller.Input((int) trackedObj.index); // Get the controller
        postions = new Vector3[2]; // The postions used gotten from the raycast
        planetsettings = GameObject.Find("PlanetSettings").GetComponent<PlanetSettings>(); // The planet setting that holds the value that defines if the force is turned on or not
        ZP = ZeroPoint.GetComponent<ParticleSystem>(); // The zero point begin point partilce (At hand)
        EP = endpointSpark.GetComponent<ParticleSystem>(); // The spark at the end of the beam (At object)
        ZP.Play(); // play it to warm up (At hand)
        ZP.Stop(); // Stop it so its ready to play (At hand)
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (planetsettings.forceEnabled) // Check to see if the force is turned on in panetsetting by the toggle on the watch
        {
            if (!grabbed) // If the object if currently being grabbed
            {
                grabbable = RaycastForGrabbedObject(); // The object we grabbbed
                if (!grabbable) return; // Return if the object was null
            }

            Vector3 curHandPos = transform.position; // The current has postion
            Quaternion curHandRot = transform.rotation; //The current hand rotation
            GrabTypes startingGrabType = hand.GetGrabStarting();
            if (startingGrabType == GrabTypes.Pinch) // Force grab
            {
                grabbed = true; // The object is now being grabbed
                grabbable.Grab(true); // Turn the grabbed object to iskinematic
                grabbable.SetMoveScale(transform.position); // Sets the movescale (Exagurated move, move controller a little the object moves a lot)
                lastHandPos = curHandPos; // Store the current had postion to the last has postion
                DisplayLine(false, transform.position); // Hide the raycast line since we dont need it while the object is grabbed
                LaserLine(false); // Turn the visual laser to object off as we are using a partilce effect for this now
                laserBeam.Play(); // Play the laser beam particle effect
                EP.Play(); // Play the zero point partile effect (At object)
                ZP.Play(); // Play the zero point partile effect (At hand)
                ZeroPoint.transform.LookAt(grabbable.transform); // Make the partilce effect point towards the object
                difference = transform.rotation * Quaternion.Inverse(grabbable.transform.rotation);
                //difference = Quaternion.Inverse(transform.rotation) * transform.rotation;
                holding = true;
            }

            if (hand.IsGrabbingWithType(GrabTypes.Pinch) && holding == true) // If the player is holding the trigger in 
            {
                grabbable.Move(curHandPos, lastHandPos, difference, transform.rotation); // Move the object about
                postions[0] = transform.position; // The postions used that define the viaual beam between the hand and the object (Here hand)
                postions[1] = grabbable.transform.position; // The postions used that define the viaual beam between the hand and the object (Here object)
                endpointSpark.transform.position = grabbable.transform.position; // Put the endpoit effect at the object being moved
                lRender.SetPositions(postions); // Set the postions of the line renderer
                ZeroPoint.transform.LookAt(grabbable.transform); // Make the beam point at the object
            }

            GrabTypes endingGrabType = hand.GetGrabEnding();
            if (endingGrabType == GrabTypes.Pinch) // If teh player released the trigger
            {
                grabbed = false; // The object is no longer beign grabbed
                grabbable.Grab(false); // The object is no longer beign grabbed
                endWidth = 0.07f; // change the laser line end width
                laserBeam.Stop(); // Stop the partile effect
                EP.Stop(); // Stop the endpoint partile effect
                ZP.Stop(); // Stop the hand partile effect
                holding = false; // Its not being held anymore
                difference = transform.rotation * Quaternion.Inverse(grabbable.transform.rotation); // Get the rotational difference between the hand and the object being rotated

            }

            lastHandPos = curHandPos; // Update the han postion

            GrabTypes pushPullGrabType = hand.GetGrabStarting();
            if (startingGrabType == GrabTypes.Grip && hand.startingHandType == Hand.HandType.Left && holding == false) // detect if the squeeze was triggered and detect witch hand
            {
                laserBeam.Play(); // Play the laser beam
                grabbable.ForcePush(-1 * transform.forward, 200); // Apply a pulling force
                EP.Play(); // Play the zero point partile effect (At object)
                ZP.Play(); // Play the zero point partile effect (At hand)
                ZeroPoint.transform.LookAt(grabbable.transform); // Make the beam point at the object
                var main = ZP.main; // Get the main componate of the particle effect
                main.simulationSpeed = 10; // Speed up the simulation
                endWidth = 0.2f; // Set the laser end width
                StartCoroutine(Wait(1f)); // Wait for a second
            }

            if (startingGrabType == GrabTypes.Grip && hand.startingHandType == Hand.HandType.Right && holding == false) // detect if the squeeze was triggered and detect witch hand
            {
                laserBeam.Play();
                grabbable.ForcePush(transform.forward, 300); // Apply a pushing force
                EP.Play(); // Play the zero point partile effect (At object)
                ZP.Play(); // Play the zero point partile effect (At hand)
                ZeroPoint.transform.LookAt(grabbable.transform); // Make the beam point at the object
                var main = ZP.main; // Get the main componate of the particle effect
                main.simulationSpeed = 10; // Speed up the simulation
                endWidth = 0.2f; // Set the laser end width
                StartCoroutine(Wait(1f)); // Wait for a second
            }
        }
        lRender.endWidth = Mathf.Lerp(lRender.endWidth, endWidth, 15f * Time.deltaTime); // Lerp the create a transition for the laser endpoint size
    }

    IEnumerator Wait(float time) // Wait function
    {
        endWidth = 0.07f;  // Set the laser end width
        yield return new WaitForSeconds(time); // Time to wait
        laserBeam.Stop(); // Stop the visual beam
        EP.Stop(); // Stop the endpoint partile effect
        ZP.Stop(); // Stop the hand partile effect
        var main = ZP.main; // Get the main componate of the particle effect
        main.simulationSpeed = 1; // Slow down the simulation
    }

private Droppable RaycastForGrabbedObject() { // raycast function
        RaycastHit hit; // The hit point
        Ray r = new Ray(transform.position, transform.forward); // Create a new ray

        if (Physics.Raycast(r, out hit, Mathf.Infinity) && hit.collider.gameObject.GetComponent<Droppable>() != null) // If we have hit something that has the dropaable script attached
        {

            if (hit.distance > 1) // Make sure its more then one meter away so it wont break the normal grab
            {
                DisplayLine(true, hit.point); // Show the line between the hand and the object
                ZeroPoint.transform.LookAt(hit.point); // make the partile point at the object we hit
                return hit.collider.gameObject.GetComponent<Droppable>(); // Return the object we hit
            }
            else
            {
                DisplayLine(false, transform.position); 
                return null;
            }
            
        } else
        {
            DisplayLine(false, transform.position);
            return null;
        }
    }

    void DisplayLine (bool display, Vector3 endpoint) // Show the line between the hand and the object
    {
        lRender.enabled = display; // Turn it one
        lRender.material.color = new Color(1f, 0f, 0f, 0f); // Set the colour
        lRender.material.SetColor("_EmissionColor", Color.red * Mathf.LinearToGammaSpace(1.0f)); // Make it glow
        postions[0] = transform.position; // Set the postions
        postions[1] = endpoint; // Set the postions
        lRender.SetPositions(postions);  // Apply the postions to the actual line
    }

    void LaserLine(bool display) // Special laser line used the display the line between hand and object
    {
        endWidth = 0.07f; // Make the end width wider
        lRender.material.color = new Color(1f, 0f, 0f, 0.5f); // Set its colour               
        lRender.enabled = display; // Display it
    }
}
