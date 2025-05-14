using System.Collections.Generic;
using UnityEngine;

public class SpaceManager : Singleton<SpaceManager>
{
    [Header("Corridors in order")] [SerializeField]
    private List<Corridor> corridors = new();

    [Header("Buttons in order")] [SerializeField]
    private List<SpaceButtonInteraction> spaceButtons = new();

    [SerializeField] private Collider shrinker;
    [SerializeField] private Collider stretcher;

    // [SerializeField] private RoomToHallway doorInteract;

    // [SerializeField] private GameObject door;
    private int currentCorridorIndex = -1;
    private bool isCorridorActive;

    public bool DidWin { get; private set; }

    private void Start()
    {
        for (var i = 0; i < spaceButtons.Count; i++)
            if (spaceButtons[i] != null)
                spaceButtons[i].gameObject.SetActive(i == 0); // Only the first button active

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

        // print($"Active corridor: {currentCorridorIndex}. count is {corridors.Count}, it is {currentCorridorIndex >= corridors.Count}");

        if (currentCorridorIndex >= corridors.Count - 1)
        {
            var lastButtonIndex = Mathf.Min(currentCorridorIndex, spaceButtons.Count - 1);
            var lastButton = spaceButtons[lastButtonIndex];
            if (lastButton != null) lastButton.SetGray();

            GameManager.Instance.ChangeState(GameManager.GameState.Victory);

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
            if (corridor != null)
            {
                corridor.DeactivateCorridor();
                corridor.gameObject.SetActive(false);
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