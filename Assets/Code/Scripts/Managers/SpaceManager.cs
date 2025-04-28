using System.Collections.Generic;
using Code.Scripts.Interactable;
using Code.Scripts.Managers;
using UnityEngine;

public class SpaceManager : Singleton<SpaceManager>
{
    [Header("Corridors in order")] [SerializeField]
    private List<Corridor> corridors = new List<Corridor>();

    [SerializeField] private Collider shrinker;
    [SerializeField] private Collider stretcher;

    private int currentCorridorIndex = -1;
    private bool isCorridorActive = false;

    private void Start()
    {
        // corridors.ForEach(c =>
        // {
        //     if (c != null) c.OnCorridorCompleted += HandleCorridorCompleted;
        // });

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
            GameManager.Instance.ChangeState(GameManager.GameState.Victory);
            return;
        }

        var nextCorridor = corridors[currentCorridorIndex];
        if (nextCorridor != null)
        {
            nextCorridor.gameObject.SetActive(true);
            nextCorridor.ActivateCorridor();
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