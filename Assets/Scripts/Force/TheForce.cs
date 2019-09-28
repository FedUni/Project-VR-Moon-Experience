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
    void FixedUpdate()
    {
        if (!grabbed)
        {
            grabbable = RaycastForGrabbedObject();
            if (!grabbable) return;
        }

        Vector3 curHandPos = transform.position;

        GrabTypes startingGrabType = hand.GetGrabStarting();
        if (startingGrabType == GrabTypes.Grip) // Force grab
        {
            isGripping = true;
            grabbed = true;
            grabbable.Grab(true);
            grabbable.SetMoveScale(transform.position);
            lastHandPos = curHandPos;
            DisplayLine(false, transform.position);
        }

        if (hand.IsGrabbingWithType(GrabTypes.Grip)) // Force move 
        {
            grabbable.Move(curHandPos, lastHandPos);
        }

        GrabTypes endingGrabType = hand.GetGrabEnding();
        if (endingGrabType == GrabTypes.Grip) // Release
        {
            isGripping = false;
            grabbed = false;
            grabbable.Grab(false);            
        }
        lastHandPos = curHandPos;

        if (!isGripping) // Fix for hand hover bug
        {
            grabbable.Grab(false);
        }

        GrabTypes pushPullGrabType = hand.GetGrabStarting();
        if (startingGrabType == GrabTypes.Pinch && hand.startingHandType == Hand.HandType.Left)
        {
            grabbable.ForcePush(-1 * transform.forward, 200);
        }

        if (startingGrabType == GrabTypes.Pinch && hand.startingHandType == Hand.HandType.Right)
        {
            grabbable.ForcePush(transform.forward, 300);
        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.5f);
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
