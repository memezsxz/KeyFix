using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraEnemyLock : MonoBehaviour
{
    [SerializeField] private CinemachineTargetGroup targetGroup;
    [SerializeField] private float enemyWeight = 1f;
    [SerializeField] private float playerWeight = 2f;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float maxDistance = 10f;

    private void Start()
    {
        // playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void OnEnable()
    {
        BuildTargetGroup();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void BuildTargetGroup()
    {
        if (!targetGroup || !playerTransform) return;

        var enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // adding the player
        List<CinemachineTargetGroup.Target> targets = new List<CinemachineTargetGroup.Target>
        {
            new CinemachineTargetGroup.Target
            {
                target = playerTransform,
                weight = playerWeight,
                radius = 0.5f
            }
        };

        // adding the sounding enemies 
        foreach (var enemy in enemies)
        {
            if (Vector3.Distance(playerTransform.position, enemy.transform.position) > maxDistance) continue;

            targets.Add(new CinemachineTargetGroup.Target
            {
                target = enemy.transform,
                weight = enemyWeight,
                radius = 0.5f
            });
        }

        targetGroup.m_Targets = targets.ToArray();
        // Debug.Log(targets.Count);
    }
    
    private void Update()
    {
        // Optional: continuously update enemies in range
        BuildTargetGroup();
    }
}
