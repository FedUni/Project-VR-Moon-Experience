using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class OnClickAnimate : MonoBehaviour {

    Animator anim;

    void Awake()
    {
        anim = gameObject.GetComponent<Animator>();
        Debug.Log(anim.name);
    }
    
    void LateUpdate()
    {
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
            Debug.Log("fuck does it work?0");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
            Debug.Log("fuck does it work?0");

        Debug.Log("Colider enter");
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("button clicked");
            //anim.SetTrigger("Active");
            anim.Play(1);
        }
    }

}
