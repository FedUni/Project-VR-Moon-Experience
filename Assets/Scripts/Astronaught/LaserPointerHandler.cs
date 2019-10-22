using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.Extras;

public class LaserPointerHandler : MonoBehaviour
{
    public bool selected;
    public SteamVR_LaserPointer laserPointer;
    // Start is called before the first frame update
    void Start()
    {
        laserPointer.PointerIn += PointerInside;
        laserPointer.PointerOut += PointerOutside;
        selected = false;
    }
    public void PointerInside(object sender, PointerEventArgs e)
    {
        if (e.target.name == this.gameObject.name && selected == false)
        {
            selected = true;
        }
    }
    public void PointerOutside(object sender, PointerEventArgs e)
    {
        if (e.target.name == this.gameObject.name && selected == true)
        {
            selected = false;
        }
    }
    public bool get_selected_value()
    {
        return selected;
    }
}
