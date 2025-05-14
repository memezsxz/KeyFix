using UnityEngine;

/// <summary>
/// Abstract base class for all interactable objects in the game.
/// Provides shared functionality for displaying interaction hints and handling basic interaction flow.
/// </summary>
public abstract class InteractableBase : MonoBehaviour
{
    #region Serialized Fields

    /// <summary>
    /// Reference to the UI component that displays hints to the player.
    /// </summary>
    [SerializeField] protected HintUI hintUI;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the message displayed by the interaction hint UI.
    /// Returns an empty string if <see cref="hintUI"/> is not assigned.
    /// </summary>
    public string InteractMessage
    {
        get => hintUI == null ? "" : hintUI.message;
        protected set => hintUI.message = value;
    }

    #endregion

    #region Abstract Methods

    /// <summary>
    /// Triggers the primary interaction behavior for this object on the E key click.
    /// Must be implemented by derived classes.
    /// </summary>
    public abstract void Interact();

    #endregion

    #region Virtual Methods

    /// <summary>
    /// Optional extended interaction method. Can be overridden by child classes if needed.
    /// </summary>
    public virtual void InteractH()
    {
        // Optional hook
    }

    /// <summary>
    /// Shows or hides the hint UI for this interactable.
    /// </summary>
    /// <param name="show">Whether to show or hide the hint.</param>
    public virtual void ShowHint(bool show)
    {
        if (hintUI == null) return;

        if (show)
            hintUI.ShowHint();
        else
            hintUI.HideHint();
    }

    #endregion
}