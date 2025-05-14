using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom Unity editor for SaveManager that enables interaction with save slots during play mode.
/// Provides buttons for New Game, Save, Load, Delete, and opening the save directory.
/// </summary>
[CustomEditor(typeof(SaveManager))]
public class SaveManagerEditor : Editor
{
    /// <summary>
    /// Currently selected index from the list of available save slots.
    /// </summary>
    [HideInInspector] public int SelectedSlotIndex;

    private string[] _availableSaveSlots = Array.Empty<string>();
    private string _customSlotName = "Game1";
    private bool _initialized;
    private int _selectedSlotIndex;

    /// <summary>
    /// Gets the currently selected save slot name.
    /// Defaults to "Game1" if invalid.
    /// </summary>
    public string SaveSlotName =>
        _availableSaveSlots != null && SelectedSlotIndex >= 0 && SelectedSlotIndex < _availableSaveSlots.Length
            ? _availableSaveSlots[SelectedSlotIndex]
            : "Game1";

    /// <summary>
    /// Loads available save slots when the editor is enabled and in play mode.
    /// </summary>
    private void OnEnable()
    {
        if (!EditorApplication.isPlaying) return;
        RefreshSaveSlotList();
    }

    /// <summary>
    /// Draws the custom inspector GUI for interacting with save slots.
    /// </summary>
    public override void OnInspectorGUI()
    {
        if (!EditorApplication.isPlaying) return;

        var saveManager = (SaveManager)target;

        GUILayout.Space(15);
        GUILayout.Label("Editor Save Controls", EditorStyles.boldLabel);

        RefreshSaveSlotList();

        if (_availableSaveSlots.Length > 0)
        {
            // Only run once to match last used slot from PlayerPrefs
            if (!_initialized)
            {
                var lastGame = PlayerPrefs.GetString(SaveManager.LAST_GAME_PREF);
                var index = Array.IndexOf(_availableSaveSlots, lastGame);

                if (!string.IsNullOrWhiteSpace(lastGame) && index >= 0)
                    _selectedSlotIndex = index;

                _initialized = true;
            }

            _selectedSlotIndex = EditorGUILayout.Popup("Existing Save Slots", _selectedSlotIndex, _availableSaveSlots);
            saveManager.SaveSlotName = _availableSaveSlots[_selectedSlotIndex];
        }
        else
        {
            EditorGUILayout.HelpBox("No existing save files found.", MessageType.Info);
        }

        GUILayout.Space(10);
        GUILayout.Label("Or create a new slot:", EditorStyles.miniBoldLabel);
        _customSlotName = EditorGUILayout.TextField("Custom Slot Name", _customSlotName);

        GUILayout.Space(5);

        // Button actions
        if (GUILayout.Button("New Game"))
        {
            saveManager.NewGame(_customSlotName);
            Debug.Log("New game data created.");
            RefreshSaveSlotList();
        }

        if (GUILayout.Button("Save Game"))
        {
            saveManager.SaveGame();
            RefreshSaveSlotList();
        }

        if (GUILayout.Button("Load Game"))
        {
            saveManager.LoadGame(saveManager.SaveSlotName);
            RefreshSaveSlotList();
        }

        if (GUILayout.Button("Delete Game"))
        {
            saveManager.DeleteGame(saveManager.SaveSlotName);
            RefreshSaveSlotList();
        }

        GUILayout.Space(10);
        if (GUILayout.Button("Open Save Folder"))
        {
            var saveFolder = Application.persistentDataPath;
            EditorUtility.RevealInFinder(saveFolder);
            RefreshSaveSlotList();
        }
    }

    /// <summary>
    /// Scans the persistent data folder for save files matching the expected extension.
    /// Populates the `_availableSaveSlots` array.
    /// </summary>
    private void RefreshSaveSlotList()
    {
        var saveDir = Application.persistentDataPath;

        if (!Directory.Exists(saveDir))
        {
            _availableSaveSlots = Array.Empty<string>();
            return;
        }

        _availableSaveSlots = Directory.GetFiles(saveDir, "*." + SaveManager.Instance.Format)
            .Select(Path.GetFileNameWithoutExtension)
            .ToArray();
    }
}
