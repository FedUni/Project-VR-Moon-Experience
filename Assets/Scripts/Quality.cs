using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Quality : MonoBehaviour
{

    public Canvas GraphicsMenu;
    public Button Button_Low;
    public Button Button_Med;
    public Button Button_High;

    void Start()
    {
        GraphicsMenu = GraphicsMenu.GetComponent<Canvas>();
        Button_Low = Button_Low.GetComponent<Button>();
        Button_Med = Button_Med.GetComponent<Button>();
        Button_High = Button_High.GetComponent<Button>();
        GraphicsMenu.enabled = false;

    }
    public void low()
    {
        QualitySettings.SetQualityLevel(0);
    }
    public void med()
    {
        QualitySettings.SetQualityLevel(1);
    }
    public void high()
    {
        QualitySettings.SetQualityLevel(3);
    }

}