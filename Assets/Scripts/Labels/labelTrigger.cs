using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class labelTrigger : MonoBehaviour
{
    GameObject[] objects;
    public void Start()
    {
        objects = GameObject.FindObjectsOfType<GameObject>(); // Get every object in the scene
    }
    
    public void hideLabels()
    {

        foreach (GameObject equiptLabel in objects) // For every label turn if off
        {
            if (equiptLabel) { 
                if (equiptLabel.name.Contains("Equipment Labels"))
                {
                    equiptLabel.SetActive(false);
                }
            }
        }
    }

    public void showLabels()
    {
        foreach (GameObject equiptLabel in objects) // For every label turn if on
        {
            if (equiptLabel)
            {
                if (equiptLabel.name.Contains("Equipment Labels"))
                {
                    equiptLabel.SetActive(true);
                }
            }
        }
    }
}
