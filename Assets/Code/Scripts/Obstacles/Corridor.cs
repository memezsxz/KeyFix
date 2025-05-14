using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Represents a single corridor challenge area containing lazars and wind blowers.
/// Can be activated or deactivated as part of a progression system.
/// </summary>
public class Corridor : MonoBehaviour
{
    /// <summary>
    /// Lazars inside this corridor, dynamically collected from children.
    /// </summary>
    [Header("Corridor Elements")] private List<Lazar> lazars = new();

    /// <summary>
    /// Wind blowers inside this corridor, dynamically collected from children.
    /// </summary>
    private List<WindBlower> windBlowers = new();

    private void Start()
    {
        lazars = GetComponentsInChildren<Lazar>(true).ToList();
        windBlowers = GetComponentsInChildren<WindBlower>(true).ToList();
    }

    /// <summary>
    /// Refreshes the list of lazars and wind blowers by re-scanning the corridor's children.
    /// </summary>
    private void UpdateObsticals()
    {
        lazars = GetComponentsInChildren<Lazar>(true).ToList();
        windBlowers = GetComponentsInChildren<WindBlower>(true).ToList();
    }

    /// <summary>
    /// Enables all lazars and wind blowers in this corridor.
    /// </summary>
    public void ActivateCorridor()
    {
        UpdateObsticals();

        // Activate each lazar
        foreach (var lazer in lazars)
        {
            if (lazer != null)
                lazer.gameObject.SetActive(true);
        }

        // Activate each wind blower
        foreach (var blower in windBlowers)
        {
            if (blower != null)
                blower.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Disables all lazars and wind blowers in this corridor.
    /// </summary>
    public void DeactivateCorridor()
    {
        UpdateObsticals();

        // Deactivate each lazar
        foreach (var lazer in lazars)
        {
            if (lazer != null)
                lazer.gameObject.SetActive(false);
        }

        // Deactivate each wind blower
        foreach (var blower in windBlowers)
        {
            if (blower != null)
                blower.gameObject.SetActive(false);
        }
    }
}