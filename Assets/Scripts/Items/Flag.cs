using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour {

    //glabal variables
    public Rigidbody rb;

    //functions
    public void Start()
    {
        rb.GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        
        Debug.Log("Flag has been put into the ground");
        rb.velocity = new Vector3(0, 0, 0);//should stop all current movement once in the ground. 
    }

}
