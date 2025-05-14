using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Manages switching control between two player characters: Robot and Robota.
/// </summary>
public class PlayerManager : MonoBehaviour
{
    #region Serialized Fields

    /// <summary>
    /// Reference to the Robot character's PlayerMovement script.
    /// </summary>
    [SerializeField] private PlayerMovement robotPlayer;

    /// <summary>
    /// Reference to the Robota character's PlayerMovement script.
    /// </summary>
    [SerializeField] private PlayerMovement robotaPlayer;

    #endregion

    #region Private Fields

    /// <summary>
    /// The player character currently under player control.
    /// </summary>
    private PlayerMovement activePlayer;

    /// <summary>
    /// Input action used to trigger switching between players.
    /// </summary>
    private InputAction switchAction;

    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        // Initialize with robot as the active player
        activePlayer = robotPlayer;
        SetPlayerControl(robotPlayer, true);
        SetPlayerControl(robotaPlayer, false);

        var actions = GetComponent<PlayerInput>().actions;
        switchAction = actions.FindAction("Interact"); // Assuming "E" is bound to "Interact"

        switchAction.performed += ctx => SwitchPlayer();
    }

    #endregion

    #region Switching Logic

    /// <summary>
    /// Toggles control between Robot and Robota characters.
    /// </summary>
    private void SwitchPlayer()
    {
        if (activePlayer == robotPlayer)
        {
            SetPlayerControl(robotPlayer, false);
            SetPlayerControl(robotaPlayer, true);
            activePlayer = robotaPlayer;
        }
        else
        {
            SetPlayerControl(robotaPlayer, false);
            SetPlayerControl(robotPlayer, true);
            activePlayer = robotPlayer;
        }
    }

    /// <summary>
    /// Enables or disables movement, input, and animation for the given player.
    /// </summary>
    /// <param name="player">The PlayerMovement script reference.</param>
    /// <param name="enable">Whether to enable or disable control.</param>
    private void SetPlayerControl(PlayerMovement player, bool enable)
    {
        player.ToggleMovement(enable);
        player.GetComponent<PlayerInput>().enabled = enable;
        player.GetComponent<CharacterController>().enabled = enable;
        player.GetComponent<Animator>().enabled = enable;
        player.enabled = enable;
    }

    #endregion
}