using System;
using System.Collections.Generic;
using System.Linq;
using Code.Scripts.Obstacles;
using UnityEngine;
using UnityEngine.Serialization;

public class Corridor : MonoBehaviour
{
    [Header("Corridor Elements")] [SerializeField]
    private List<Lazar> lazars = new List<Lazar>();

    [SerializeField] private List<WindBlower> windBlowers = new List<WindBlower>();

    private bool isDone = false;
    public event System.Action<Corridor> OnCorridorCompleted;


    private void Start()
    {
        lazars.Clear();
        windBlowers.Clear();

        lazars = GetComponentsInChildren<Lazar>(includeInactive: true).ToList();
        windBlowers = GetComponentsInChildren<WindBlower>(includeInactive: true).ToList();
        DeactivateCorridor();
    }

    public void ActivateCorridor()
    {
        foreach (var lazer in lazars)
        {
            if (lazer != null)
                lazer.gameObject.SetActive(true);
        }

        foreach (var blower in windBlowers)
        {
            if (blower != null)
                blower.gameObject.SetActive(true);
        }
    }

    public void DeactivateCorridor()
    {
        foreach (var lazar in lazars)
        {
            if (lazar != null)
                lazar.gameObject.SetActive(false);
        }

        foreach (var blower in windBlowers)
        {
            if (blower != null)
                blower.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isDone && other.CompareTag("Player"))
        {
            ActivateCorridor();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isDone = true;
            DeactivateCorridor();
            OnCorridorCompleted?.Invoke(this);
        }
    }
}