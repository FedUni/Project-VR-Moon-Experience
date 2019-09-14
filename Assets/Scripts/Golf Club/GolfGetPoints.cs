using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using UnityEngine.UI;
using System;

//Created by Hein for the Moon VR 3.0 Project

public class GolfGetPoints : MonoBehaviour
{
//public  static float dropTime = Time.time;
    public static bool hasBeenHit = false;
    public static  bool hit = false;
    public static double hitScore = 0;
    //public static double hitScore = Math.Round(Time.time);
    bool isFalling = false;

    void Start()
    {

    }


    
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "GolfClub")
        {
            hasBeenHit = true;
            isFalling = true;
        }
    }

    void Update()
    {
        if (hasBeenHit && isFalling == false)
        {
           // dropTime = Time.time;
            isFalling = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.GetContact(0).otherCollider.name == "Terrain" && hasBeenHit == true)
        {
            hit = true;

        }



    }
}




