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
 


    void Start()
    {
        string month= System.DateTime.Now.Month.ToString();
        int monthInt = Int32.Parse(month);
        string DeafultMonth = "Unable to get distance";
        if (monthInt == 1)
        {
            MoonDateCanvas = GameObject.Find("LaserSignGolf").GetComponent<Canvas>();
            MoonDateText = GameObject.Find("LaserSignGolf").GetComponentInChildren<Text>();
            MoonDateText.text = "Distance to Earth  <color=#00FFFF>" + "365,964KM" + "</color> See how far you can get!";
        }
        if (monthInt == 2)
        {
            MoonDateCanvas = GameObject.Find("LaserSignGolf").GetComponent<Canvas>();
            MoonDateText = GameObject.Find("LaserSignGolf").GetComponentInChildren<Text>();
            MoonDateText.text = "Distance to Earth  <color=#00FFFF>" + "360,464KM" + "</color> See how far you can get!";
        }
        if (monthInt == 3)
        {
            MoonDateCanvas = GameObject.Find("LaserSignGolf").GetComponent<Canvas>();
            MoonDateText = GameObject.Find("LaserSignGolf").GetComponentInChildren<Text>();
            MoonDateText.text = "Distance to Earth  <color=#00FFFF>" + "357,123KM" + "</color> See how far you can get!";
        }
        if (monthInt == 4)
        {
            MoonDateCanvas = GameObject.Find("LaserSignGolf").GetComponent<Canvas>();
            MoonDateText = GameObject.Find("LaserSignGolf").GetComponentInChildren<Text>();
            MoonDateText.text = "Distance to Earth  <color=#00FFFF>" + "356,909KM" + "</color> See how far you can get!";
        }
        if (monthInt == 5)
        {
            MoonDateCanvas = GameObject.Find("LaserSignGolf").GetComponent<Canvas>();
            MoonDateText = GameObject.Find("LaserSignGolf").GetComponentInChildren<Text>();
            MoonDateText.text = "Distance to Earth  <color=#00FFFF>" + "359,656KM " + "</color> See how far you can get!";
        }
        if (monthInt == 6)
        {
            MoonDateCanvas = GameObject.Find("LaserSignGolf").GetComponent<Canvas>();
            MoonDateText = GameObject.Find("LaserSignGolf").GetComponentInChildren<Text>();
            MoonDateText.text = "Distance to Earth  <color=#00FFFF>" + "3643,66KM" + "</color> See how far you can get!";
        }
        if (monthInt == 7)
        {
            MoonDateCanvas = GameObject.Find("LaserSignGolf").GetComponent<Canvas>();
            MoonDateText = GameObject.Find("LaserSignGolf").GetComponentInChildren<Text>();
            MoonDateText.text = "Distance to Earth  <color=#00FFFF>" + "363,513KM" + "</color> See how far you can get!";
        }
        if (monthInt == 8)
        {
            MoonDateCanvas = GameObject.Find("LaserSignGolf").GetComponent<Canvas>();
            MoonDateText = GameObject.Find("LaserSignGolf").GetComponentInChildren<Text>();
            MoonDateText.text = "Distance to Earth  <color=#00FFFF>" + "368,367KM" + "</color> See how far you can get!";
        }
        if (monthInt == 9) { 
            MoonDateCanvas = GameObject.Find("LaserSignGolf").GetComponent<Canvas>();
            MoonDateText = GameObject.Find("LaserSignGolf").GetComponentInChildren<Text>();
            MoonDateText.text = "Distance to Earth  <color=#00FFFF>" + "359,081KM" + "</color> See how far you can get!";
        }
        if (monthInt == 10)
        {
            MoonDateCanvas = GameObject.Find("LaserSignGolf").GetComponent<Canvas>();
            MoonDateText = GameObject.Find("LaserSignGolf").GetComponentInChildren<Text>();
            MoonDateText.text = "Distance to Earth  <color=#00FFFF>" + "361,316KM" + "</color> See how far you can get!";
        }
        if (monthInt == 11)
        {
            MoonDateCanvas = GameObject.Find("LaserSignGolf").GetComponent<Canvas>();
            MoonDateText = GameObject.Find("LaserSignGolf").GetComponentInChildren<Text>();
            MoonDateText.text = "Distance to Earth  <color=#00FFFF>" + "361,316KM" + "</color> See how far you can get!";
        }
        if (monthInt == 12)
        {
            MoonDateCanvas = GameObject.Find("LaserSignGolf").GetComponent<Canvas>();
            MoonDateText = GameObject.Find("LaserSignGolf").GetComponentInChildren<Text>();
            MoonDateText.text = "Distance to Earth  <color=#00FFFF>" + "370,260KM" + "</color> See how far you can get!";
        }
    }

  
    

    
}







