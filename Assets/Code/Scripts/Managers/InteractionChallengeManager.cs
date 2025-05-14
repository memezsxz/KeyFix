using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Manages a sequence of interactable objects that the player must complete in order.
/// Tracks and saves player progress and updates the score UI.
/// </summary>
public class InteractionChallengeManager : Singleton<InteractionChallengeManager>, IDataPersistence
{
    /// <summary>
    /// UI text element that displays the current score.
    /// </summary>
    [SerializeField] private TextMeshProUGUI scoreText;

    /// <summary>
    /// List of all interactable objects in the challenge sequence.
    /// </summary>
    [SerializeField] private List<GameObject> interactables;

    /// <summary>
    /// Number of successfully completed interactables.
    /// </summary>
    private int currentScore;

    private void Start()
    {
        UpdateUI();
        EnableNextItem();
    }

    private void OnEnable()
    {
        Start();
    }

    /// <summary>
    /// Saves the current progress to the save data.
    /// </summary>
    public void SaveData(ref SaveData data)
    {
        data.Progress.CollectablesCount = currentScore;
    }

    /// <summary>
    /// Loads progress from the save data and sets up the next interactable.
    /// </summary>
    public void LoadData(ref SaveData data)
    {
        // Debug.Log("Loading data in w room");
        currentScore = data.Progress.CollectablesCount;
        UpdateUI();
        EnableNextItem();
    }

    /// <summary>
    /// Activates the next interactable object and disables all others.
    /// </summary>
    private void EnableNextItem()
    {
        var defaultLayer = LayerMask.NameToLayer("Default");
        var interactableLayer = LayerMask.NameToLayer("Interactable");

        // Disable all interactables first
        interactables.ForEach(i => SetLayerRecursively(i, defaultLayer));
        SetLayerRecursively(interactables[currentScore], interactableLayer);
    }

    /// <summary>
    /// Recursively sets the layer of an object and all its children.
    /// </summary>
    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, newLayer);
    }

    /// <summary>
    /// Increments the player's score and enables the next interactable or ends the challenge.
    /// </summary>
    public void IncrementScore()
    {
        currentScore++;
        UpdateUI();

        if (currentScore >= interactables.Count)
            GameManager.Instance.ChangeState(GameManager.GameState.Victory);
        else
            EnableNextItem();
    }

    /// <summary>
    /// Updates the on-screen UI to reflect the current score.
    /// </summary>
    private void UpdateUI()
    {
        scoreText.text = $"{currentScore} / {interactables.Count}";
    }
}