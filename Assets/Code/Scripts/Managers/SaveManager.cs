using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

public class SaveManager : PersistentSingleton<SaveManager>
{
    [Header("Save Slot")] public string SaveSlotName = "Save1";

    private List<IDataPersistence> _dataHandlers;


    private readonly BinaryFormatter _formatter = new();
    private DateTime _sessionStartTime;
    public SaveData SaveData { get; private set; }

    public string Format => "save";

    private void Start()
    {
        _sessionStartTime = DateTime.Now;
        FindAllDataHandlers();
        // LoadGame(SaveSlotName);
    }

    private string GetSavePath(string slotName)
    {
        return Path.Combine(Application.persistentDataPath, slotName + "." + Format);
    }

    private void FindAllDataHandlers()
    {
        _dataHandlers = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>().ToList();
    }

    public SaveData CreateDefaultSave()
    {
        return new SaveData
        {
            Meta = new MetaData
            {
                SaveName = SaveSlotName
            }
        };
    }


    public void NewGame(string slotName)
    {
        SaveSlotName = slotName;
        SaveData = CreateDefaultSave();
        SaveGame();
    }

    public void ManualRefreshHandlers()
    {
        FindAllDataHandlers(); // Call this if new objects are created dynamically at runtime
    }

    private string EncryptDecrypt(string text)
    {
        var result = new StringBuilder(text.Length);
        foreach (var c in text) result.Append((char)(c ^ 129)); // Simple XOR cipher

        return result.ToString();
    }

    public PlayerStateData GetCharacterData(CharacterType type)
    {
        var entry = SaveData.CharacterStates.FirstOrDefault(e => e.Type == type);
        if (entry == null)
        {
            entry = new CharacterStateEntry
            {
                Type = type,
                State = new PlayerStateData()
            };
            SaveData.CharacterStates.Add(entry);
        }

        return entry.State;
    }


    public void SaveGame()
    {
        FindAllDataHandlers();

        UpdateMetaData();
        _sessionStartTime = DateTime.Now;

        foreach (var handler in _dataHandlers) handler.SaveData(SaveData);

        var json = JsonUtility.ToJson(SaveData);
        using var stream = new FileStream(GetSavePath(SaveSlotName), FileMode.OpenOrCreate, FileAccess.Write);
        // _formatter.Serialize(stream, json);
        _formatter.Serialize(stream, EncryptDecrypt(json));
    }

    private void UpdateMetaData()
    {
        SaveData.Meta.LastSaveTime = DateTime.Now;
        SaveData.Meta.PlayTimeSeconds = (float)(DateTime.Now - _sessionStartTime).TotalSeconds;
        SaveData.Meta.SaveName = SaveSlotName;

        SaveData.Meta.ScreenWidth = Screen.currentResolution.width;
        SaveData.Meta.ScreenHeight = Screen.currentResolution.height;
        SaveData.Meta.Fullscreen = Screen.fullScreen;
    }

    public void LoadGame(string slotName)
    {
        FindAllDataHandlers();
        var path = GetSavePath(slotName);
        SaveSlotName = slotName;

        if (!File.Exists(path))
        {
            // Debug.LogWarning($"Save file for slot '{slotName}' not found. Creating new save.");
            SaveData = CreateDefaultSave();

            SaveData.Meta.SaveName = slotName;

            SaveGame();
        }
        else
        {
            using var stream = new FileStream(path, FileMode.Open);
            var data = (string)_formatter.Deserialize(stream);
            SaveData = JsonUtility.FromJson<SaveData>(EncryptDecrypt(data));
            // SaveData = JsonUtility.FromJson<SaveData>(data);
            // Debug.Log($"load {SaveData.CharacterStates[CharacterType.Robot].HitsRemaining}");
        }

        foreach (var handler in _dataHandlers) handler.LoadData(SaveData);
        Screen.SetResolution(
            SaveData.Meta.ScreenWidth,
            SaveData.Meta.ScreenHeight,
            SaveData.Meta.Fullscreen
        );
    }

    public void DeleteGame(string slotName)
    {
        var path = GetSavePath(slotName);
        if (File.Exists(path))
            File.Delete(path);
    }

    public void OnClick_Continue()
    {
        Instance.LoadGame(Instance.SaveSlotName);
    }

    public void OnClick_NewGame(string slotName)
    {
        Instance.NewGame(slotName);
    }

    public void OnClick_LoadSave(string saveName)
    {
        Instance.LoadGame(saveName);
    }

    /// <summary>
    ///     to disable the continue button if there are no games saved
    /// </summary>
    /// <param name="slotName">the name of the last game played</param>
    /// <returns></returns>
    public bool HasSave(string slotName)
    {
        return File.Exists(Path.Combine(Application.persistentDataPath, slotName + "." + Format));
    }
}