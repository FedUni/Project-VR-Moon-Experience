﻿using System.Collections;
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
 


    void Start()
    { MoonDateText.text = "Distance to Earth  <color=#00FFFF>" + "365,964KM" + "</color> See how far you can get!";
        string month= System.DateTime.Now.Month.ToString();
        int monthInt = Int32.Parse(month);
        string DeafultMonth = "Unable to get distance";
        MoonDateText.text = "Distance to Earth  <color=#00FFFF>Unknown</color> Activate the laser module to find out!";
        if (monthInt == 1)
        {
            MoonDateCanvas = GameObject.Find("LaserSignGolf").GetComponent<Canvas>();
            MoonDateText = GameObject.Find("LaserSignGolf").GetComponentInChildren<Text>();
            GameObject.Find("LaserSignExp").GetComponent<Text>().text = "Lunar Laser Ranging Experiment Distance to Earth is <color=#00FFFF>365,964KM</color>"; // Set the text
            MoonDateText.text = "Distance to Earth  <color=#00FFFF>" + "365,964KM" + "</color> See how far you can get!";
        }
        if (monthInt == 2)
        {
            MoonDateCanvas = GameObject.Find("LaserSignGolf").GetComponent<Canvas>();
            MoonDateText = GameObject.Find("LaserSignGolf").GetComponentInChildren<Text>();
            GameObject.Find("LaserSignExp").GetComponent<Text>().text = "Lunar Laser Ranging Experiment Distance to Earth is <color=#00FFFF>360,464KM</color>"; // Set the text
            MoonDateText.text = "Distance to Earth  <color=#00FFFF>" + "360,464KM" + "</color> See how far you can get!";
        }
        if (monthInt == 3)
        {
            MoonDateCanvas = GameObject.Find("LaserSignGolf").GetComponent<Canvas>();
            MoonDateText = GameObject.Find("LaserSignGolf").GetComponentInChildren<Text>();
            GameObject.Find("LaserSignExp").GetComponent<Text>().text = "Lunar Laser Ranging Experiment Distance to Earth is <color=#00FFFF>357,123KM</color>"; // Set the text
            MoonDateText.text = "Distance to Earth  <color=#00FFFF>" + "357,123KM" + "</color> See how far you can get!";
        }
        if (monthInt == 4)
        {
            MoonDateCanvas = GameObject.Find("LaserSignGolf").GetComponent<Canvas>();
            MoonDateText = GameObject.Find("LaserSignGolf").GetComponentInChildren<Text>();
            GameObject.Find("LaserSignExp").GetComponent<Text>().text = "Lunar Laser Ranging Experiment Distance to Earth is <color=#00FFFF>356,909KM</color>"; // Set the text
            MoonDateText.text = "Distance to Earth  <color=#00FFFF>" + "356,909KM" + "</color> See how far you can get!";
        }
        if (monthInt == 5)
        {
            MoonDateCanvas = GameObject.Find("LaserSignGolf").GetComponent<Canvas>();
            MoonDateText = GameObject.Find("LaserSignGolf").GetComponentInChildren<Text>();
            GameObject.Find("LaserSignExp").GetComponent<Text>().text = "Lunar Laser Ranging Experiment Distance to Earth is <color=#00FFFF>359,656KM</color>"; // Set the text
            MoonDateText.text = "Distance to Earth  <color=#00FFFF>" + "359,656KM " + "</color> See how far you can get!";
        }
        if (monthInt == 6)
        {
            MoonDateCanvas = GameObject.Find("LaserSignGolf").GetComponent<Canvas>();
            MoonDateText = GameObject.Find("LaserSignGolf").GetComponentInChildren<Text>();
            GameObject.Find("LaserSignExp").GetComponent<Text>().text = "Lunar Laser Ranging Experiment Distance to Earth is <color=#00FFFF>364,366KM</color>"; // Set the text
            MoonDateText.text = "Distance to Earth  <color=#00FFFF>" + "364,366KM" + "</color> See how far you can get!";
        }
        if (monthInt == 7)
        {
            MoonDateCanvas = GameObject.Find("LaserSignGolf").GetComponent<Canvas>();
            MoonDateText = GameObject.Find("LaserSignGolf").GetComponentInChildren<Text>();
            GameObject.Find("LaserSignExp").GetComponent<Text>().text = "Lunar Laser Ranging Experiment Distance to Earth is <color=#00FFFF>363,513KM</color>"; // Set the text
            MoonDateText.text = "Distance to Earth  <color=#00FFFF>" + "363,513KM" + "</color> See how far you can get!";
        }
        if (monthInt == 8)
        {
            MoonDateCanvas = GameObject.Find("LaserSignGolf").GetComponent<Canvas>();
            MoonDateText = GameObject.Find("LaserSignGolf").GetComponentInChildren<Text>();
            GameObject.Find("LaserSignExp").GetComponent<Text>().text = "Lunar Laser Ranging Experiment Distance to Earth is <color=#00FFFF>368,367K</color>"; // Set the text
            MoonDateText.text = "Distance to Earth  <color=#00FFFF>" + "368,367KM" + "</color> See how far you can get!";
        }
        if (monthInt == 9) { 
            MoonDateCanvas = GameObject.Find("LaserSignGolf").GetComponent<Canvas>();
            MoonDateText = GameObject.Find("LaserSignGolf").GetComponentInChildren<Text>();
            GameObject.Find("LaserSignExp").GetComponent<Text>().text = "Lunar Laser Ranging Experiment Distance to Earth is <color=#00FFFF>359,081KM</color>"; // Set the text
            MoonDateText.text = "Distance to Earth  <color=#00FFFF>" + "359,081KM" + "</color> See how far you can get!";
        }
        if (monthInt == 10)
        {
            MoonDateCanvas = GameObject.Find("LaserSignGolf").GetComponent<Canvas>();
            MoonDateText = GameObject.Find("LaserSignGolf").GetComponentInChildren<Text>();
            GameObject.Find("LaserSignExp").GetComponent<Text>().text = "Lunar Laser Ranging Experiment Distance to Earth is <color=#00FFFF>361,316KM</color>"; // Set the text
            MoonDateText.text = "Distance to Earth  <color=#00FFFF>" + "361,316KM" + "</color> See how far you can get!";
        }
        if (monthInt == 11)
        {
            MoonDateCanvas = GameObject.Find("LaserSignGolf").GetComponent<Canvas>();
            MoonDateText = GameObject.Find("LaserSignGolf").GetComponentInChildren<Text>();
            GameObject.Find("LaserSignExp").GetComponent<Text>().text = "Lunar Laser Ranging Experiment Distance to Earth is <color=#00FFFF>366,721KM</color>"; // Set the text
            MoonDateText.text = "Distance to Earth  <color=#00FFFF>" + "366,721KM" + "</color> See how far you can get!";
        }
        if (monthInt == 12)
        {
            MoonDateCanvas = GameObject.Find("LaserSignGolf").GetComponent<Canvas>();
            MoonDateText = GameObject.Find("LaserSignGolf").GetComponentInChildren<Text>();
            GameObject.Find("LaserSignExp").GetComponent<Text>().text = "Lunar Laser Ranging Experiment Distance to Earth is <color=#00FFFF>370,260KM</color>"; // Set the text
            MoonDateText.text = "Distance to Earth  <color=#00FFFF>" + "370,260KM" + "</color> See how far you can get!";
        }
    }

  
    

    
}







