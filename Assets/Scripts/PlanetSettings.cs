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
    public float radius; // Planets radious
    public int distanceToEarth; // Planets distance from earth
    public float orbitalPeriod; // Planets orbnital period
    public float density; // Planets density
    public float excapeVelocity; // Planets excape velocity
    public string mass; // Planets mass
    public string lenthOfDay; // Planets lenth of day
    public float gravity; // Multiplier for gravity based on earths gravity
    // Start is called before the first frame update
    void Start()
    {
        if (isMars) {
            Physics.gravity = marsGravity;
            hasAtmos = false;
            radius = 3389.5f;
            distanceToEarth = 54600000;
            orbitalPeriod = 687;
            density = 3.93f;
            excapeVelocity = 5000;
            mass = "6.4169 x 10^23 kg";
            lenthOfDay = "1d 0h 37m";
        }

        if (isMoon)
        {
            Physics.gravity = moonGravity;
            hasAtmos = false;
            radius = 1737.1f;
            distanceToEarth = 384400;
            orbitalPeriod = 27;
            density = 1.62f;
            excapeVelocity = 2.38f;
            mass = "7.35 x 10^22 kg";
            lenthOfDay = "29.5 Days";
        }

        if (isEarth)
        {
            Physics.gravity = earthGravity;
            hasAtmos = true;
            radius = 6371;
            distanceToEarth = 0;
            orbitalPeriod = 365.25f;
            density = 5.51f;
            excapeVelocity = 11.2f;
            mass = "5.972 × 10^24 kg";
            lenthOfDay = "0d 23hrs 56mins";
        }
    }

    private void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
