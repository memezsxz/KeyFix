using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the activation and progression through space corridors using button sequences.
/// Controls scaler access and win condition state.
/// </summary>
public class SpaceManager : Singleton<SpaceManager>
{
    #region Serialized Fields

    [Header("Corridors in order")]
    /// <summary>
    /// Ordered list of corridors the player must progress through.
    /// </summary>
    [SerializeField]
    private List<Corridor> corridors = new();


    /// <summary>
    /// Buttons that control progression, matched to each corridor.
    /// </summary>
    [Header("Buttons in order")] [SerializeField]
    private List<SpaceButtonInteraction> spaceButtons = new();

    /// <summary>
    /// Collider for the shrinker area (disabled during corridor traversal).
    /// </summary>
    [SerializeField] private Collider shrinker;

    /// <summary>
    /// Collider for the stretcher area (disabled during corridor traversal).
    /// </summary>
    [SerializeField] private Collider stretcher;

    #endregion

    #region Private Fields

    /// <summary>
    /// Index of the currently active corridor.
    /// </summary>
    private int currentCorridorIndex = -1;

    /// <summary>
    /// Whether a corridor is currently active and must be completed before continuing.
    /// </summary>
    private bool isCorridorActive;

    #endregion

    #region Public Properties

    /// <summary>
    /// Indicates whether the player has completed all corridors.
    /// </summary>
    public bool DidWin { get; private set; }

    #endregion

    private void Start()
    {
        // Enable only the first button in the sequence
        for (var i = 0; i < spaceButtons.Count; i++)
        {
            if (spaceButtons[i] != null)
                spaceButtons[i].gameObject.SetActive(i == 0);
        }

        DeactivateAllCorridors();
    }

    /// <summary>
    /// Deactivates the current corridor and re-enables scalers.
    /// </summary>
    public void DeactivateCurrentCorridor()
    {
        if (currentCorridorIndex < 0 || currentCorridorIndex >= corridors.Count)
            return;

        var currentCorridor = corridors[currentCorridorIndex];
        if (currentCorridor != null)
        {
            currentCorridor.DeactivateCorridor();
            currentCorridor.gameObject.SetActive(false);
        }

        isCorridorActive = false;
        ToggleScaler();
    }

    /// <summary>
    /// Activates the next corridor in the sequence.
    /// Disables scalers and shows the next progression button.
    /// </summary>
    public void ActivateNextCorridor()
    {
        if (isCorridorActive)
        {
            Debug.LogWarning("Cannot activate next corridor: current corridor still active. Deactivate first.");
            return;
        }

        currentCorridorIndex++;

        // Check if final corridor has been reached
        if (currentCorridorIndex >= corridors.Count - 1)
        {
            // Disable final button
            var lastButtonIndex = Mathf.Min(currentCorridorIndex, spaceButtons.Count - 1);
            var lastButton = spaceButtons[lastButtonIndex];
            if (lastButton != null)
                lastButton.SetGray();

            GameManager.Instance.ChangeState(GameManager.GameState.Victory);
            return;
        }

        // Activate the next corridor
        var nextCorridor = corridors[currentCorridorIndex];
        if (nextCorridor != null)
        {
            nextCorridor.gameObject.SetActive(true);
            nextCorridor.ActivateCorridor();
        }

        // Activate the next button if available
        var nextButton = spaceButtons[(currentCorridorIndex + 1) % spaceButtons.Count];
        if (nextButton != null)
        {
            // Debug.Log("Activated next button: " + nextButton.gameObject.name);
            nextButton.gameObject.SetActive(true);
        }

        isCorridorActive = true;
        ToggleScaler(); // Disable scalers while inside a corridor
    }

    /// <summary>
    /// Disables all corridors and resets scaler state.
    /// </summary>
    public void DeactivateAllCorridors()
    {
        foreach (var corridor in corridors)
        {
            if (corridor != null)
            {
                corridor.DeactivateCorridor();
                corridor.gameObject.SetActive(false);
            }
        }

        isCorridorActive = false;
        ToggleScaler();
    }

    /// <summary>
    /// Resets all progress and deactivates all corridors.
    /// </summary>
    public void ResetSpace()
    {
        currentCorridorIndex = -1;
        DeactivateAllCorridors();
    }

    /// <summary>
    /// Enables or disables the shrinker and stretcher depending on corridor state.
    /// </summary>
    private void ToggleScaler()
    {
        if (shrinker != null) shrinker.enabled = !isCorridorActive;

        if (stretcher != null) stretcher.enabled = !isCorridorActive;
    }
}