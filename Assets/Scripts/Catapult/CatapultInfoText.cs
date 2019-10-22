using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CatapultInfoText : MonoBehaviour {
    private Text[] catapultTexts = new Text[3];
    private CatapultFire CFscript;
    private Text CDText;
    // Start is called before the first frame update
    void Start()
    {
        catapultTexts[0] = GameObject.Find("CatapultAngleText").GetComponent<Text>();
        catapultTexts[1] = GameObject.Find("CatapultPowerText").GetComponent<Text>();
        catapultTexts[2] = GameObject.Find("CatapultDistanceTraveledText").GetComponent<Text>();
        CFscript = GameObject.Find("CatapultFireButton").GetComponentInChildren<CatapultFire>();
        CDText = GameObject.Find("CatapultDistance").GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (CFscript.launchAngle == 1f) // If this is changed the script for the angle button will need changing (Launch90)
        {
            catapultTexts[0].text = "The angle is currently set to 70°";
        }
        if (CFscript.launchAngle == 0.5f) // If this is changed the script for the angle button will need changing (Launch45)
        {
            catapultTexts[0].text = "The angle is currently set to 35°";
        }
        if (CFscript.launchAngle == 0.2f) // If this is changed the script for the angle button will need changing (Launch20)
        {
            catapultTexts[0].text = "The angle is currently set to 15°";
        }
        /////////////////////////////////////////////////////////////////////////
        if (CFscript.speed == 5f) // If this is changed the script for the speed button will need changing (SpeedSetting1)
        {
            catapultTexts[1].text = "The power is curretly set to low";
        }
        if (CFscript.speed == 10f) // If this is changed the script for the speed button will need changing (SpeedSetting2)
        {
            catapultTexts[1].text = "The power is curretly set to medium";
        }
        if (CFscript.speed == 30f) // If this is changed the script for the speed button will need changing (SpeedSetting3)
        {
            catapultTexts[1].text = "The power is curretly set to high";
        }
        if (CFscript.speed == 40f) // If this is changed the script for the speed button will need changing (SpeedSetting4)
        {
            catapultTexts[1].text = "The power is curretly set to very high";
        }

        //catapultTexts[0].text = "The angle is currently set to " + (CFscript.launchAngle*100) + " degrees";
        //catapultTexts[1].text = "The power is curretly set to " + CFscript.speed + "m/s";
        catapultTexts[2].text = CDText.text;
    }
}
