using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Detects and manages interaction with nearby interactable objects.
/// Responsible for proximity checks, displaying hint UIs, and invoking interaction methods.
/// Attach this script to the player.
/// </summary>
public class InteractionController : MonoBehaviour
{
    #region Serialized Fields

    /// <summary>
    /// The radius around the player used to search for interactable objects.
    /// </summary>
    [SerializeField] private float interactionRadius = 1f;

    #endregion

    #region Private Fields

    /// <summary>
    /// The current interactable object the player is near.
    /// </summary>
    private InteractableBase currentTargetedInteractable;

    /// <summary>
    /// Layer mask used to filter only objects marked as interactables.
    /// </summary>
    private LayerMask interactableLayer;

    #endregion

    #region Unity Methods

    private void Start()
    {
        // Define the interactable layer to limit physics checks
        interactableLayer = LayerMask.GetMask("Interactable");
    }

    private void Update()
    {
        UpdateCurrentInteractable(); // Find the closest interactable
        UpdateInteractionText(); // Toggle hint UI display
        CheckForInteractionInput(); // Handle keyboard input
    }

    #endregion

    #region Detection & Hint Display

    /// <summary>
    /// Checks for interactable objects within the interaction radius and selects the first one found.
    /// </summary>
    private void UpdateCurrentInteractable()
    {
        // get objects with in the spicified radios
        var hits = Physics.OverlapSphere(transform.position, interactionRadius, interactableLayer);

        // detach the current interactable
        currentTargetedInteractable = null;

        // loop over the list and get the closest interactable
        foreach (var hit in hits)
        {
            var interactable = hit.GetComponent<InteractableBase>();
            if (interactable != null)
            {
                currentTargetedInteractable = interactable;
                break;
            }
        }
    }

    /// <summary>
    /// Updates hint UI visibility for interactables in the scene.
    /// Only the currently targeted object will show its hint.
    /// </summary>
    private void UpdateInteractionText()
    {
        // First hide all hints
        foreach (var btn in FindObjectsOfType<InteractableBase>())
            btn.ShowHint(false);

        // Then show the hint for the current interactable, if one is found
        if (currentTargetedInteractable is InteractableBase sb)
            sb.ShowHint(true);
    }

    #endregion

    #region Input Handling

    /// <summary>
    /// Checks for interaction key inputs and invokes interaction methods on the current interactable.
    /// </summary>
    private void CheckForInteractionInput()
    {
        // on e key
        if (Keyboard.current.eKey.wasPressedThisFrame && currentTargetedInteractable != null)
            currentTargetedInteractable.Interact();

        // on h key
        if (Keyboard.current.hKey.wasPressedThisFrame && currentTargetedInteractable != null)
            currentTargetedInteractable.InteractH();
    }

    #endregion
}