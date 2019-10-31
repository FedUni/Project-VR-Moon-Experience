using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using System;

public class radio : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip[] audioClips; // Array of clips to play
    int clip = 0;
    public bool notRandom;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    //Called every Update() while a Hand is hovering over this object
    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();
        if (startingGrabType != GrabTypes.None)
        {

            if (notRandom) // If not set to random just go threw one by one
            {
                clip++; // advance to next
                if (clip >= audioClips.Length)
                {
                    clip = 0; // make sure we dont go over the end of the array
                }
            }
            else
            {
                int randClip = UnityEngine.Random.Range(0, audioClips.Length); // get a random number
                while (randClip == clip) { // make sure its not the clip we just played kind of annoying
                    randClip = UnityEngine.Random.Range(0, audioClips.Length); // Get a random clip between 0 and the audio clips lenth +1 plus one becase rand in exclusive
                }
                clip = randClip; // Assign the random number to the clip
                
            }

            if (audioClips[clip] != null) {
                audioSource.clip = audioClips[clip];
                GetComponent<AudioSource>().Play(); // Play the sound
            }
        }

    }
}
