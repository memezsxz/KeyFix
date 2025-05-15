using System;
using UnityEngine;

/// <summary>
///     Manages graphics and display settings, including resolution and quality.
///     Implements IDataPersistence for saving/loading persistent settings data.
/// </summary>
public class GraphicsManager : Singleton<GraphicsManager>, IDataPersistence
{
    /// <summary>
    /// Index of the resolution currently applied.
    /// </summary>
    public int ResolutionIndex { get;  set; }

    /// <summary>
    /// Index of the current quality level (e.g., Low, Medium, High).
    /// </summary>
    public int QualityIndex { get; private set; }

    /// <summary>
    /// Whether the game is currently in fullscreen mode.
    /// </summary>
    public bool FullScreen { get; private set; }

    public void SaveData(ref SaveData data)
    {
        // Save current quality setting by name
        data.Graphics.QualityName = QualitySettings.names[QualitySettings.GetQualityLevel()];

        // Save current screen resolution
        var currentResolution = Screen.resolutions[ResolutionIndex];
        data.Graphics.ResolutionWidth = currentResolution.width;
        data.Graphics.ResolutionHeight = currentResolution.height;
        
        // Save fullscreen setting
        data.Graphics.Fullscreen = Screen.fullScreen;

        // Store the last used save slot
        PlayerPrefs.SetString(SaveManager.LAST_GAME_PREF, data.Meta.SaveName);
        PlayerPrefs.Save();
    }

    public void LoadData(ref SaveData data)
    {
        // === Load Quality ===
        var qualityName = data.Graphics.QualityName;

        // Get the index of the saved quality level
        var qualityIndex = Array.IndexOf(QualitySettings.names, qualityName);

        // If not found, fallback to last available level
        qualityIndex = qualityIndex < 0 ? QualitySettings.names.Length - 1 : qualityIndex;

        // Apply quality settings
        QualitySettings.SetQualityLevel(qualityIndex);
        QualityIndex = qualityIndex;

        // === Load Resolution ===
        var saveData = data;

        // Find the resolution index that matches the saved width and height
        var index = Array.FindIndex(Screen.resolutions, r =>
            r.width == saveData.Graphics.ResolutionWidth &&
            r.height == saveData.Graphics.ResolutionHeight);

        // If not found, fallback to highest available resolution
        index = index < 0 ? Screen.resolutions.Length - 1 : index;

        // Debug.Log("ires index is : " + (index < 0 ? " not found" : "found"));

        // Apply resolution and fullscreen setting
        var res = Screen.resolutions[index];
        Screen.SetResolution(res.width, res.height, data.Graphics.Fullscreen);
        ResolutionIndex = index;

        // Apply fullscreen toggle
        Screen.fullScreen = data.Graphics.Fullscreen;
        FullScreen = data.Graphics.Fullscreen;
    }
}