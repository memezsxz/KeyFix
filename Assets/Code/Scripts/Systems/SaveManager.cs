using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

/// <summary>
///     Manages saving and loading game state to disk using a slot-based system.
///     Handles data persistence for any objects implementing IDataPersistence.
/// </summary>
public class SaveManager : Singleton<SaveManager>
{
    #region Fields & Properties

    [Header("Save Slot")] public string SaveSlotName = "Game1";

    public bool IsNewGame { get; private set; }

    private List<IDataPersistence> _dataHandlers;
    public static readonly string LAST_GAME_PREF = "LastGame";

    private readonly BinaryFormatter _formatter = new();
    private DateTime _sessionStartTime;

    private SaveData _saveData = new();

    public SaveData SaveData
    {
        get => _saveData;
        private set => _saveData = value;
    }

    public string Format => "json";
    private readonly bool _encrypt = false;

    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        _sessionStartTime = DateTime.Now;

        // Load last save slot from PlayerPrefs
        IsNewGame = PlayerPrefs.GetString(LAST_GAME_PREF) == null;

        SaveSlotName = PlayerPrefs.GetString(LAST_GAME_PREF, "Game1");
        IsNewGame = !File.Exists(GetSavePath(SaveSlotName));

        // Auto-load last game
        LoadGame(SaveSlotName);

