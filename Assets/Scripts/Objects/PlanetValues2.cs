using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanetValues2 : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject planetSettings;
    void Start()
    {
        planetSettings = GameObject.Find("PlanetSettings"); // Get the planet settings
        GameObject.Find("Density").GetComponent<Text>().text = planetSettings.GetComponent<PlanetSettings>().density.ToString(); // Set the text
        GameObject.Find("Mass").GetComponent<Text>().text = planetSettings.GetComponent<PlanetSettings>().mass; // Set the text
        GameObject.Find("EscVol").GetComponent<Text>().text = planetSettings.GetComponent<PlanetSettings>().excapeVelocity.ToString(); // Set the text
        GameObject.Find("DayLength").GetComponent<Text>().text = planetSettings.GetComponent<PlanetSettings>().lenthOfDay; // Set the text
    }

}
