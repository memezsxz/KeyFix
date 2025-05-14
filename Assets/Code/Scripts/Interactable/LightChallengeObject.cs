using UnityEngine;

/// <summary>
/// Represents an interactable object that spawns and manages a light-based shape challenge.
/// Disables player movement during the challenge and re-enables it upon completion or exit.
/// </summary>
public class LightChallengeObject : InteractableBase
{
    #region Serialized Fields

    /// <summary>
    /// The prefab that holds the challenge UI and logic.
    /// </summary>
    [SerializeField] private GameObject challengePrefab;

    #endregion

    #region Private Fields

    /// <summary>
    /// The instance of the challenge prefab that gets spawned.
    /// </summary>
    private GameObject challengeInstance;

    /// <summary>
    /// Whether the challenge is currently active and visible.
    /// </summary>
    private bool isActive;

    /// <summary>
    /// Reference to the shape checker component used to monitor challenge progress.
    /// </summary>
    private ShapeChecker shapeChecker;

    #endregion

    #region Interaction Logic

    /// <summary>
    /// Handles player interaction: toggles challenge UI and registers completion callback.
    /// </summary>
    public override void Interact()
    {
        if (!isActive)
        {
            // Instantiate the challenge if it hasn't been already
            if (!challengeInstance)
            {
                challengeInstance = Instantiate(challengePrefab, transform.position, Quaternion.identity);
                shapeChecker = challengeInstance.GetComponentInChildren<ShapeChecker>();

                // Register callback to handle success scenario
                shapeChecker.successCallback += HandleWin;
            }

            // Show the challenge and disable player movement
            challengeInstance.SetActive(true);
            isActive = true;
            GameManager.Instance.TogglePlayerMovement(false);
        }
        else
        {
            // Hide the challenge and re-enable player movement
            isActive = false;
            challengeInstance.SetActive(false);
            GameManager.Instance.TogglePlayerMovement(true);
        }
    }

    /// <summary>
    /// Called when the shape challenge is successfully completed.
    /// Updates the object state, hides the challenge, and awards points.
    /// </summary>
    private void HandleWin()
    {
        isActive = false;
        challengeInstance.SetActive(false);

        // Make the object no longer interactable
        gameObject.layer = LayerMask.NameToLayer("Default");

        // Restore movement and increase score
        GameManager.Instance.TogglePlayerMovement(true);
        InteractionChallengeManager.Instance.IncrementScore();
    }

    #endregion
}
