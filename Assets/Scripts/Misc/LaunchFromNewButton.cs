using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR.InteractionSystem;
using UnityEngine.Rendering;

public class LaunchFromNewButton : MonoBehaviour
{
    public GameObject rocket;
    public AudioSource countdown;
    // Start is called before the first frame update
    void Start()
    {
        rocket = GameObject.Find("saturn_V_final_fbx_exp");
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();
        if (startingGrabType != GrabTypes.None)
        {
            rocket.GetComponent<Rocket>().launch();
            countdown.Play();
        }
    }
}
