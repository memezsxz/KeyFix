﻿using UnityEngine;

public class ExplodableMonitor : MonoBehaviour
{
    [SerializeField] private GameObject screenExplosionParticleSystem;

    [SerializeField] private GameObject screenOff;

    [SerializeField] private GameObject screenOn;

    [SerializeField] private GameObject shards;

    private bool broken;

    // Start is called before the first frame update
    private void Start()
    {
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "bullet" && !broken)
        {
            broken = true;
            screenOff.SetActive(false);
            screenOn.SetActive(false);
            shards.SetActive(true);
            var shardRBs = GetComponentsInChildren<Rigidbody>();
            screenExplosionParticleSystem.SetActive(true);
            foreach (var shardRB in shardRBs)
            {
                float randomForce = Random.Range(1, 5);
                float randomRotationX = Random.Range(-20, 20);
                float randomRotationY = Random.Range(-20, 20);
                float randomRotationZ = Random.Range(-20, 20);
                shardRB.transform.Rotate(randomRotationX, randomRotationY, randomRotationZ);
                shardRB.AddRelativeForce(Vector3.forward * randomForce, ForceMode.Impulse);
            }
        }
    }
}