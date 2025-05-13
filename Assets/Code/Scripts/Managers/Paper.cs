using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Paper : MonoBehaviour
{
    [SerializeField] private List<Material> color;
    private readonly float fallSpeed = 1.5f; // standard gravity
    private bool hasLanded;

    private Rigidbody rb;
    private readonly float torqueForce = 0.1f;

    private void Start()
    {
        GetComponent<MeshRenderer>().material = color[Random.Range(0, color.Count)];

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // Disable Unity gravity
        rb.mass = 0.01f;
        rb.drag = 1f;
        rb.angularDrag = 2f;

        // Only add some torque (spin)
        var torque = new Vector3(
            Random.Range(-torqueForce, torqueForce),
            Random.Range(-torqueForce, torqueForce),
            Random.Range(-torqueForce, torqueForce)
        );
        rb.AddTorque(torque, ForceMode.Impulse);
    }

    private void Update()
    {
        if (!hasLanded) rb.AddForce(Vector3.down * fallSpeed, ForceMode.Acceleration);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Only trigger if we hit something beneath us
        foreach (var contact in collision.contacts)
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.5f) // hit from below
            {
                hasLanded = true;
                rb.useGravity = true;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                enabled = false;
                break;
            }
    }
}