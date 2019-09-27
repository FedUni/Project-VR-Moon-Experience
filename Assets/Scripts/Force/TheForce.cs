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
    private GrabObject grabbable;
    private bool grabbed;
    public SteamVR_TrackedObject trackedObj;
    public Hand hand;
    private bool isGripping = false;

    private Vector3 lastHandPos;


    // Start is called before the first frame update
    void Start()
    {
        //SteamVR_TrackedObject trackedObj = GetComponent<SteamVR_TrackedObject>();
        controller = SteamVR_Controller.Input((int) trackedObj.index);
        //lRender = GetComponent<LineRenderer>();
        postions = new Vector3[2];
       
    }

    // Update is called once per frame
    void Update()
    {
        if (!grabbed)
        {
            grabbable = RaycastForGrabbedObject();
            if (!grabbable) return;
        }

        Vector3 curHandPos = transform.position;

        GrabTypes startingGrabType = hand.GetGrabStarting();
        if (startingGrabType == GrabTypes.Pinch) // Force grab
        {
            isGripping = true;
            grabbed = true;
            grabbable.Grab(true);
            lastHandPos = curHandPos;
            DisplayLine(false, transform.position);
        }
        if (isGripping) // Force move 
        {
            grabbable.Move(curHandPos, lastHandPos);  
        }
        GrabTypes pushPullGrabType = hand.GetGrabStarting();
        if (startingGrabType == GrabTypes.Grip)
        {            
            grabbable.ForcePush(transform.forward, 300);           
        }

        GrabTypes endingGrabType = hand.GetGrabEnding();
        if (endingGrabType == GrabTypes.Pinch) // Release
        {
            isGripping = false;
            grabbed = false;
            grabbable.Grab(false);            
        }
        lastHandPos = curHandPos;
        if (!isGripping)
        {
            grabbable.Grab(false);
        }
    }

    private GrabObject RaycastForGrabbedObject() {
        RaycastHit hit;
        Ray r = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(r, out hit, Mathf.Infinity) && hit.collider.gameObject.GetComponent<GrabObject>() != null)
        {
            if (!isGripping)
            {
                DisplayLine(true, hit.point);                
            }
            return hit.collider.gameObject.GetComponent<GrabObject>();
        } else
        {
            DisplayLine(false, transform.position);
            return null;
        }
    }

    void DisplayLine (bool display, Vector3 endpoint)
    {
        lRender.enabled = display;
        postions[0] = transform.position;
        postions[1] = endpoint;
        lRender.SetPositions(postions);
    }
}
