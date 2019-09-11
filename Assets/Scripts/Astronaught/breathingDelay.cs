using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class breathingDelay : MonoBehaviour {

    AudioSource myAudio;

	// Use this for initialization
	void Start () {
        myAudio = GetComponent<AudioSource>();
        myAudio.PlayDelayed(10.5f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
