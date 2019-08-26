using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.Extras;
using UnityEngine.EventSystems;

public class WatchScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool isEnabled;
    public RectTransform canvas;
    Vector3 scale;
    Vector3 postition;
  

    // Start is called before the first frame update
    void Start()
    {
        //canvas = GetComponent<RectTransform>();
        //scaledCanvas = new RectTransform;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.localScale = Vector3.Lerp(canvas.localScale, scale, 1.0f * Time.deltaTime);
        //transform.localPosition = Vector3.Lerp(canvas.localPosition, postition, 1.0f * Time.deltaTime);
    }

    public void onEnter ()
    {
        if (isEnabled)
        {
            //scale = new Vector3(0.0003974478f, 0.0003974483f, 0.0002515115f);
            //postition = new Vector3(89.43001f, 222.684f, 132.24f);
            Debug.Log("On enter");
            canvas.localScale = new Vector3(0.0003974478f, 0.0003974483f, 0.0002515115f);
            canvas.localPosition = new Vector3(89.43001f, 222.684f, 132.24f);

        }
    }

    public void onExit()
    {
        if (isEnabled)
        {
            //scale = new Vector3(0.0001217484f, 0.0001217486f, 7.704442e-05f);
            //postition = new Vector3(-0.0025f, 0.0277f, -0.0003f);
            Debug.Log("On exit");
            canvas.localScale = new Vector3(0.0001217484f, 0.0001217486f, 7.704442e-05f);
            canvas.localPosition = new Vector3(-0.0025f, 0.0277f, -0.0003f);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("On enter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("On exit");
    }
}
