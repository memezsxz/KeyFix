using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles player settings UI for volume, resolution, fullscreen, and graphics quality.
/// Connects UI components to system settings and updates values accordingly.
/// </summary>
public class SettingsManager : MonoBehaviour
{
    #region Serialized Fields

    /// <summary>
    /// Root panel for the settings UI.
    /// </summary>
    public GameObject settingsPanel;


    /// <summary>
    /// Toggle to switch fullscreen mode.
    /// </summary>
    [Header("Fullscreen")] public Toggle fullscreenToggle;


    /// <summary>
    /// Slider controlling background music volume.
    /// </summary>
    [Header("Volume")] public Slider musicSlider;

    /// <summary>
    /// Slider controlling sound effects volume.
    /// </summary>
    public Slider sfxSlider;


    /// <summary>
    /// Dropdown to select graphics quality level.
    /// </summary>
    [Header("Graphics")] public TMP_Dropdown graphicsDropdown;


    /// <summary>
    /// Text label displaying current resolution.
    /// </summary>
    [Header("Resolution")] public TextMeshProUGUI txt_resolution;

    /// <summary>
    /// Dropdown used to select screen resolution.
    /// </summary>
    public TMP_Dropdown resolutionDropDown;

    #endregion

    #region Private Fields

    /// <summary>
    /// Stores the selected quality level index.
    /// </summary>
    private int qualityIndex;

    /// <summary>
    /// List of supported screen resolutions available on the device.
    /// </summary>
    private Resolution[] resolutionsList;

    /// <summary>
    /// Whether the graphics dropdown is currently being initialized to avoid triggering unwanted events.
    /// </summary>
    private bool initializingGraphicsDropdown;

    #endregion

    #region Unity Lifecycle

    public void Start()
    {
        SetupResolutionSelector();
        SetupVolume();
        SetupGraphicsDropdown();
        SetupFullscreenToggle();
    }

    private void OnEnable()
    {
        Start(); // Ensures the UI is refreshed when settings panel is shown
    }

    #endregion

    #region Fullscreen

    /// <summary>
    /// Initializes the fullscreen toggle state and listener.
    /// </summary>
    public void SetupFullscreenToggle()
    {
        fullscreenToggle.isOn = GraphicsManager.Instance.FullScreen;

        fullscreenToggle.onValueChanged.AddListener((value) => { Screen.fullScreen = value; });
    }

    #endregion

    #region Volume

    /// <summary>
    /// Initializes the volume sliders and their event listeners.
    /// </summary>
    public void SetupVolume()
    {
        musicSlider.onValueChanged.AddListener(value => SoundManager.Instance.SetMusicVolume(value));
        sfxSlider.onValueChanged.AddListener(value => SoundManager.Instance.SetSoundVolume(value));

        musicSlider.value = SoundManager.Instance.MusicVolume;
        sfxSlider.value = SoundManager.Instance.SoundVolume;
    }

    #endregion

    #region Reset

    /// <summary>
    /// Resets all settings to default values.
    /// </summary>
    public void ResetSettings()
    {
        SaveManager.Instance.ResetSettings();
        Start(); // Reinitialize UI after reset
    }

    #endregion

    #region Graphics

    /// <summary>
    /// Initializes the graphics quality dropdown and listeners.
    /// </summary>
    private void SetupGraphicsDropdown()
    {
        initializingGraphicsDropdown = true;

        graphicsDropdown.ClearOptions();

        // Populate dropdown with all quality level names from Unity
        var qualityLevels = QualitySettings.names;
        graphicsDropdown.AddOptions(new List<string>(qualityLevels));

        // Remove previous listeners to avoid duplicate calls
        graphicsDropdown.onValueChanged.RemoveAllListeners();

        // Set current value from saved quality index
        graphicsDropdown.value = GraphicsManager.Instance.QualityIndex;
        graphicsDropdown.RefreshShownValue();

        // Add event listener after setting the value
        graphicsDropdown.onValueChanged.AddListener(delegate { OnDropdownChanged(); });

        // Apply the current quality setting
        QualitySettings.SetQualityLevel(GraphicsManager.Instance.QualityIndex);

        initializingGraphicsDropdown = false;
    }

    /// <summary>
    /// Callback for when the graphics dropdown value changes.
    /// </summary>
    public void OnDropdownChanged()
    {
        // Skip if this change was triggered during initialization
        if (initializingGraphicsDropdown)
            return;

        // Cache new selected index and apply graphics level
        qualityIndex = graphicsDropdown.value;
        SetQuality();
    }

    /// <summary>
    /// Applies the selected quality level to Unity's graphics settings.
    /// </summary>
    public void SetQuality()
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        // Debug.Log("Graphics set to: " + QualitySettings.names[qualityIndex]);
    }

    #endregion

    #region Resolution

    /// <summary>
    /// Initializes the resolution dropdown with available screen modes.
    /// </summary>
    public void SetupResolutionSelector()
    {
        resolutionsList = Screen.resolutions;

        resolutionDropDown.ClearOptions();
        var savedIndex = GraphicsManager.Instance.ResolutionIndex;

        var options = new List<string>();

        // Build list of resolution strings
        for (int i = 0; i < resolutionsList.Length; i++)
        {
            var option = resolutionsList[i].width + " x " + resolutionsList[i].height;
            options.Add(option);
        }

        // Populate dropdown and apply saved value
        resolutionDropDown.AddOptions(options);
        resolutionDropDown.value = savedIndex;
        resolutionDropDown.RefreshShownValue();
    }

    /// <summary>
    /// Applies the selected resolution index to the screen.
    /// </summary>
    /// <param name="index">Index of the resolution in the dropdown list.</param>
    public void ApplyResolution(int index)
    {
        // Apply selected resolution at fullscreen
        var r = resolutionsList[index];
        Screen.SetResolution(r.width, r.height, Screen.fullScreen);
    }

    #endregion
}