using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI interactText;
    [SerializeField] float interactionRadius = 10f;
    [SerializeField] LayerMask interactableLayer;

    IInteractable currentTargetedInteractable;

    void Update()
    {
        UpdateCurrentInteractable();
        UpdateInteractionText();
        CheckForInteractionInput();
    }

    void UpdateCurrentInteractable()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactionRadius, interactableLayer);

        currentTargetedInteractable = null;

        foreach (var hit in hits)
        {
            IInteractable interactable = hit.GetComponent<IInteractable>();
            if (interactable != null)
            {
                currentTargetedInteractable = interactable;
                break;
            }
        }
    }

    void UpdateInteractionText()
    {
        // Hide all hints first
        foreach (var btn in FindObjectsOfType<SpawnButton>())
        {
            btn.ShowHint(false);
        }

        if (currentTargetedInteractable is SpawnButton sb)
        {
            sb.ShowHint(true);
        }
    }


    void CheckForInteractionInput()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame && currentTargetedInteractable != null)
        {
            currentTargetedInteractable.Interact();
        }
    }
}
