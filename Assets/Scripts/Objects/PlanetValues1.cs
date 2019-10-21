using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanetValues1 : MonoBehaviour
{
    GameObject planetSettings;
    // Start is called before the first frame update
    void Start()
    {
        planetSettings = GameObject.Find("PlanetSettings"); // Get the planet settings
        if (planetSettings.GetComponent<PlanetSettings>().hasAtmos)
        {
            GameObject.Find("Atmos").GetComponent<Text>().text = "Yes";
        } else
        {
            GameObject.Find("Atmos").GetComponent<Text>().text = "None";
        }
        GameObject.Find("Radius").GetComponent<Text>().text = planetSettings.GetComponent<PlanetSettings>().radius; // Set the text
        GameObject.Find("Distance").GetComponent<Text>().text = planetSettings.GetComponent<PlanetSettings>().distanceToEarth; // Set the text
        GameObject.Find("Orbital").GetComponent<Text>().text = planetSettings.GetComponent<PlanetSettings>().orbitalPeriod; // Set the text
    }

}
