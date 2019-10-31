using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR.Extras;
using UnityEngine.UI;

public class SteamVRLaserWrapper : MonoBehaviour
{
    private SteamVR_LaserPointer steamVrLaserPointer;
    private GameObject watchCanvas;
    private Canvas[] labels;
    private Vector3 scale;
    private Vector3 postition;
    private Vector3 originalScale;
    [Tooltip("The new size for the watch UI")]
    public Vector3 newScale;
    private Vector3 originalPostition;
    [Tooltip("The new postion for the watch UI")]
    public Vector3 newPostition;
    public bool isEnabled;
    public float speed;
    Canvas labelPointedAt;

    private void Awake()
    {
        steamVrLaserPointer = gameObject.GetComponent<SteamVR_LaserPointer>(); // Get the laser poiner object on the hand
        steamVrLaserPointer.PointerIn += OnPointerIn; // SteamVR stuff
        steamVrLaserPointer.PointerOut += OnPointerOut; // SteamVR stuff
        steamVrLaserPointer.PointerClick += OnPointerClick; // SteamVR stuff
        watchCanvas = GameObject.Find("WatchUI"); // Find the watch UI
        originalScale = watchCanvas.GetComponent<RectTransform>().localScale; // Get the original scale
        originalPostition = watchCanvas.GetComponent<RectTransform>().localPosition; // Get the original postion
        scale = watchCanvas.GetComponent<RectTransform>().localScale; // Get the original scale, Stops strange wobble
        postition = watchCanvas.GetComponent<RectTransform>().localPosition; // Get the original postion, Stops strange wobble
    }

    private void OnPointerClick(object sender, PointerEventArgs e) // Do things when the click with the pointer in a button
    {
        IPointerClickHandler clickHandler = e.target.GetComponent<IPointerClickHandler>();
        if (clickHandler == null)
        {           
            return;
        }        

        clickHandler.OnPointerClick(new PointerEventData(EventSystem.current));
    }

    private void Update()
    {
        watchCanvas.GetComponent<RectTransform>().localScale = Vector3.Lerp(watchCanvas.GetComponent<RectTransform>().localScale, scale, speed * Time.deltaTime); // Lerp the scale of the watch UI
        watchCanvas.GetComponent<RectTransform>().localPosition = Vector3.Lerp(watchCanvas.GetComponent<RectTransform>().localPosition, postition, speed * Time.deltaTime); // Lerp the location fo the watch UI
    }

    private void OnPointerOut(object sender, PointerEventArgs e) // When the pointer leaves the UI
    {
        IPointerExitHandler pointerExitHandler = e.target.GetComponent<IPointerExitHandler>();
        if (pointerExitHandler == null)
        {
            return;
        }
        if (isEnabled)
        {
            scale = originalScale; // Set the scale back to original
            postition = originalPostition; // Same for the postion
            pointerExitHandler.OnPointerExit(new PointerEventData(EventSystem.current));
        }

    }

    private void OnPointerIn(object sender, PointerEventArgs e)
    {
        IPointerEnterHandler pointerEnterHandler = e.target.GetComponent<IPointerEnterHandler>();
        if (pointerEnterHandler == null)
        {
            return;
        }
        if (isEnabled) {
            scale = newScale; // Set the scale to the new one
            postition = newPostition; // Same for the postion
            pointerEnterHandler.OnPointerEnter(new PointerEventData(EventSystem.current));
        }
    }
}