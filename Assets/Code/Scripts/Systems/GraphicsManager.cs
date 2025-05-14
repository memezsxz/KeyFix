using System;
using UnityEngine;

/// <summary>
///     Manages graphics and display settings, including resolution and quality.
///     Implements IDataPersistence for saving/loading persistent settings data.
/// </summary>
public class GraphicsManager : Singleton<GraphicsManager>, IDataPersistence
{
    public int ResolutionIndex { get; private set; }
    public int QualityIndex { get; private set; }

    public bool FullScreen { get; private set; }
    public void SaveData(ref SaveData data)
    {
        data.Graphics.QualityName = QualitySettings.names[QualitySettings.GetQualityLevel()];
        data.Graphics.ResolutionWidth = Screen.currentResolution.width;
        data.Graphics.ResolutionHeight = Screen.currentResolution.height;
        data.Graphics.Fullscreen = Screen.fullScreen;

        PlayerPrefs.SetString(SaveManager.LAST_GAME_PREF, data.Meta.SaveName);
        PlayerPrefs.Save();

        // Debug.Log("saved quality: " + data.Graphics.QualityName);
        // Debug.Log("saved res: " + Screen.currentResolution.width + " x " + Screen.currentResolution.height);
    }

    public void LoadData(ref SaveData data)
    {
        // Quality
        var qualityName = data.Graphics.QualityName;
        var qualityIndex = Array.IndexOf(QualitySettings.names, qualityName);
        qualityIndex = qualityIndex < 0 ? QualitySettings.names.Length - 1 : qualityIndex;
        QualitySettings.SetQualityLevel(qualityIndex);
        QualityIndex = qualityIndex;

        // Resolution
        var saveData = data;
        var index = Array.FindIndex(Screen.resolutions, r =>
            r.width == saveData.Graphics.ResolutionWidth &&
            r.height == saveData.Graphics.ResolutionHeight);

        index = index < 0 ? Screen.resolutions.Length - 1 : index;
        // Debug.Log("ires index is : " + (index < 0 ? " not found" : "found"));

        var res = Screen.resolutions[index];
        Screen.SetResolution(res.width, res.height, data.Graphics.Fullscreen);
        ResolutionIndex = index;

        Screen.fullScreen = data.Graphics.Fullscreen;
        FullScreen = data.Graphics.Fullscreen;
        // Debug.Log("loaded quality: " + data.Graphics.QualityName);
        // Debug.Log("loaded res: " + Screen.resolutions[ResolutionIndex].width + " x " +
        //           Screen.resolutions[ResolutionIndex].height);
    }
}