using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFlash : MonoBehaviour
{
    public void flashEnded()
    {
        GetComponent<Animator>().SetBool("flash", false);
    }
}
