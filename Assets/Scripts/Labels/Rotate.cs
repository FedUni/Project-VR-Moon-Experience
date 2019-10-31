using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Created by Hein for the Moon VR 3.0 Project
public class Rotate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * 100);
    }
}
