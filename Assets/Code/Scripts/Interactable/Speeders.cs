using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speeders : MonoBehaviour
{
    [SerializeField] private GameObject model;

    [SerializeField] private Transform endposition;
    [SerializeField] private Material activeMaterial;
    [SerializeField] private Material inactiveMaterial;
    [SerializeField] private MeshRenderer meshRenderer;
    // public event Action OnPushEnd;

    private Vector3 endpoint;

    // Start is called before the first frame update
    void Start()
    {
        endpoint = endposition.position;
        gameObject.transform.localScale = model.transform.localScale;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;


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
            meshRenderer.material = activeMaterial;
            Vector3 pushTarget = player.transform.position + transform.forward * model.transform.localScale.z;
            player.MoveTowards(pushTarget); // Smooth full push
            StartCoroutine(DelayedReset());
        }

    }

    // private void OnTriggerExit(Collider other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         meshRenderer.material = inactiveMaterial;
    //     }
    // }

    private IEnumerator DelayedReset()
    {
        yield return new WaitForSeconds(0.3f); // tiny delay
        meshRenderer.material = inactiveMaterial;
    }

    private IEnumerator WaitUntilPushEnds(SpeedersInteraction mover)
    {
        while (mover != null && mover.IsBeingPushed)
        {
            yield return null;
        }

        meshRenderer.material = inactiveMaterial;
    }
}