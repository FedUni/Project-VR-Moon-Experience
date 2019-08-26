using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR.Extras;
using UnityEngine.UI;

public class SteamVRLaserWrapper : MonoBehaviour
{
    private SteamVR_LaserPointer steamVrLaserPointer;
    private GameObject canvas;
    private Vector3 scale;
    private Vector3 postition;
    private Vector3 originalScale;
    public Vector3 newScale;
    private Vector3 originalPostition;
    public Vector3 newPostition;
    public bool isEnabled;
    public float speed;

    private void Awake()
    {
        steamVrLaserPointer = gameObject.GetComponent<SteamVR_LaserPointer>();
        steamVrLaserPointer.PointerIn += OnPointerIn;
        steamVrLaserPointer.PointerOut += OnPointerOut;
        steamVrLaserPointer.PointerClick += OnPointerClick;
        canvas = GameObject.Find("WatchUI");
        originalScale = canvas.GetComponent<RectTransform>().localScale;
        originalPostition = canvas.GetComponent<RectTransform>().localPosition;
        scale = canvas.GetComponent<RectTransform>().localScale;
        postition = canvas.GetComponent<RectTransform>().localPosition;
    }

    private void OnPointerClick(object sender, PointerEventArgs e)
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
        canvas.GetComponent<RectTransform>().localScale = Vector3.Lerp(canvas.GetComponent<RectTransform>().localScale, scale, speed * Time.deltaTime);
        canvas.GetComponent<RectTransform>().localPosition = Vector3.Lerp(canvas.GetComponent<RectTransform>().localPosition, postition, speed * Time.deltaTime);
    }

    private void OnPointerOut(object sender, PointerEventArgs e)
    {
        IPointerExitHandler pointerExitHandler = e.target.GetComponent<IPointerExitHandler>();
        if (pointerExitHandler == null)
        {
            return;
        }
        if (isEnabled)
        {
            //canvas.GetComponent<RectTransform>().localScale = originalScale;
            //canvas.GetComponent<RectTransform>().localPosition = originalPostition;
            scale = originalScale;
            postition = originalPostition;
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
            //canvas.GetComponent<RectTransform>().localScale = newScale;
            //canvas.GetComponent<RectTransform>().localPosition = newPostition;
            scale = newScale;
            postition = newPostition;
            pointerEnterHandler.OnPointerEnter(new PointerEventData(EventSystem.current));
        }
    }
}