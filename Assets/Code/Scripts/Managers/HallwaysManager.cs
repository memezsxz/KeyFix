using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Code.Scripts.Interactable;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Manages hallway logic including door visibility, player positioning, and collectables UI.
/// Implements data persistence for hallway position and rotation.
/// </summary>
public class HallwaysManager : Singleton<HallwaysManager>, IDataPersistence
{
    /// <summary>
    /// List of all hallway room doors found in the scene.
    /// </summary>
    [FormerlySerializedAs("doorInteract")] [SerializeField]
    private List<HallwayRoomDoor> doors = new();

    /// <summary>
    /// Text element displaying the number of collectables found.
    /// </summary>
    [SerializeField] private TextMeshProUGUI collectablesText;

    private void Start()
    {
        // Dynamically find and assign all doors by tag
        GameObject.FindGameObjectsWithTag("Door").ToList()
            .ForEach(gm => doors.Add(gm.GetComponentInChildren<HallwayRoomDoor>()));

        // Set completed doors to default layer (non-interactable)
        doors.ForEach(d =>
        {
            if (SaveManager.Instance.SaveData.Progress.RepairedKeys.Contains(d.Scene))
                d.gameObject.layer = LayerMask.NameToLayer("Default");
        });
    }

    /// <summary>
    /// Saves the player's position and rotation in the hallway if available.
    /// </summary>
    public void SaveData(ref SaveData data)
    {
        var psd = data.CharacterStates.FirstOrDefault(cse => cse.Type == CharacterType.Robot)?.State;
        var tf = GameManager.Instance.GetPlayerTransform();

        if (psd != null && tf != null)
        {
            psd.HallwaysPosition = tf.position;
            psd.HallwaysRotation = tf.rotation;
        }
    }

    /// <summary>
    /// Restores player location and UI state based on saved data.
    /// </summary>
    public void LoadData(ref SaveData data)
    {
        var psd = data.CharacterStates.FirstOrDefault(cse => cse.Type == CharacterType.Robot)?.State;

        if (psd != null)
            GameManager.Instance.MovePlayerTo(psd.HallwaysPosition, psd.HallwaysRotation);

        UpdateInteractablesUI(data.Progress.CollectablesCount);
    }

    /// <summary>
    /// Initiates coroutine to move player to the exit door of the completed room.
    /// </summary>
    public void HandleVictory(GameManager.Scenes scene)
    {
        StartCoroutine(DelayedMoveToExit(scene));
    }

    /// <summary>
    /// Waits for the player to be available, then moves them to the corresponding exit door.
    /// </summary>
    private IEnumerator DelayedMoveToExit(GameManager.Scenes scene)
    {
        yield return null; // Wait 1 frame
        yield return new WaitUntil(() => GameManager.Instance.GetPlayerTransform() != null);

        var door = doors.FirstOrDefault(d => d.Scene == scene && d.type == HallwayRoomDoor.DoorType.Exit);

        if (door != null)
        {
            door.HandleExitDoor();
            SaveManager.Instance.SaveGame();
        }
        else
        {
            Debug.LogWarning("Exit door not found for scene: " + scene);
        }
    }

    /// <summary>
    /// Updates the UI text displaying number of collected items.
    /// </summary>
    public void UpdateInteractablesUI(int value)
    {
        collectablesText.text = value.ToString();
    }
}