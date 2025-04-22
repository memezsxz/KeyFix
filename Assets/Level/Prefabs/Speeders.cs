using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speeders : MonoBehaviour
{
    [SerializeField] private GameObject model;

    // [SerializeField] private float speed ;
    [SerializeField] private Transform endposition;

    private Vector3 endpoint;
    // Start is called before the first frame update
    void Start()
    {
        endpoint = endposition.position;
        gameObject.transform.localScale = model.transform.localScale;
    }
    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<SpeedersInteraction>();
        if (player == null) return;

        Vector3 toPlayer = (player.transform.position - transform.position).normalized;
        float alignment = Vector3.Dot(transform.forward, toPlayer);

        if (alignment > 0.3f)
        {
            // Entered from the back (wrong direction) → push just a little
            Vector3 minimalPushTarget = player.transform.position + transform.forward * 0.4f;
            player.TeleportTo(minimalPushTarget); // Not MoveTowards — no freeze or delay
        }
        else
        {
            // Normal forward entry → full push
            Vector3 pushTarget = player.transform.position + transform.forward * model.transform.localScale.z;
            player.MoveTowards(pushTarget); // Smooth full push
        }
    }
}