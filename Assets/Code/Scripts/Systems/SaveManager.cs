using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

/// <summary>
/// Manages saving and loading game state to disk using a slot-based system.
/// Handles data persistence for any objects implementing IDataPersistence.
/// </summary>
public class SaveManager : Singleton<SaveManager>
{
    #region Fields & Properties

    [Header("Save Slot")] public string SaveSlotName = "Save1";
    public bool IsNewGame { get; private set; } = false;
    private List<IDataPersistence> _dataHandlers;
    public static readonly string LAST_GAME_PREF = "LastGame";

    private readonly BinaryFormatter _formatter = new();
    private DateTime _sessionStartTime;

    private SaveData _saveData = new SaveData();

    public SaveData SaveData
    {
        get => _saveData;
        private set => _saveData = value;
    }

    public string Format => "json";

    private bool _encrypt = false;

    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        _sessionStartTime = DateTime.Now;

        // Load last save slot from PlayerPrefs
        IsNewGame = PlayerPrefs.GetString(LAST_GAME_PREF) == null;

        SaveSlotName = PlayerPrefs.GetString(LAST_GAME_PREF, "Slot1");
        // Debug.Log("Using save slot: " + SaveSlotName);

        FindAllDataHandlers();

        // Optional: Auto-load
        // LoadGame(SaveSlotName);

        DebugController.Instance.AddDebugCommand(new DebugCommand("save_game", "saved the game", "", () => SaveGame()));
    }

    #endregion

    #region Save/Load Public API

    /// <summary>
    /// Saves the current game state to disk.
    /// </summary>
    public void SaveGame()
    {
        FindAllDataHandlers();
        UpdateMetaData();
        _sessionStartTime = DateTime.Now;

        foreach (var handler in _dataHandlers)
            handler.SaveData(ref _saveData);
        
        IsNewGame = !File.Exists(GetSavePath(_saveData.Meta.SaveName));

        WriteToFile();
    }


    /// <summary>
    /// Loads game data from disk, or creates a new save if file doesn't exist.
    /// </summary>
    public void LoadGame(string slotName)
    {
        FindAllDataHandlers();
        SaveSlotName = slotName;

        string path = GetSavePath(slotName);

        if (!File.Exists(path))
        {
            IsNewGame = true;
            SaveData = CreateDefaultSave();
            SaveData.Meta.SaveName = slotName;
            SaveGame();
            // Debug.Log("New game saved");
        }
        else
        {
            IsNewGame = false;
            using var stream = new FileStream(path, FileMode.Open);
            var data = (string)_formatter.Deserialize(stream);

            SaveData = _encrypt
                ? JsonUtility.FromJson<SaveData>(EncryptDecrypt(data))
                : JsonUtility.FromJson<SaveData>(data);
            // Debug.Log("old game saved");

        }

        foreach (var handler in _dataHandlers)
            handler.LoadData(ref _saveData);
    }

    /// <summary>
    /// Deletes the save file associated with a given slot.
    /// </summary>
    public void DeleteGame(string slotName)
    {
        var path = GetSavePath(slotName);
        if (File.Exists(path))
            File.Delete(path);
    }

    public void RollBack()
    {
        LoadGame(SaveSlotName);
    }

    /// <summary>
    /// Creates a new SaveData object with default values.
    /// </summary>
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

    #endregion

    #region Settings Public API

    public void SaveSettings()
    {
        SoundManager.Instance.SaveData(ref _saveData);
        GraphicsManager.Instance.SaveData(ref _saveData);
        WriteToFile();

        // print($"Saved game settings: sound {_saveData.Sounds.SoundVolume}, music {_saveData.Sounds.MusicVolume}, quality {_saveData.Graphics.QualityName}, resolution {_saveData.Graphics.ResolutionWidth}x{_saveData.Graphics.ResolutionHeight}");
       
        SoundManager.Instance.LoadData(ref _saveData);
        GraphicsManager.Instance.LoadData(ref _saveData);
    }

    public void ResetSettings()
    {
        // UnityEngine.Device.Screen.
        _saveData.Graphics = new GraphicData();
        _saveData.Sounds = new SoundData();

        WriteToFile();
        
        SoundManager.Instance.LoadData(ref _saveData);
        GraphicsManager.Instance.LoadData(ref _saveData);

    }

    #endregion

    #region Meta & Slot Helpers

    /// <summary>
    /// Updates the SaveData meta with resolution, playtime, and save info.
    /// </summary>
    private void UpdateMetaData()
    {
        SaveData.Meta.LastSaveTime = DateTime.Now;
        SaveData.Meta.PlayTimeSeconds += (float)(DateTime.Now - _sessionStartTime).TotalSeconds;
        SaveData.Meta.SaveName = SaveSlotName;

        SaveData.Meta.ScreenWidth = Screen.width;
        SaveData.Meta.ScreenHeight = Screen.height;
        SaveData.Graphics.Fullscreen = Screen.fullScreen;
    }


    /// <summary>
    /// Forces a refresh of the list of data handlers.
    /// Useful if new handlers are created at runtime.
    /// </summary>
    // public void ManualRefreshHandlers() =>
    //     FindAllDataHandlers();


    /// <summary>
    /// Scans the scene for all MonoBehaviours implementing IDataPersistence.
    /// </summary>
    private void FindAllDataHandlers()
    {
        _dataHandlers = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>().ToList();
    }

    #endregion

    #region File System
 
    public bool HasSave(string slotName) =>
        File.Exists(GetSavePath(slotName));

    /// <summary>
    /// Constructs the full path to the save file for a given slot.
    /// </summary>
    private string GetSavePath(string slotName) =>
        Path.Combine(Application.persistentDataPath, $"{slotName}.{Format}");

    /// <summary>
    /// Simple XOR encryption/decryption used for optional save file obfuscation.
    /// </summary>
    private string EncryptDecrypt(string text)
    {
        var result = new StringBuilder(text.Length);
        foreach (char c in text) result.Append((char)(c ^ 129)); // XOR obfuscation
        return result.ToString();
    }

    /// <summary>
    /// Writes the save data to file 
    /// </summary>
    private void WriteToFile()
    {
        string json = JsonUtility.ToJson(SaveData);

        using var stream = new FileStream(GetSavePath(SaveSlotName), FileMode.OpenOrCreate, FileAccess.Write);
        _formatter.Serialize(stream, _encrypt ? EncryptDecrypt(json) : json);
    }

    #endregion

    #region Character Save Support

    /// <summary>
    /// Retrieves (or initializes) the state data for a given character.
    /// </summary>
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

    #endregion

    #region UI Event Handlers

    public void OnClick_Continue() => LoadGame(SaveSlotName);

    public void OnClick_NewGame(string slotName) => NewGame(slotName);

    public void OnClick_LoadSave(string saveName) => LoadGame(saveName);

    /// <summary>
    /// Starts a new game using a given save slot name.
    /// </summary>
    public void NewGame(string slotName)
    {
        SaveSlotName = slotName;
        SaveData = CreateDefaultSave();
        SaveGame();
    }

    #endregion
}