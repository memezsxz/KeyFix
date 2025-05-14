using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

/// <summary>
/// Dynamically builds a target group for Cinemachine based on nearby enemies and the player.
/// </summary>
public class CameraEnemyLock : MonoBehaviour
{
    #region Serialized Fields

    /// <summary>
    /// The Cinemachine target group to update with player and nearby enemies.
    /// </summary>
    [SerializeField] private CinemachineTargetGroup targetGroup;

    /// <summary>
    /// The weight given to each enemy in the target group.
    /// </summary>
    [SerializeField] private float enemyWeight = 1f;

    /// <summary>
    /// The weight given to the player in the target group.
    /// </summary>
    [SerializeField] private float playerWeight = 2f;

    /// <summary>
    /// The player's transform. Will be auto-assigned if not set.
    /// </summary>
    [SerializeField] private Transform playerTransform;

    /// <summary>
    /// Maximum distance from the player to include an enemy in the target group.
    /// </summary>
    [SerializeField] private float maxDistance = 10f;

    #endregion

    #region Unity Methods

    private void Start()
    {
        // Automatically find the player if not assigned
        if (!playerTransform)
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        // Continuously update the target group every frame (optional for dynamic enemy movement)
        BuildTargetGroup();
    }

    private void OnEnable()
    {
        // Ensure target group is built when object is enabled
        BuildTargetGroup();

        // Refresh player reference in case it was re-instantiated
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    #endregion

    #region Target Group Logic

    /// <summary>
    /// Rebuilds the CinemachineTargetGroup with the player and nearby enemies.
    /// </summary>
    public void BuildTargetGroup()
    {
        if (!targetGroup || !playerTransform) return;

        var enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // Start with the player as the primary target
        var targets = new List<CinemachineTargetGroup.Target>
        {
            new()
            {
                target = playerTransform,
                weight = playerWeight,
                radius = 0.5f
            }
        };

        // Add all enemies within maxDistance to the target group
        foreach (var enemy in enemies)
        {
            if (Vector3.Distance(playerTransform.position, enemy.transform.position) > maxDistance)
                continue;

            targets.Add(new CinemachineTargetGroup.Target
            {
                target = enemy.transform,
                weight = enemyWeight,
                radius = 0.5f
            });
        }

        // Update the target group
        targetGroup.m_Targets = targets.ToArray();
    }

    #endregion
}
