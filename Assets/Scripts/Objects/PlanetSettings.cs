using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetSettings : MonoBehaviour
{
    Vector3 moonGravity = new Vector3(0.0f, -1.62f, 0.0f); // Planets gavity
    Vector3 marsGravity = new Vector3(0.0f, -3.711f, 0.0f); // Planets gavity
    Vector3 earthGravity = new Vector3(0.0f, -9.82f, 0.0f); // Planets gavity
    public bool isMars;
    public bool isMoon;
    public bool isEarth;
    public bool hasAtmos; // If the planet has an atmossphere
    public string radius; // Planets radious
    public string distanceToEarth; // Planets distance from earth
    public string orbitalPeriod; // Planets orbnital period
    public string density; // Planets density
    public string excapeVelocity; // Planets excape velocity
    public string mass; // Planets mass
    public string lenthOfDay; // Planets lenth of day
    public float gravity; // Multiplier for gravity based on earths gravity
    public bool forceEnabled;
    // Start is called before the first frame update
    void Start()
    {
        if (isMars) {
            Physics.gravity = marsGravity;
            hasAtmos = false;
            radius = "3389.5km";
            distanceToEarth = "54600000km";
            orbitalPeriod = "687 Days";
            density = "3.93g/cm³";
            excapeVelocity = "5000m/s";
            mass = "6.4169 x 10^23 kg";
            lenthOfDay = "1d 0h 37m";
        }

        if (isMoon)
        {
            Physics.gravity = moonGravity;
            hasAtmos = false;
            radius = "1737.1km";
            distanceToEarth = "384400km";
            orbitalPeriod = "27 Days";
            density = "1.62g/cm³";
            excapeVelocity = "2.38m/s";
            mass = "7.35 x 10^22 kg";
            lenthOfDay = "29.5 Days";
        }

        if (isEarth)
        {
            Physics.gravity = earthGravity;
            hasAtmos = true;
            radius = "6371km";
            distanceToEarth = "0km";
            orbitalPeriod = "365.25 Days";
            density = "5.51g/cm^3";
            excapeVelocity = "11.2m/s";
            mass = "5.972 × 10^24 kg";
            lenthOfDay = "0d 23hrs 56mins";
        }
    }

}
