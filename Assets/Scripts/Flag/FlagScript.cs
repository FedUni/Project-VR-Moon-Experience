using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagScript : MonoBehaviour
{
    public Rigidbody rb;
    // Use this for initialization
    void Start ()
    {
        rb.GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        Debug.Log("Flag has been put into the ground");
        rb.velocity = new Vector3(0, 0, 0);//should stop all current movement once in the ground. 
    }
}
