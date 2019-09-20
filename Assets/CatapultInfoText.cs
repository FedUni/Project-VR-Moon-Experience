using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CatapultInfoText : MonoBehaviour {
    private Text[] catapultTexts = new Text[3];
    // Start is called before the first frame update
    void Start()
    {
        catapultTexts[0] = GameObject.Find("CatapultAngleText").GetComponent<Text>();
        catapultTexts[1] = GameObject.Find("CatapultPowerText").GetComponent<Text>();
        catapultTexts[2] = GameObject.Find("CatapultDistanceTraveledText").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        catapultTexts[0].text = "The angle is currently set to " + (GameObject.Find("CatapultFireButton").GetComponentInChildren<CatapultFire>().launchAngle*100) + " degrees";
        catapultTexts[1].text = "The power is curretly set to " + GameObject.Find("CatapultFireButton").GetComponentInChildren<CatapultFire>().speed + "m/s";
        catapultTexts[2].text = GameObject.Find("CatapultDistance").GetComponentInChildren<Text>().text;
    }
}
