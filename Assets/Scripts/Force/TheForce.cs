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
    private Quaternion lastHandRot;
    private Quaternion lastObjectRot;
    private Vector3 lastHandPos;
    private PlanetSettings planetsettings;
    //private bool forceEnabled;


    // Start is called before the first frame update
    void Start()
    {
        controller = SteamVR_Controller.Input((int) trackedObj.index);
        postions = new Vector3[2];
        planetsettings = GameObject.Find("PlanetSettings").GetComponent<PlanetSettings>();
        //forceEnabled = planetsettings.forceEnabled;
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
                grabbable.Grab(true);
                grabbable.SetMoveScale(transform.position);
                lastHandPos = curHandPos;
                lastHandRot = curHandRot;
                DisplayLine(false, transform.position);
            }

            if (hand.IsGrabbingWithType(GrabTypes.Pinch)) // Force move 
            {
                grabbable.Move(curHandPos, lastHandPos, curHandRot, lastHandRot, lastObjectRot);
            }

            GrabTypes endingGrabType = hand.GetGrabEnding();
            if (endingGrabType == GrabTypes.Pinch) // Release
            {
                grabbed = false;
                grabbable.Grab(false);
            }

            lastHandPos = curHandPos;
            lastHandRot = curHandRot;


            GrabTypes pushPullGrabType = hand.GetGrabStarting();
            if (startingGrabType == GrabTypes.Grip && hand.startingHandType == Hand.HandType.Left)
            {
                grabbable.ForcePush(-1 * transform.forward, 200);
            }

            if (startingGrabType == GrabTypes.Grip && hand.startingHandType == Hand.HandType.Right)
            {
                grabbable.ForcePush(transform.forward, 300);
            }
        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.5f);
    }

    private Droppable RaycastForGrabbedObject() {
        RaycastHit hit;
        Ray r = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(r, out hit, Mathf.Infinity) && hit.collider.gameObject.GetComponent<Droppable>() != null)
        {

            if (hit.distance > 1)
            {
                DisplayLine(true, hit.point);
                lastObjectRot = hit.collider.gameObject.transform.rotation;
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
        postions[0] = transform.position;
        postions[1] = endpoint;
        lRender.SetPositions(postions);
    }
}
