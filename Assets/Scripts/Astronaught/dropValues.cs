using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class dropValues : MonoBehaviour
{
    GameObject DropRig;
    // Start is called before the first frame update
    void Start()
    {
        DropRig = GameObject.Find("DropRig"); // Get the planet settings
        
        //GameObject.Find("WatchDropLeft").GetComponent<Text>().text = planetSettings.GetComponent<PlanetSettings>().radius; // Set the text
        //GameObject.Find("WatchDropRight").GetComponent<Text>().text = planetSettings.GetComponent<PlanetSettings>().distanceToEarth; // Set the text
    }

    // Update is called once per frame
    void Update()
    {
        GameObject.Find("WatchDropHeight").GetComponent<Text>().text = (int)DropRig.GetComponent<Animator>().GetFloat("wingHeight") + "m"; // Set the text 
    }
}
