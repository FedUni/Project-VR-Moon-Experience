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
    public Vector3 newScale;
    private Vector3 originalPostition;
    public Vector3 newPostition;
    public bool isEnabled;
    public float speed;
    Canvas labelPointedAt;

    private void Awake()
    {
        steamVrLaserPointer = gameObject.GetComponent<SteamVR_LaserPointer>();
        steamVrLaserPointer.PointerIn += OnPointerIn;
        steamVrLaserPointer.PointerOut += OnPointerOut;
        steamVrLaserPointer.PointerClick += OnPointerClick;
        watchCanvas = GameObject.Find("WatchUI");
        originalScale = watchCanvas.GetComponent<RectTransform>().localScale;
        originalPostition = watchCanvas.GetComponent<RectTransform>().localPosition;
        scale = watchCanvas.GetComponent<RectTransform>().localScale;
        postition = watchCanvas.GetComponent<RectTransform>().localPosition;
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
        watchCanvas.GetComponent<RectTransform>().localScale = Vector3.Lerp(watchCanvas.GetComponent<RectTransform>().localScale, scale, speed * Time.deltaTime);
        watchCanvas.GetComponent<RectTransform>().localPosition = Vector3.Lerp(watchCanvas.GetComponent<RectTransform>().localPosition, postition, speed * Time.deltaTime);
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
            scale = newScale;
            postition = newPostition;
            pointerEnterHandler.OnPointerEnter(new PointerEventData(EventSystem.current));
        }
    }
}