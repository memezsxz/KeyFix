using System;
using System.Collections.Generic;
using System.Linq;
using Code.Scripts.Obstacles;
using UnityEngine;

public class Corridor : MonoBehaviour
{
    [Header("Corridor Elements")] 
    private List<Lazar> lazars = new List<Lazar>();
    private List<WindBlower> windBlowers = new List<WindBlower>();

    // public event Action<Corridor> OnCorridorCompleted;

    private void Start()
    {
        lazars = GetComponentsInChildren<Lazar>(includeInactive: true).ToList();
        windBlowers = GetComponentsInChildren<WindBlower>(includeInactive: true).ToList();
        // DeactivateCorridor();
    }

    private void UpdateObsticals()
    {
        lazars = GetComponentsInChildren<Lazar>(includeInactive: true).ToList();
        windBlowers = GetComponentsInChildren<WindBlower>(includeInactive: true).ToList();
    }

    public void ActivateCorridor()
    {
        UpdateObsticals();
        // print(lazars.Count);
        foreach (var lazer in lazars)
            if (lazer != null)
                lazer.gameObject.SetActive(true);
        // print(windBlowers.Count);

        foreach (var blower in windBlowers)
            if (blower != null)
                blower.gameObject.SetActive(true);
    }

    public void DeactivateCorridor()
    {
        UpdateObsticals();
        // print("deactivated");
        foreach (var lazer in lazars)
            if (lazer != null)
                lazer.gameObject.SetActive(false);

        foreach (var blower in windBlowers)
            if (blower != null)
                blower.gameObject.SetActive(false);
    }

    // public void CompleteCorridor()
    // {
    //     DeactivateCorridor();
    //     OnCorridorCompleted?.Invoke(this);
    // }
}