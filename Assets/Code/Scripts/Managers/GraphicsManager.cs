using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages graphics and display settings, including resolution and quality.
/// Implements IDataPersistence for saving/loading persistent settings data.
/// </summary>
public class GraphicsManager : Singleton<GraphicsManager>, IDataPersistence
{
    public void SaveData(ref SaveData data)
    {
        data.Graphics.QualityName = QualitySettings.GetQualityLevel().ToString();
        data.Graphics.ResolutionHeight = Screen.currentResolution.height;
        data.Graphics.ResolutionWidth = Screen.currentResolution.width;
        data.Graphics.Fullscreen = Screen.fullScreen;

        PlayerPrefs.SetString(SaveManager.LAST_GAME_PREF, data.Meta.SaveName);
        PlayerPrefs.Save();
        
        // print("Saved: " + PlayerPrefs.GetString("LastGame"));
        // Debug.Log("saved quality: " + data.Graphics.QualityName);
        // Debug.Log("saved res: " + Screen.currentResolution.width + " x " + Screen.currentResolution.height);
    }

    public void LoadData(ref SaveData data)
    {
        // quality
        string qualityName = SaveManager.Instance.SaveData.Graphics.QualityName;
        int qualityIndex = System.Array.IndexOf(QualitySettings.names, qualityName);

        if (qualityIndex < 0)
            qualityIndex = QualitySettings.names.Length - 1;

        QualitySettings.SetQualityLevel(qualityIndex);

        // resolution
        Resolution targetResolution = new Resolution()
        {
            height = data.Graphics.ResolutionHeight,
            width = data.Graphics.ResolutionWidth
        };

        int resolutionIndex = Array.FindIndex(Screen.resolutions, r =>
            r.width == targetResolution.width && r.height == targetResolution.height);

        if (resolutionIndex < 0) resolutionIndex = Screen.resolutions.Length - 1;

        Screen.SetResolution(
            Screen.resolutions[resolutionIndex].width,
            Screen.resolutions[resolutionIndex].height,
            data.Graphics.Fullscreen
        );

        // Debug.Log("loaded quality: " + data.Graphics.QualityName);
        // Debug.Log("loaded res: " + Screen.resolutions[resolutionIndex].width + " x " +
        //           Screen.resolutions[resolutionIndex].height);
    }
}
