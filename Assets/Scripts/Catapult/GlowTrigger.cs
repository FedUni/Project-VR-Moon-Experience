using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowTrigger : MonoBehaviour
{
    // Start is called before the first frame update

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInChildren<LaunchGlow>() != null) // Only do this if the object has the LanchGlow script attached
        {
            other.GetComponentInChildren<LaunchGlow>().isLerping = true; // Make it lerp to chaneg the canvas size
            other.GetComponentInChildren<LaunchGlow>().postion = new Vector3(1.563f, 4.2f, 1.628f); // Pop up the distacne canvas
        }
    }
}
