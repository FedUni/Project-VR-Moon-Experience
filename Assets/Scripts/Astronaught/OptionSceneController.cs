using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionSceneController : MonoBehaviour
{
    // List of filtered resolutions
    List<Resolution> filteredRes;

    bool fullScreen;

    public Dropdown quality;

    public Dropdown resolution;

    /// <summary>
    /// Apply button clicked.
    /// </summary>
    public void Apply_Clicked()
    {
        Resolution res = filteredRes[resolution.value];
        int qual = quality.value;

        GameSettings.Instance.SaveSettings(qual, res.width, res.height, fullScreen);
        SceneManager.LoadScene("MainMenu");
    }

    public void Cancel_Clicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // Use this for initialization
    IEnumerator Start()
    {
        // Wait for the game settings to become available
        while (!GameSettings.Instance.IsReady)
        {
            yield return null;
        }

        var user = GameSettings.Instance.UserOptions;

        // Apply the contents of Quality 
        quality.ClearOptions();
        quality.AddOptions(GameSettings.Instance.QualityNames);
        quality.value = user.quality;

        // ... and Screen to the drop downs

        List<string> resos = new List<string>();
        filteredRes = new List<Resolution>();

        int lw = -1;
        int lh = -1;

        int index = 0;
        int currentResIndex = -1;

        foreach (var res in GameSettings.Instance.Resolutions)
        {
            if (lw != res.width || lh != res.height)
            {
                // Create a neatly formatted string to add to the dropdown
                string fmt = string.Format("{0} x {1}", res.width, res.height);
                resos.Add(fmt);

                lw = res.width;
                lh = res.height;

                // Figure out if this is the user's current resolution
                if (lw == user.width && lh == user.height)
                {
                    currentResIndex = index;
                }

                // Add the filtered resolution to the list
                filteredRes.Add(res);

                index++;
            }
        }

        resolution.ClearOptions();
        resolution.AddOptions(resos);
        resolution.value = currentResIndex;


    }

}
