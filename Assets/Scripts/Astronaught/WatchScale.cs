using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.Extras;
using UnityEngine.EventSystems;

public class WatchScale : MonoBehaviour
{
    public bool isEnabled;
    public RectTransform canvas;
    Vector3 scale;
    Vector3 postition;
 
    public void onEnter ()
    {
        if (isEnabled) // Check to make shure the watch expecnd is turned on
        {
            canvas.localScale = new Vector3(0.0008074478f, 0.0008074483f, 0.0002515115f); // Set the enlarged scale
            canvas.localPosition = new Vector3(89.43001f, 222.684f, 132.24f); // Set the enlarged postion
        }
    }

    public void onExit()
    {
        if (isEnabled) // Check to make shure the watch expecnd is turned on
        {
            canvas.localScale = new Vector3(0.0001217484f, 0.0001217486f, 7.704442e-05f); // Set the normal scale
            canvas.localPosition = new Vector3(-0.0025f, 0.0277f, -0.0003f); // Set the normal scale
        }
    }

}
