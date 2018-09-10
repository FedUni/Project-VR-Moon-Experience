using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    #region Unity Singleton pattern

    // Instance of the game settings class
    private static GameSettings _instance;

    /// <summary>
    /// Instance of the game settings class. There should only be one of these!
    /// </summary>
    public static GameSettings Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameSettings>();
                if (_instance == null)
                {
                    // Create a game object that is a holder for our game settings script
                    GameObject go = new GameObject("_Singleton_GameSettings");

                    // Add the game settings script to the holder object
                    _instance = go.AddComponent<GameSettings>();

                    // Tell Unity not to unload this object; there's one per game!
                    DontDestroyOnLoad(go);
                }
            }

            return _instance;
        }
    }

    #endregion

    #region Static names

    // The name of the settings file
    static readonly string SETTINGS_FILE = "settings.json";

    #endregion

    #region Public properties

    /// <summary>
    /// Indicates that the game settings have been loaded and the object is ready.
    /// </summary>
    public bool IsReady { get; private set; }

    /// <summary>
    /// Unity quality settings.
    /// </summary>
    public List<string> QualityNames { get; private set; }

    /// <summary>
    /// Unity resolutions.
    /// </summary>
    public List<Resolution> Resolutions { get; private set; }

    /// <summary>
    /// The user's current game options;
    /// </summary>
    public UserGameOptions UserOptions { get; private set; }

    #endregion

    #region Public methods

    /// <summary>
    /// Save the player's options.
    /// </summary>
    /// <param name="quality"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public void SaveSettings(int quality, int width, int height, bool fullscreen)
    {
        var settings = new UserGameOptions()
        {
            quality = quality,
            height = height,
            width = width
        };

        string fullPath = Path.Combine(Application.persistentDataPath, SETTINGS_FILE);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        // Write the contents of the settings to the disk
        File.WriteAllText(fullPath, JsonUtility.ToJson(settings));

        // Apply the settings for this session
        ApplySettings(settings);

        // Remember the settings for the next time this is called
        UserOptions = settings;
    }

    #endregion

    #region Unity messages

    /// <summary>
    /// Awaken the object!
    /// </summary>
    void Awake()
    {
        QualityNames = new List<string>(QualitySettings.names);
        Resolutions = new List<Resolution>(Screen.resolutions);

        UserOptions = LoadOptions();

        IsReady = true;
    }

    #endregion

    #region Private helper methods

    /// <summary>
    /// Load the user's current options (if any).
    /// </summary>
    /// <returns></returns>
    UserGameOptions LoadOptions()
    {
        string fullPath = Path.Combine(Application.persistentDataPath, SETTINGS_FILE);

        if (File.Exists(fullPath))
        {
            // Read in the contents of the settings file
            string json = File.ReadAllText(fullPath);
            var settings = JsonUtility.FromJson<UserGameOptions>(json);

            // Apply the settings
            ApplySettings(settings);
            return settings;
        }
        else
        {
            // Make up a default one
            return new UserGameOptions()
            {
                quality = QualitySettings.GetQualityLevel(),
                height = Screen.height,
                width = Screen.width
            };
        }
    }

    /// <summary>
    /// Apply quality and resolution settings for the game.
    /// </summary>
    /// <param name="settings">User options</param>
    void ApplySettings(UserGameOptions settings)
    {
        QualitySettings.SetQualityLevel(settings.quality);
    }

    #endregion
}