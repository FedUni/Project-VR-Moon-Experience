using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour {

    //global variables
    public Rigidbody rb;

    //functions
    public void Start()
    {
        rb.GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Ground"))
        {
            Debug.Log("Flag has been put into the " + other.name);
            rb.velocity = new Vector3(0, 0, 0);//should stop all current movement once in the ground. 
            //rb.useGravity = false;//turn off gravity once its in the ground, shouldnt move. 
            rb.freezeRotation = true;//unfreeze rotation
        }
    }
    //once leaving the ground re-apply physics
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Flag has been removed from the " + other.name);
        rb.freezeRotation = false;
    }

}
