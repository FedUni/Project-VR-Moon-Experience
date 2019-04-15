using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropRigSoundStopper : MonoBehaviour
{
    void minHeight() // This turns the sound off for the drop rig when it hits the bottom
    {
        AnimatorStateInfo animationState = GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
        if (GetComponent<Animator>().GetFloat("Direction") < 0) {
            GetComponent<AudioSource>().Stop();
        }
    }


    void maxHeight() // This turns the sound off for the drop rig when it hits the top
    {
        GetComponent<AudioSource>().Stop();
    }

}