        // Register debug command to manually trigger save
        DebugController.Instance?.AddDebugCommand(new DebugCommand("save_game", "saved the game", "",
            () => SaveGame()));
    }

    #endregion

    #region Character Save Support

    /// <summary>
    /// Retrieves (or initializes) the state data for a given character.
    /// </summary>
    /// <param name="type">Character type to retrieve state for.</param>
    /// <returns>The player's saved state data.</returns>
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
    /// <param name="slotName">Save slot name.</param>
    public void LoadGame(string slotName)
    {
        FindAllDataHandlers();
        SaveSlotName = slotName;

        var path = GetSavePath(slotName);

        if (!File.Exists(path))
        {
            IsNewGame = true;
            SaveData = CreateDefaultSave();
            SaveData.Meta.SaveName = slotName;
            SaveGame();
        }
        else
        {
            IsNewGame = false;
            using var stream = new FileStream(path, FileMode.Open);
            var data = (string)_formatter.Deserialize(stream);

            SaveData = _encrypt
                ? JsonUtility.FromJson<SaveData>(EncryptDecrypt(data))
                : JsonUtility.FromJson<SaveData>(data);
        }

        foreach (var handler in _dataHandlers)
            handler.LoadData(ref _saveData);
    }

    /// <summary>
    /// Deletes the save file associated with a given slot.
    /// </summary>
    /// <param name="slotName">Save slot name to delete.</param>
    public void DeleteGame(string slotName)
    {
        var path = GetSavePath(slotName);
        if (File.Exists(path))
            File.Delete(path);
    }

    /// <summary>
    /// Reloads the current save slot from disk.
    /// </summary>
    public void RollBack()
    {
        LoadGame(SaveSlotName);
    }

    /// <summary>
    /// Starts a new game using current slot name.
    /// </summary>
    public void StartNewGame()
    {
        IsNewGame = true;

        PlayerPrefs.SetString(LAST_GAME_PREF, SaveSlotName);
        PlayerPrefs.Save();

        SaveData = CreateDefaultSave();
        SaveGame();
    }

    #endregion

    #region Settings Public API

    /// <summary>
    /// Saves sound and graphics settings to file.
    /// </summary>
    public void SaveSettings()
    {
        SoundManager.Instance.SaveData(ref _saveData);
        GraphicsManager.Instance.SaveData(ref _saveData);
        WriteToFile();

        SoundManager.Instance.LoadData(ref _saveData);
        GraphicsManager.Instance.LoadData(ref _saveData);
    }

    /// <summary>
    /// Resets sound and graphics settings to defaults.
    /// </summary>
    public void ResetSettings()
    {
        _saveData.Graphics = new GraphicData();
        _saveData.Sounds = new SoundData();

        WriteToFile();

        SoundManager.Instance.LoadData(ref _saveData);
        GraphicsManager.Instance.LoadData(ref _saveData);
    }

    #endregion

    #region Meta & Slot Helpers

    /// <summary>
    /// Updates metadata values such as time and resolution.
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
    /// Finds all MonoBehaviour objects in the scene that implement IDataPersistence.
    /// </summary>
    private void FindAllDataHandlers()
    {
        _dataHandlers = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>().ToList();
    }

    #endregion

    #region Player Movement Public API

    /// <summary>
    /// Loads player control bindings from saved data.
    /// </summary>
    public void LoadPlayerBindings()
    {
        var pbm = FindObjectOfType<PlayerBindingManage>();
        if (pbm) pbm.LoadData(ref _saveData);
    }

    /// <summary>
    /// Saves player position in the hallway scene.
    /// </summary>
    public void SaveHallwayPosition()
    {
        if (GameManager.Instance.CurrentScene != GameManager.Scenes.HALLWAYS) return;
        HallwaysManager.Instance.SaveData(ref _saveData);
    }

    /// <summary>
    /// Loads player position in the hallway scene.
    /// </summary>
    public void LoadHallwayPosition()
    {
        if (GameManager.Instance.CurrentScene != GameManager.Scenes.HALLWAYS) return;
        HallwaysManager.Instance.LoadData(ref _saveData);
    }

    #endregion

    #region File System

    /// <summary>
    /// Checks if a save file exists for the given slot name.
    /// </summary>
    public bool HasSave(string slotName)
    {
        return File.Exists(GetSavePath(slotName));
    }

    /// <summary>
    /// Returns full file path of a save slot.
    /// </summary>
    private string GetSavePath(string slotName)
    {
        return Path.Combine(Application.persistentDataPath, $"{slotName}.{Format}");
    }

    /// <summary>
    /// Encrypts or decrypts data using XOR cipher.
    /// </summary>
    private string EncryptDecrypt(string text)
    {
        var result = new StringBuilder(text.Length);
        foreach (var c in text) result.Append((char)(c ^ 129));
        return result.ToString();
    }

    /// <summary>
    /// Serializes SaveData and writes it to disk.
    /// </summary>
    private void WriteToFile()
    {
        var json = JsonUtility.ToJson(SaveData);

        using var stream = new FileStream(GetSavePath(SaveSlotName), FileMode.OpenOrCreate, FileAccess.Write);
        _formatter.Serialize(stream, _encrypt ? EncryptDecrypt(json) : json);
    }

    #endregion

    #region UI Event Handlers

    /// <summary>
    /// Continues the game by loading current save slot.
    /// </summary>
    public void OnClick_Continue()
    {
        LoadGame(SaveSlotName);
    }

    /// <summary>
    /// Creates a new game in the specified save slot.
    /// </summary>
    public void OnClick_NewGame(string slotName)
    {
        NewGame(slotName);
    }

    /// <summary>
    /// Loads a save file based on user input.
    /// </summary>
    public void OnClick_LoadSave(string saveName)
    {
        LoadGame(saveName);
    }

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

    #region Helpers

    /// <summary>
    /// Creates a default save structure.
    /// </summary>
    private SaveData CreateDefaultSave()
    {
        var defaultBindingSet = Resources.Load<InputBindingSet>("InputBindingSet");
        if (defaultBindingSet == null) Debug.LogError("Default InputBindingSet not found in Resources folder!");

        return new SaveData
        {
            Meta = new MetaData
            {
                SaveName = SaveSlotName
            },
            Progress = new ProgressData
            {
                BindingOverrides = DeepCopyBindingSet(defaultBindingSet)
            }
        };
    }

    /// <summary>
    /// Copies input bindings into a serializable form.
    /// </summary>
    private SerializableInputBindingSet DeepCopyBindingSet(InputBindingSet original)
    {
        return new SerializableInputBindingSet
        {
            bindings = original.bindings
                .Select(b => new InputBindingConfig
                {
                    actionName = b.actionName,
                    bindingName = b.bindingName,
                    defaultPath = b.defaultPath,
                    isUnlocked = b.isUnlocked
                }).ToList()
        };
    }

    #endregion
}