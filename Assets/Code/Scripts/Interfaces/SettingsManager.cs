using System.Collections.Generic;
using System.Linq;
using Michsky.UI.Shift;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public GameObject settingsPanel;

    [Header("Audio")]
    public AudioSource buttonClickSound;
    public AudioSource musicSound;

    [Header("Volume")]
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Graphics")]
    public TMP_Dropdown graphicsDropdown;
    private int qualityIndex;

    [Header("Resolution")]
    public TextMeshProUGUI txt_resolution;
    private Resolution[] resolutionsList;
    public TMP_Dropdown resolutionDropDown;
    private int currentResolutionIndex = 0;

    private List<Resolution> resolutions;
    //private HorizontalSelector resolutionScript;

    private const string MUSIC_PREF = "MusicVolume";
    private const string SFX_PREF = "SFXVolume";
    private const string QUALITY_PREF = "GraphicsQuality";
    private const string RES_PREF = "ResolutionIndex";


    public void Start()
    {
      

        SetupResolutionSelector();
        SetupVolume();
        SetupGraphicsDropdown();

    }



    #region Volume
    public void SetupVolume()
    {
        float musicVolume = PlayerPrefs.GetFloat(MUSIC_PREF, 0.3f);
        float sfxVolume = PlayerPrefs.GetFloat(SFX_PREF, 0.5f);

        musicSlider.value = musicVolume;
        sfxSlider.value = sfxVolume;

        musicSound.volume = musicVolume;
        buttonClickSound.volume = sfxVolume;
    }

    public void OnMusicVolumeChanged(float value)
    {
        musicSound.volume = value;
        PlayerPrefs.SetFloat(MUSIC_PREF, value);
    }

    public void OnSFXVolumeChanged(float value)
    {
        buttonClickSound.volume = value;
        PlayerPrefs.SetFloat(SFX_PREF, value);
    }
    #endregion

    #region Graphics
    private bool initializingGraphicsDropdown = false;

    void SetupGraphicsDropdown()
    {
        initializingGraphicsDropdown = true;

        graphicsDropdown.ClearOptions();

        string[] qualityLevels = QualitySettings.names;
        graphicsDropdown.AddOptions(new List<string>(qualityLevels));

        int savedIndex = PlayerPrefs.GetInt(QUALITY_PREF, -1);
        Debug.Log("Saved index: " + savedIndex);

        if (savedIndex < 0 || savedIndex >= qualityLevels.Length)
        {
            Debug.Log("Using current quality level instead.");
            qualityIndex = QualitySettings.GetQualityLevel();
            
        }
        else
        {
            qualityIndex = savedIndex;
        }

        graphicsDropdown.onValueChanged.RemoveAllListeners();
        graphicsDropdown.value = qualityIndex;
        graphicsDropdown.RefreshShownValue();

        graphicsDropdown.onValueChanged.AddListener(delegate { OnDropdownChanged(); });

        QualitySettings.SetQualityLevel(qualityIndex);

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
        PlayerPrefs.SetInt(QUALITY_PREF, qualityIndex);
        Debug.Log("Graphics set to: " + QualitySettings.names[qualityIndex]);
    }
    #endregion

    #region Resolution
    public void SetupResolutionSelector()
    {

        resolutionsList = Screen.resolutions;

        resolutionDropDown.ClearOptions();
        int savedIndex = PlayerPrefs.GetInt(RES_PREF, -1);

        List<string> options = new List<string>();

        for (int i = 0; i < resolutionsList.Length; i++) {

            string option = resolutionsList[i].width + " x " + resolutionsList[i].height;
            options.Add(option);

            // If no saved resolution, match current screen resolution
            if (savedIndex == -1 &&
                resolutionsList[i].width == Screen.currentResolution.width &&
                resolutionsList[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        // Use saved index if it's within valid range
        if (savedIndex >= 0 && savedIndex < resolutionsList.Length)
        {
            currentResolutionIndex = savedIndex;
        }

        resolutionDropDown.AddOptions(options);
        resolutionDropDown.value = currentResolutionIndex;
        resolutionDropDown.RefreshShownValue();



    }

    public void ApplyResolution(int index)
    {
        Resolution r = resolutionsList[index];
        Screen.SetResolution(r.width, r.height, Screen.fullScreen);

        // Save selected resolution index
        PlayerPrefs.SetInt(RES_PREF, index);
        PlayerPrefs.Save();
    }
    #endregion

    #region Reset
    public void ResetSettings()
    {
        PlayerPrefs.DeleteKey(MUSIC_PREF);
        PlayerPrefs.DeleteKey(SFX_PREF);
        PlayerPrefs.DeleteKey(QUALITY_PREF);
        PlayerPrefs.DeleteKey(RES_PREF);


        int defaultQuality = 2; // You can change this to 1 or 0 based on what you want
        QualitySettings.SetQualityLevel(defaultQuality);
        PlayerPrefs.SetInt(QUALITY_PREF, defaultQuality);

        Start();

    }
    #endregion
}
