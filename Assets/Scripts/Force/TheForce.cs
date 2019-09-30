using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using System;

public class TheForce : MonoBehaviour
{
    private SteamVR_Controller.Device controller;
    public LineRenderer lRender;
    private Vector3[] postions;
    private Droppable grabbable;
    private bool grabbed;
    public SteamVR_TrackedObject trackedObj;
    public Hand hand;
    private Vector3 lastHandPos;
    private PlanetSettings planetsettings;
    public ParticleSystem laserBeam;
    private ParticleSystem ZP;
    private ParticleSystem EP;
    public GameObject endpointSpark;
    public GameObject ZeroPoint;
    private float endWidth = 0.07f;
    private float speed = 10f;
    private bool holding = false;


    // Start is called before the first frame update
    void Start()
    {
        controller = SteamVR_Controller.Input((int) trackedObj.index);
        postions = new Vector3[2];
        planetsettings = GameObject.Find("PlanetSettings").GetComponent<PlanetSettings>();
        ZP = ZeroPoint.GetComponent<ParticleSystem>();
        EP = endpointSpark.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (planetsettings.forceEnabled)
        {
            if (!grabbed)
            {
                grabbable = RaycastForGrabbedObject();
                if (!grabbable) return;
            }

            Vector3 curHandPos = transform.position;
            Quaternion curHandRot = transform.rotation;
            GrabTypes startingGrabType = hand.GetGrabStarting();
            if (startingGrabType == GrabTypes.Pinch) // Force grab
            {
                grabbed = true;
                grabbable.Grab(true); // Change this to true to type two force mode
                grabbable.SetMoveScale(transform.position);
                lastHandPos = curHandPos;
                DisplayLine(false, transform.position);
                LaserLine(false);
                laserBeam.Play();
                EP.Play();
                ZP.Play();
                ZeroPoint.transform.LookAt(grabbable.transform);
                speed = 10;
                holding = true;
            }

            if (hand.IsGrabbingWithType(GrabTypes.Pinch) && holding == true) // Force move 
            {
                grabbable.Move(curHandPos, lastHandPos);
                postions[0] = transform.position;
                postions[1] = grabbable.transform.position;
                endpointSpark.transform.position = grabbable.transform.position;
                lRender.SetPositions(postions);
                ZeroPoint.transform.LookAt(grabbable.transform);
            }

            GrabTypes endingGrabType = hand.GetGrabEnding();
            if (endingGrabType == GrabTypes.Pinch) // Release
            {
                grabbed = false;
                grabbable.Grab(false);
                endWidth = 0.07f;
                laserBeam.Stop();
                EP.Stop();
                ZP.Stop();
                holding = false;
                            
            }

            lastHandPos = curHandPos;

            GrabTypes pushPullGrabType = hand.GetGrabStarting();
            if (startingGrabType == GrabTypes.Grip && hand.startingHandType == Hand.HandType.Left && holding == false)
            {
                laserBeam.Play();
                grabbable.ForcePush(-1 * transform.forward, 200);
                EP.Play();
                ZP.Play();
                ZeroPoint.transform.LookAt(grabbable.transform);
                var main = ZP.main;
                main.simulationSpeed = 10;
                endWidth = 0.2f;
                speed = 100f;
                StartCoroutine(Wait(1f));
            }

            if (startingGrabType == GrabTypes.Grip && hand.startingHandType == Hand.HandType.Right && holding == false)
            {
                laserBeam.Play();
                grabbable.ForcePush(transform.forward, 300);
                EP.Play();
                ZP.Play();
                ZeroPoint.transform.LookAt(grabbable.transform);
                var main = ZP.main;
                main.simulationSpeed = 10;
                endWidth = 0.2f;
                speed = 100f;
                StartCoroutine(Wait(1f));
            }
        }
        //lRender.endWidth = Mathf.PingPong(5f * Time.deltaTime, 0.5f);
        lRender.endWidth = Mathf.Lerp(lRender.endWidth, endWidth, 15f * Time.deltaTime);
    }

    IEnumerator Wait(float time)
    {
        endWidth = 0.07f;
        yield return new WaitForSeconds(time);
        laserBeam.Stop();
        EP.Stop();
        ZP.Stop();
        var main = ZP.main;
        main.simulationSpeed = 1;
        speed = 10f;
    }

private Droppable RaycastForGrabbedObject() {
        RaycastHit hit;
        Ray r = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(r, out hit, Mathf.Infinity) && hit.collider.gameObject.GetComponent<Droppable>() != null)
        {

            if (hit.distance > 1)
            {
                DisplayLine(true, hit.point);
                ZeroPoint.transform.LookAt(hit.point);
                return hit.collider.gameObject.GetComponent<Droppable>();
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

    void DisplayLine (bool display, Vector3 endpoint)
    {
        lRender.enabled = display;
        lRender.material.color = new Color(1f, 0f, 0f, 0f);
        lRender.material.SetColor("_EmissionColor", Color.red * Mathf.LinearToGammaSpace(1.0f));
        postions[0] = transform.position;
        postions[1] = endpoint;
        lRender.SetPositions(postions);
    }

    void LaserLine(bool display)
    {
        endWidth = 0.07f;
        lRender.material.color = new Color(1f, 0f, 0f, 0.5f);               
        lRender.enabled = display;
    }
}
