using System.Collections.Generic;
using Code.Scripts.Interactable;
using Code.Scripts.Managers;
using UnityEngine;

public class SpaceManager : Singleton<SpaceManager>
{
    [Header("Corridors in order")] [SerializeField]
    private List<Corridor> corridors = new List<Corridor>();

    [Header("Buttons in order")] [SerializeField]
    private List<SpaceButtonInteraction> spaceButtons = new List<SpaceButtonInteraction>();

    [SerializeField] private Collider shrinker;
    [SerializeField] private Collider stretcher;

    [SerializeField] private DoorInteract doorInteract;
    private int currentCorridorIndex = -1;
    private bool isCorridorActive = false;

    public bool DidWin { get; private set; }

    [SerializeField] private GameObject door;

    private void Start()
    {
        for (int i = 0; i < spaceButtons.Count; i++)
        {
            if (spaceButtons[i] != null)
            {
                spaceButtons[i].gameObject.SetActive(i == 0); // Only the first button active
            }
        }

        DeactivateAllCorridors();
    }
 
    public void DeactivateCurrentCorridor()
    {
        if (currentCorridorIndex < 0 || currentCorridorIndex >= corridors.Count)
            return;

        var currentCorridor = corridors[currentCorridorIndex];
        if (currentCorridor != null)
        {
            currentCorridor.DeactivateCorridor();
            currentCorridor.gameObject.SetActive(false);
        }

        isCorridorActive = false;
        ToggleScaler();
    }

    public void ActivateNextCorridor()
    {
        if (isCorridorActive)
        {
            Debug.LogWarning("Cannot activate next corridor: current corridor still active. Deactivate first.");
            return;
        }

        currentCorridorIndex++;

        if (currentCorridorIndex >= corridors.Count)
        {
            var lastButtonIndex = Mathf.Min(currentCorridorIndex, spaceButtons.Count - 1);
            var lastButton = spaceButtons[lastButtonIndex];
            if (lastButton != null)
            {
                lastButton.SetGray();
            }
            door.gameObject.layer = LayerMask.NameToLayer("Interactable");
            return;
        }

        var nextCorridor = corridors[currentCorridorIndex];
        if (nextCorridor != null)
        {
            nextCorridor.gameObject.SetActive(true);
            nextCorridor.ActivateCorridor();
        }

        var nextButton = spaceButtons[(currentCorridorIndex + 1) % spaceButtons.Count];
        if (nextButton != null)
        {
            print("activated next button " + nextButton.gameObject.name);
            nextButton.gameObject.SetActive(true);
        }

        isCorridorActive = true;
        ToggleScaler(); // Always lock immediately after corridor becomes active
    }

    public void DeactivateAllCorridors()
    {
        foreach (var corridor in corridors)
        {
            if (corridor != null)
            {
                corridor.DeactivateCorridor();
                corridor.gameObject.SetActive(false);
            }
        }

        isCorridorActive = false;
        ToggleScaler();
    }

    public void ResetSpace()
    {
        currentCorridorIndex = -1;
        DeactivateAllCorridors();
    }

    private void ToggleScaler()
    {
        if (shrinker != null) shrinker.enabled = !isCorridorActive;
        if (stretcher != null) stretcher.enabled = !isCorridorActive;
    }
}