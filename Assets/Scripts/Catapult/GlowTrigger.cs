using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowTrigger : MonoBehaviour
{
    // Start is called before the first frame update

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInChildren<LaunchGlow>() != null)
        {
            other.GetComponentInChildren<LaunchGlow>().isLerping = true;
            other.GetComponentInChildren<LaunchGlow>().postion = new Vector3(1.563f, 4.2f, 1.628f);
        }
    }
}
