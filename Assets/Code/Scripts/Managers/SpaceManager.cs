using System.Collections.Generic;
using UnityEngine;

public class SpaceManager : MonoBehaviour
{
    [Header("Corridors in order")] [SerializeField]
    private List<Corridor> corridors = new List<Corridor>();

    private int currentCorridorIndex = -1;

    private void Start()
    {
        
        corridors.ForEach(c =>
        {
            if (c != null) c.OnCorridorCompleted += HandleCorridorCompleted;
        });

        DeactivateAllCorridors();
    }

    private void HandleCorridorCompleted(Corridor corridor)
    {
        Debug.Log($"Corridor completed: {corridor.name}");
        ActivateNextCorridor();
    }

    public void ActivateNextCorridor()
    {
        currentCorridorIndex++;

        if (currentCorridorIndex >= corridors.Count)
        {
            Debug.Log("All corridors completed!");
            return;
        }

        DeactivateAllCorridors();

        Corridor nextCorridor = corridors[currentCorridorIndex];
        if (nextCorridor != null)
        {
            nextCorridor.gameObject.SetActive(true);
            nextCorridor.ActivateCorridor();
        }
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
    }

    public void ResetSpace()
    {
        currentCorridorIndex = -1;
        DeactivateAllCorridors();
    }
}