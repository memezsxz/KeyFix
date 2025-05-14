using UnityEngine;

/// <summary>
/// An interactable power lever that toggles between 'up' and 'down' states.
/// Plays animations and can trigger victory when toggled to the down position.
/// </summary>
public class PowerLever : InteractableBase
{
    #region Constants

    /// <summary>
    /// Animator parameter ID for toggling the power state.
    /// </summary>
    private static readonly int IsPowerOn = Animator.StringToHash("is_power_on");

    #endregion

    #region Private Fields

    /// <summary>
    /// Reference to the Animator component controlling lever animations.
    /// </summary>
    private Animator animator;

    /// <summary>
    /// Tracks the current state of the lever.
    /// </summary>
    private LeverState leverState = LeverState.down;

    #endregion

    #region Unity Methods

    private void Start()
    {
        animator = GetComponent<Animator>();
        InteractMessage = "E"; // Set the interaction prompt
    }

    #endregion

    #region Interaction Logic

    /// <summary>
    /// Called when the player interacts with the lever.
    /// </summary>
    public override void Interact()
    {
        ToggleState();
    }

    /// <summary>
    /// Toggles the lever between up and down states.
    /// Temporarily sets the state to 'freeze' while changing to prevent double interactions.
    /// If toggled to down, victory is triggered after a delay.
    /// </summary>
    private void ToggleState()
    {
        // Set state to freeze while processing the toggle
        if (leverState == LeverState.down)
        {
            leverState = LeverState.freeze;
            SetAnimatorState();
            leverState = LeverState.up;
        }
        else if (leverState == LeverState.up)
        {
            leverState = LeverState.freeze;
            SetAnimatorState();
            leverState = LeverState.down;
        }

        // Apply the final state to the Animator
        SetAnimatorState();

        // Optional win condition: if lever goes down, trigger victory
        if (leverState == LeverState.down)
            Invoke(nameof(TriggerVictory), 2f);
    }

    /// <summary>
    /// Triggers the victory state in the game after the lever is pulled down.
    /// </summary>
    private void TriggerVictory()
    {
        GameManager.Instance.ChangeState(GameManager.GameState.Victory);
    }

    /// <summary>
    /// Updates the Animator with the current lever state.
    /// </summary>
    private void SetAnimatorState()
    {
        animator.SetInteger(IsPowerOn, (int)leverState);
    }

    #endregion

    #region Lever State

    /// <summary>
    /// Possible states for the power lever.
    /// </summary>
    private enum LeverState
    {
        freeze = -1,
        down = 0,
        up = 1
    }

    #endregion
}
