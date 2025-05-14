using UnityEngine;

/// <summary>
/// Represents an interactable light puzzle board that toggles a color-based challenge UI.
/// Requires a <see cref="BulbColorController"/> to manage logic.
/// </summary>
[RequireComponent(typeof(BulbColorController))]
public class LightBoard : InteractableBase
{
    #region Serialized Fields

    /// <summary>
    /// Renderer for the bulb mesh (used for material/color feedback).
    /// </summary>
    [SerializeField] private MeshRenderer bulbRenderer;

    /// <summary>
    /// Material to apply when the bulb is in a red state.
    /// </summary>
    [SerializeField] private Material bulbRedMaterial;

    /// <summary>
    /// The canvas UI to be shown when the board is activated.
    /// </summary>
    [SerializeField] private GameObject canvas;

    #endregion

    #region Private Fields

    /// <summary>
    /// Reference to the bulb color controller that handles puzzle logic.
    /// </summary>
    private BulbColorController bulbColorController;

    /// <summary>
    /// Tracks whether the canvas is currently active.
    /// </summary>
    private bool isActive;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        // Get the required BulbColorController and ensure the canvas starts hidden
        bulbColorController = gameObject.GetComponent<BulbColorController>();
        if (canvas != null) canvas.SetActive(false); // Always hide the canvas initially
    }

    private void Start()
    {
        // Set the hint message for interaction
        InteractMessage = "E";
    }

    #endregion

    #region Interaction Logic

    /// <summary>
    /// Called when the player interacts with the light board.
    /// If the challenge is already complete, closes the UI and re-enables movement.
    /// Otherwise, toggles the UI canvas and starts/stops the color challenge.
    /// </summary>
    public override void Interact()
    {
        // If the puzzle is already completed, just close the UI and allow movement again
        if (bulbColorController.IsDone)
        {
            canvas.SetActive(false);
            GameManager.Instance.TogglePlayerMovement(true);
            return;
        }

        // Determine if the UI canvas is already visible
        isActive = canvas.gameObject.activeSelf;

        if (!isActive)
        {
            // Show the puzzle UI and disable player movement during interaction
            canvas.SetActive(true);
            bulbColorController.ShowArrowChallenge();
            GameManager.Instance.TogglePlayerMovement(false);
        }
        else
        {
            // Hide the puzzle UI and re-enable player movement
            canvas.SetActive(false);
            GameManager.Instance.TogglePlayerMovement(true);
        }
    }

    /// <summary>
    /// External call to forcibly show or hide the canvas and enable/disable controller logic.
    /// </summary>
    /// <param name="value">True to enable interaction, false to disable and hide UI.</param>
    public void ToggleCanvas(bool value)
    {
        bulbColorController.enabled = value;
        // Do NOT enable the canvas based on BulbColorController directly
        if (value) return;
        // If we are disabling, make sure canvas is hidden too
        if (canvas != null)
            canvas.SetActive(false);
    }

    #endregion
}