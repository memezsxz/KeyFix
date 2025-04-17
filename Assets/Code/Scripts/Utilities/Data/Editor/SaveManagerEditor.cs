using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;


// TODO Maryam: check that the functionality is working, new games are loaded, save is happening correctly
[CustomEditor(typeof(SaveManager))]
public class SaveManagerEditor : Editor
{
    [HideInInspector] public int SelectedSlotIndex;
    private string[] _availableSaveSlots = Array.Empty<string>();
    private string _customSlotName = "NewSaveSlot";
    private int _selectedSlotIndex;
    private bool _initialized = false;

    public string SaveSlotName =>
        (_availableSaveSlots != null && SelectedSlotIndex >= 0 && SelectedSlotIndex < _availableSaveSlots.Length)
            ? _availableSaveSlots[SelectedSlotIndex]
            : "DefaultSaveSlot";

    private void OnEnable()
    {
        if (!EditorApplication.isPlaying) return;
        RefreshSaveSlotList();
    }


    public override void OnInspectorGUI()
    {
        if (!EditorApplication.isPlaying) return;

        var saveManager = (SaveManager)target;


        GUILayout.Space(15);
        GUILayout.Label("Editor Save Controls", EditorStyles.boldLabel);

        RefreshSaveSlotList();

        if (_availableSaveSlots.Length > 0)
        {
            // Only initialize once
            if (!_initialized)
            {
                string lastGame = PlayerPrefs.GetString(SaveManager.LAST_GAME_PREF);
                int index = Array.IndexOf(_availableSaveSlots, lastGame);

                if (!string.IsNullOrWhiteSpace(lastGame) && index >= 0)
                {
                    _selectedSlotIndex = index;
                }

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

        if (GUILayout.Button("New Game"))
        {
            saveManager.NewGame(_customSlotName);
            Debug.Log("New game data created.");
            RefreshSaveSlotList();
        }

        if (GUILayout.Button("Save Game"))
        {
            saveManager.SaveGame();
            // Debug.Log("Game saved.");
            RefreshSaveSlotList();
        }

        if (GUILayout.Button("Load Game"))
        {
            saveManager.LoadGame(saveManager.SaveSlotName);
            // Debug.Log("Game loaded.");
            RefreshSaveSlotList();
        }

        if (GUILayout.Button("Delete Game"))
        {
            saveManager.DeleteGame(saveManager.SaveSlotName);
            // Debug.Log("Game deleted.");
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