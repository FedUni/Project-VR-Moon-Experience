﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.Extras;

public class LaserPointerHandler : MonoBehaviour
{
    public bool selected;
    public SteamVR_LaserPointer laserPointer;
    // Start is called before the first frame update
    void Start()
    {
        laserPointer.PointerIn += PointerInside;
        laserPointer.PointerOut += PointerOutside;
        selected = false;
    }
    // Update is called once per frame
    void Update()
    {

    }
    public void PointerInside(object sender, PointerEventArgs e)
    {

        if (e.target.name == this.gameObject.name && selected == false)
        {
            selected = true;
            Debug.Log("pointer is inside this object" + e.target.name);
        }
    }
    public void PointerOutside(object sender, PointerEventArgs e)
    {

        if (e.target.name == this.gameObject.name && selected == true)
        {
            selected = false;
            Debug.Log("pointer is outside this object" + e.target.name);
        }
    }
    public bool get_selected_value()
    {
        return selected;
    }
}
