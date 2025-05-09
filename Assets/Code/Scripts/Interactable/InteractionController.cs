using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionController : MonoBehaviour
{
    // [SerializeField] TextMeshProUGUI interactText;
    [SerializeField] float interactionRadius = 1f;
    LayerMask interactableLayer;

    InteractableBase currentTargetedInteractable;

    private void Start()
    {
        interactableLayer = LayerMask.GetMask("Interactable");
    }

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
            InteractableBase interactable = hit.GetComponent<InteractableBase>();
            if (interactable != null)
            {
                // print("found interactable");
                currentTargetedInteractable = interactable;
                break;
            }
        }
    }

    void UpdateInteractionText()
    {
        // Hide all hints first
        foreach (var btn in FindObjectsOfType<InteractableBase>())
        {
            btn.ShowHint(false);
        }

        if (currentTargetedInteractable is InteractableBase sb)
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

        if (Keyboard.current.hKey.wasPressedThisFrame && currentTargetedInteractable != null)
        {
            currentTargetedInteractable.InteractH();
        }
    }
}