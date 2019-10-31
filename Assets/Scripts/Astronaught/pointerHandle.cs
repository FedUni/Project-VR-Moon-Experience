using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR.InteractionSystem;
using UnityEngine.UI;

public class pointerHandle : MonoBehaviour
{
    Animator anim;
    AudioSource sound;
    AnimatorStateInfo animationState;
    GameObject planetSettings;
    GameObject DropRig;
    Text[] text;
    public double dropHeight;
    bool pointerDown = false;
    Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        DropRig = GameObject.Find("DropRig"); // Get the drop rig
        anim = DropRig.GetComponent<Animator>(); // Get animation controller from the object
        sound = DropRig.GetComponent<AudioSource>(); // Get the sound source from the correct place in the object
        AnimatorStateInfo animationState = anim.GetCurrentAnimatorStateInfo(0); // Used Get the current animation playtime
        planetSettings = GameObject.Find("PlanetSettings"); // Get the planet settings
        sound.loop = true;
        text = DropRig.GetComponentsInChildren<Text>(); // Get all the text elements in the drop rig
        slider = gameObject.GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (anim.GetBool("heightHasPlayed"))
        {
            text[2].text = "The current drop is " + System.Math.Round(anim.GetFloat("wingHeight"), 0) + " Metres"; // Set the drop rig LCD text
            text[2].color = Color.green;
        }
    }

    public void upPressing()
    {
        anim.SetBool("heightHasPlayed", true);
        anim.StopPlayback(); // Stop any current playback
        anim.SetFloat("Direction", 10); // Set the direction and in this case the speed
        anim.Play("DropRigHeight"); // Play the animation
        text[3].text = ""; // Clear the instructions
        if (planetSettings.GetComponent<PlanetSettings>().hasAtmos)
        {
            sound.Play(); // Play the sound effect
        }
        AnimatorStateInfo animationState = anim.GetCurrentAnimatorStateInfo(0); // Used Get the current animation playtime
                                                                                // This next section is to fix a delay between playing the animation in reverse becase the animation counter keep counting even when the animation is finished
        if (animationState.normalizedTime < 0)
        { // If is less than zero is rewound too far

            anim.Play("DropRigHeight", -1, 0); // Play it back from the start postion
        }
    }
    public void upReleasing()
    {
        anim.SetFloat("Direction", 0); // effectilty stops the animaiton for the hight ajustment
        sound.Stop();
        dropHeight = System.Math.Truncate(animationState.normalizedTime * 100); // calaulate the hight of the drop rig based on the animation playthrough time
        text[2].text = "The current drop is " + System.Math.Round(anim.GetFloat("wingHeight"), 0) + " Meters"; // Set the drop rig LCD text
    }

    public void setPostion() {
        
        anim.SetBool("heightHasPlayed", true);
        anim.Play("DropRigHeight", 0, slider.value); // Play the animation
        text[3].text = ""; // Clear the instructions
    }

}
