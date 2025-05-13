using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private PlayerMovement robotPlayer;
    [SerializeField] private PlayerMovement robotaPlayer;

    private PlayerMovement activePlayer;
    private InputAction switchAction;

    private void Start()
    {
        // Start with robot active
        activePlayer = robotPlayer;
        SetPlayerControl(robotPlayer, true);
        SetPlayerControl(robotaPlayer, false);

        var actions = GetComponent<PlayerInput>().actions;
        switchAction = actions.FindAction("Interact"); // Assuming "E" is bound to "Interact"

        switchAction.performed += ctx => SwitchPlayer();
    }

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

    private void SetPlayerControl(PlayerMovement player, bool enable)
    {
        player.ToggleMovement(enable);
        player.GetComponent<PlayerInput>().enabled = enable;
        player.GetComponent<CharacterController>().enabled = enable;
        player.GetComponent<Animator>().enabled = enable;
        player.enabled = enable;
    }
}