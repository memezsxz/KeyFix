using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionController : MonoBehaviour
{
    // [SerializeField] TextMeshProUGUI interactText;
    [SerializeField] private float interactionRadius = 1f;

    private InteractableBase currentTargetedInteractable;
    private LayerMask interactableLayer;

    private void Start()
    {
        interactableLayer = LayerMask.GetMask("Interactable");
    }

    private void Update()
    {
        UpdateCurrentInteractable();
        UpdateInteractionText();
        CheckForInteractionInput();
    }

    private void UpdateCurrentInteractable()
    {
        var hits = Physics.OverlapSphere(transform.position, interactionRadius, interactableLayer);

        currentTargetedInteractable = null;

        foreach (var hit in hits)
        {
            var interactable = hit.GetComponent<InteractableBase>();
            if (interactable != null)
            {
                // print("found interactable");
                currentTargetedInteractable = interactable;
                break;
            }
        }
    }

    private void UpdateInteractionText()
    {
        // Hide all hints first
        foreach (var btn in FindObjectsOfType<InteractableBase>()) btn.ShowHint(false);

        if (currentTargetedInteractable is InteractableBase sb) sb.ShowHint(true);
    }


    private void CheckForInteractionInput()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame && currentTargetedInteractable != null)
            currentTargetedInteractable.Interact();

        if (Keyboard.current.hKey.wasPressedThisFrame && currentTargetedInteractable != null)
            currentTargetedInteractable.InteractH();
    }
}