using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    float rangeDistance = 0;
    private Text catapultText;
    private Canvas catapultCanvas;
    public Vector3 postion;

    // Start is called before the first frame update
    void Start()
    {
        glowCanvas = gameObject.GetComponentInChildren<Canvas>(); // Get the canvas used for the glow
        catapultText = GameObject.Find("CatapultDistance").GetComponentInChildren<Text>();
        catapultCanvas = GameObject.Find("CatapultDistance").GetComponentInChildren<Canvas>();
        canvasRec = glowCanvas.GetComponent<RectTransform>(); // Get the transform of the glow canvas
        scale = canvasRec.localScale; // Get its strating scale
        catapult = GameObject.Find("Spoon").transform; // Get the catapult arm
        lookCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {

        if (isLerping)
        {
            distance = Vector3.Distance(Camera.main.transform.position, gameObject.transform.position);
            rangeDistance = Vector3.Distance(catapult.transform.position, gameObject.transform.position);
            glowCanvas.enabled = true;
            glowCanvas.GetComponent<RectTransform>().localScale = scale * distance; // Scale up as it get futher away
            catapultText.text = "The object has flown " + Mathf.RoundToInt(rangeDistance - 3.0f) + "m down range";

            catapultCanvas.GetComponent<RectTransform>().localPosition = Vector3.Lerp(catapultCanvas.GetComponent<RectTransform>().localPosition, postion, 5 * Time.deltaTime);

        }

        if (isDebug) // If we are using the 2d debug mode this needs to be turn off or the canvas will still face the vr camera
        {
            lookCamera = Camera.main;
        }
        canvasRec.LookAt(transform.position + lookCamera.transform.rotation * Vector3.back, lookCamera.transform.rotation * Vector3.up); // Look at the camera
        canvasRec.Rotate(0, 180, 0); // Turn 180 to face the camera
    }


    private void OnCollisionStay(Collision collision)
    {
        if (collision.GetContact(0).otherCollider.name == "Terrain")
        {            
            glowCanvas.enabled = false; // Turn the canvas off when it hits the grounb
            StartCoroutine(retractDistanceCanvas()); // Start to wait function
        }
    }

    public IEnumerator retractDistanceCanvas()
    {
        postion = new Vector3(1.563f, 3.614f, 1.628f);
        yield return new WaitForSeconds(1f); // Show the distacne canvas for a bit
        isLerping = false; // Stop lerping
    }

    private void HandHoverUpdate(Hand hand)
    {
        isLerping = false;
        GrabTypes startingGrabType = hand.GetGrabStarting();
        if (startingGrabType != GrabTypes.None) // When grabbed turn the glow off
        {
            glowCanvas.GetComponent<RectTransform>().localScale = scale;
            glowCanvas.enabled = false; // Turn the canvas off
            isLerping = false; // Stop lerping

        }
    }
}
