using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour {

    //global variables
    public Rigidbody rb;
    private bool touchingGround = false;

    //functions
    public void Start()
    {
        rb.GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Terrain")
        {
            rb.isKinematic = true;
            touchingGround = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals(other.name == "Terrain"))
        {
            rb.isKinematic = false;
            touchingGround = false;
        }
    }
}
