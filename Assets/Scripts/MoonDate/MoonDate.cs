using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

//Created by Hein for the Moon VR 3.0 Project
//Dont hate, I did it in a hurry haha

public class MoonDate : MonoBehaviour
{

    private Canvas MoonDateCanvas;
    private Text MoonDateText;

    string[] distances = new string[] {        
            "365,964KM",       
            "360,464KM",        
            "357,123KM",       
            "356,909KM",        
            "359,656KM",       
            "364,366KM",        
            "363,513KM",        
            "368,367KM",        
            "359,081KM",        
            "361,316KM",        
            "366,721KM",        
            "370,260KM",       
        };


    public void Calculate()
    {
        setText(distances[System.DateTime.Now.Month] + 1);        
    }

    private void setText(string distance) {
        GameObject.Find("LaserSignGolf").GetComponentInChildren<Text>().text = "Distance to Earth  <color=#00FFFF>" + distance + "</color> See how far you can get!";
        GameObject.Find("LaserSignExp").GetComponent<Text>().text = "Lunar Laser Ranging Experiment Distance to Earth is <color=#00FFFF>" + distance + "</color>";
        if (GameObject.Find("LaserActive") != null) // This is for when the controller are turned off
        {
            GameObject.Find("LaserDist").GetComponent<Text>().text = distance; // Set the text
            GameObject.Find("LaserTime").GetComponent<Text>().text = "1.3 Seconds"; // Set the text
        }
    }

  
    

    
}







