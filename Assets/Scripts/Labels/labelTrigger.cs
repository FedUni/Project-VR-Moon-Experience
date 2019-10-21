using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class labelTrigger : MonoBehaviour
{
    GameObject[] objects;
    public void Start()
    {
        objects = GameObject.FindObjectsOfType<GameObject>();
    }
    
    public void hideLabels()
    {
        foreach (GameObject equiptLabel in objects)
        {
            if (equiptLabel.name.Contains("Equipment Labels"))
            {
                equiptLabel.SetActive(false);
            }
        }
    }

    public void showLabels()
    {
        foreach (GameObject equiptLabel in objects)
        {
            if (equiptLabel.name.Contains("Equipment Labels"))
            {
                equiptLabel.SetActive(true);
            }
        }
    }
}
