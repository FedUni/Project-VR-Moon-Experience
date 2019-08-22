using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TimeRemain : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject player;
    void Start()
    {
        player = GameObject.Find("Player"); // Get the planet settings
    }

    void Update()
    {

        TimeSpan timeSpan = TimeSpan.FromSeconds(player.GetComponent<resetScene>().TIME_REMAINING);
        string timeText = string.Format("{0:D1}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);

        GameObject.Find("TimeNumber").GetComponent<Text>().text = timeText; // Set the text
    }

}
