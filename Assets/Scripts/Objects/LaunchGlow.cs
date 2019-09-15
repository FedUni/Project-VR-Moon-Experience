using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using System;

public class LaunchGlow : MonoBehaviour
{
    public bool isLerping = false;
    private Canvas glowCanvas;
    private Vector3 scale;
    private Transform catapult;
    private Camera lookCamera;
    private RectTransform canvasRec;
    public bool isDebug = false;
    float distance = 0;

    // Start is called before the first frame update
    void Start()
    {
        glowCanvas = gameObject.GetComponentInChildren<Canvas>();
        canvasRec = glowCanvas.GetComponent<RectTransform>();
        scale = canvasRec.localScale;
        catapult = GameObject.Find("Catapult arm").transform;
        lookCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {

        if (isLerping)
        {
            distance = Vector3.Distance(Camera.main.transform.position, gameObject.transform.position);
            glowCanvas.enabled = true;
            glowCanvas.GetComponent<RectTransform>().localScale = scale * distance;
           
        }

        if (isDebug)
        {
            lookCamera = Camera.main;
        }
        canvasRec.LookAt(transform.position + lookCamera.transform.rotation * Vector3.back, lookCamera.transform.rotation * Vector3.up);
        canvasRec.Rotate(0, 180, 0);
    }


    private void OnCollisionStay(Collision collision)
    {
        if (collision.GetContact(0).otherCollider.name == "Terrain")
        {
            //glowCanvas.GetComponent<RectTransform>().localScale = scale;
            isLerping = false;
        }
    }

    private void HandHoverUpdate(Hand hand)
    {
        isLerping = false;
        GrabTypes startingGrabType = hand.GetGrabStarting();
        if (startingGrabType != GrabTypes.None)
        {
            glowCanvas.GetComponent<RectTransform>().localScale = scale;
            glowCanvas.enabled = false;
            isLerping = false;

        }
    }
}
