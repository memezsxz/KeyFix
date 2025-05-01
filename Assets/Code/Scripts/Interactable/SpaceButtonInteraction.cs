using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceButtonInteraction : InteractableBase
{
    private MeshRenderer meshRenderer;
    private bool isInteractable = true;
    private bool hasClickedOnce = false;

    [SerializeField] private bool isFirstButton = false;
    [SerializeField] private bool isLastButton = false;

    [SerializeField] private Material redMaterial;
    [SerializeField] private Material greenMaterial;
    [SerializeField] private Material grayMaterial;

    private void Start()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();

        if (isFirstButton)
            SetGreen();
        else
            SetRed();
    }

    public override void Interact()
    {
        if (!isInteractable) return;

        if (isFirstButton)
        {
            SpaceManager.Instance.ActivateNextCorridor();
            SetGray();
            isInteractable = false;
        }
        else if (isLastButton)
        {
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
                // Second click: Activate next corridor
                SpaceManager.Instance.ActivateNextCorridor();
                SetGray();
                isInteractable = false;
            }
        }
    }


    private void SetRed()
    {
        if (meshRenderer != null)
            meshRenderer.material = redMaterial;
    }

    private void SetGreen()
    {
        if (meshRenderer != null)
            meshRenderer.material = greenMaterial;
    }

    public void SetGray()
    {
        if (meshRenderer != null) meshRenderer.material = grayMaterial;
        ShowHint(false);
        isInteractable = false;
        gameObject.layer = LayerMask.NameToLayer("Default");
    }
}