using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public GameObject settingsPanel;

    // [Header("Audio")]
    // public AudioSource buttonClickSound;
    // public AudioSource musicSound;

    [Header("Fullscreen")]
    public Toggle fullscreenToggle;

    [Header("Volume")] public Slider musicSlider;

    public Slider sfxSlider;

    [Header("Graphics")] public TMP_Dropdown graphicsDropdown;

    [Header("Resolution")] public TextMeshProUGUI txt_resolution;

    public TMP_Dropdown resolutionDropDown;

    private int qualityIndex;
    // private int currentResolutionIndex = 0;

    private List<Resolution> resolutions;

    private Resolution[] resolutionsList;
    //private HorizontalSelector resolutionScript;

    // private const string MUSIC_PREF = "MusicVolume";
    // private const string SFX_PREF = "SFXVolume";
    // private const string QUALITY_PREF = "GraphicsQuality";
    // private const string RES_PREF = "ResolutionIndex";


    public void Start()
    {
        SetupResolutionSelector();
        SetupVolume();
        SetupGraphicsDropdown();
        SetupFullscreenToggle();
    }

    private void OnEnable()
    {
        Start();
    }


    public void SetupFullscreenToggle()
    {

        fullscreenToggle.isOn = GraphicsManager.Instance.FullScreen;
        
        fullscreenToggle.onValueChanged.AddListener((value) =>
        {
            Screen.fullScreen = value;
        });
    }

    #region Volume

    public void SetupVolume()
    {
        musicSlider.onValueChanged.AddListener(value => SoundManager.Instance.SetMusicVolume(value));
        sfxSlider.onValueChanged.AddListener(value => SoundManager.Instance.SetSoundVolume(value));

        var musicVolume = SoundManager.Instance.MusicVolume;
        var sfxVolume = SoundManager.Instance.SoundVolume;

        musicSlider.value = musicVolume;
        sfxSlider.value = sfxVolume;
    }

    #endregion

    #region Reset

    public void ResetSettings()
    {
        SaveManager.Instance.ResetSettings();
        Start();
    }

    #endregion

    #region Graphics

    private bool initializingGraphicsDropdown;

    private void SetupGraphicsDropdown()
    {
        initializingGraphicsDropdown = true;

        graphicsDropdown.ClearOptions();

        var qualityLevels = QualitySettings.names;
        graphicsDropdown.AddOptions(new List<string>(qualityLevels));

        // int savedIndex = GraphicsManager.Instance.QualityIndex;
        // Debug.Log("Saved index: " + savedIndex);

        // if (savedIndex < 0 || savedIndex >= qualityLevels.Length)
        // {
        //     Debug.Log("Using current quality level instead.");
        //     qualityIndex = QualitySettings.GetQualityLevel();
        // }
        // else
        // {
        //     qualityIndex = savedIndex;
        // }

        graphicsDropdown.onValueChanged.RemoveAllListeners();
        graphicsDropdown.value = GraphicsManager.Instance.QualityIndex;
        graphicsDropdown.RefreshShownValue();

        graphicsDropdown.onValueChanged.AddListener(delegate { OnDropdownChanged(); });

        QualitySettings.SetQualityLevel(GraphicsManager.Instance.QualityIndex);

        initializingGraphicsDropdown = false;
    }

    public void OnDropdownChanged()
    {
        if (initializingGraphicsDropdown)
            return;

        qualityIndex = graphicsDropdown.value;
        SetQuality();
    }

    public void SetQuality()
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        Debug.Log("Graphics set to: " + QualitySettings.names[qualityIndex]);
    }

    #endregion

    #region Resolution

    public void SetupResolutionSelector()
    {
        resolutionsList = Screen.resolutions;

        resolutionDropDown.ClearOptions();
        var savedIndex = GraphicsManager.Instance.ResolutionIndex;

        // print($"Resolution Index: {savedIndex}");

        var options = new List<string>();

        for (var i = 0; i < resolutionsList.Length; i++)
        {
            var option = resolutionsList[i].width + " x " + resolutionsList[i].height;
            options.Add(option);

            // If no saved resolution, match current screen resolution
            // if (savedIndex == -1 &&
            //     resolutionsList[i].width == Screen.currentResolution.width &&
            //     resolutionsList[i].height == Screen.currentResolution.height)
            // {
            //     currentResolutionIndex = i;
            // }
        }

        // Use saved index if it's within valid range
        // if (savedIndex >= 0 && savedIndex < resolutionsList.Length)
        // {
        //     currentResolutionIndex = savedIndex;
        // }

        resolutionDropDown.AddOptions(options);
        resolutionDropDown.value = GraphicsManager.Instance.ResolutionIndex;
        resolutionDropDown.RefreshShownValue();
    }

    public void ApplyResolution(int index)
    {
        var r = resolutionsList[index];
        Screen.SetResolution(r.width, r.height, Screen.fullScreen);
    }

    #endregion
}