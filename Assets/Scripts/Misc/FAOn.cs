using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FAOn : MonoBehaviour
{
    public void ChangeFAState (bool state)
    {
        GameObject.Find("PlanetSettings").GetComponent<PlanetSettings>().forceEnabled = state; // Set the force this is done via the watch
    }
}
