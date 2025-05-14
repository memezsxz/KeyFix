using UnityEngine;

/// <summary>
/// Handles interaction logic for buttons in the space corridor puzzle.
/// Controls activation and deactivation of corridor sections depending on button type and interaction sequence.
/// </summary>
public class SpaceButtonInteraction : InteractableBase
{
    #region Serialized Fields

    /// <summary>
    /// Indicates if this is the first button in the sequence.
    /// </summary>
    [SerializeField] private bool isFirstButton;

    /// <summary>
    /// Indicates if this is the last button in the sequence.
    /// </summary>
    [SerializeField] private bool isLastButton;

    /// <summary>
    /// Material to use when the button is in its inactive (red) state.
    /// </summary>
    [SerializeField] private Material redMaterial;

    /// <summary>
    /// Material to use when the button is in the ready (green) state.
    /// </summary>
    [SerializeField] private Material greenMaterial;

    /// <summary>
    /// Material to use when the button has been used and disabled (gray).
    /// </summary>
    [SerializeField] private Material grayMaterial;

    #endregion

    #region Private Fields

    /// <summary>
    /// Tracks whether the button has already been clicked once.
    /// Used for mid-sequence buttons.
    /// </summary>
    private bool hasClickedOnce;

    /// <summary>
    /// Indicates whether this button can currently be interacted with.
    /// </summary>
    private bool isInteractable = true;

    /// <summary>
    /// The mesh renderer used to visually update button state.
    /// </summary>
    private MeshRenderer meshRenderer;

    #endregion

    #region Unity Methods

    private void Start()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();

        // First button starts green, others red
        if (isFirstButton)
            SetGreen();
        else
            SetRed();
    }

    #endregion

    #region Interaction Logic

    /// <summary>
    /// Handles the interaction logic based on whether this is a first, last, or mid-sequence button.
    /// </summary>
    public override void Interact()
    {
        if (!isInteractable) return;

        if (isFirstButton)
        {
            // First button simply activates the first corridor
            SpaceManager.Instance.ActivateNextCorridor();
            SetGray();
            isInteractable = false;
        }
        else if (isLastButton)
        {
            // Last button deactivates current and activates final corridor
            SpaceManager.Instance.DeactivateCurrentCorridor();
            SpaceManager.Instance.ActivateNextCorridor();
            SetGray();
            isInteractable = false;
        }
        else
        {
            if (!hasClickedOnce)
            {
                // First click: Deactivate current corridor
                SpaceManager.Instance.DeactivateCurrentCorridor();
                SetGreen();
                hasClickedOnce = true;
            }
            else
            {
                // Second click: Activate next corridor and lock the button
                SpaceManager.Instance.ActivateNextCorridor();
                SetGray();
                isInteractable = false;
            }
        }
    }

    #endregion

    #region Material State Methods

    /// <summary>
    /// Sets the button material to red (inactive).
    /// </summary>
    private void SetRed()
    {
        if (meshRenderer != null)
            meshRenderer.material = redMaterial;
    }

    /// <summary>
    /// Sets the button material to green (ready).
    /// </summary>
    private void SetGreen()
    {
        if (meshRenderer != null)
            meshRenderer.material = greenMaterial;
    }

    /// <summary>
    /// Sets the button material to gray and disables further interaction.
    /// </summary>
    public void SetGray()
    {
        if (meshRenderer != null)
            meshRenderer.material = grayMaterial;

        ShowHint(false);
        isInteractable = false;
        gameObject.layer = LayerMask.NameToLayer("Default");
    }

    #endregion
}
