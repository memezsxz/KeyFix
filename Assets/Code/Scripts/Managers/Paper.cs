using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simulates a light floating paper with randomized color and rotational torque.
/// The paper falls manually and stops on collision with the ground.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Paper : MonoBehaviour
{
    /// <summary>
    /// List of material color options to randomly assign at spawn.
    /// </summary>
    [SerializeField] private List<Material> color;

    /// <summary>
    /// Downward force applied to simulate falling.
    /// </summary>
    private readonly float fallSpeed = 1.5f;

    /// <summary>
    /// Indicates whether the paper has touched a surface and settled.
    /// </summary>
    private bool hasLanded;

    /// <summary>
    /// Reference to the Rigidbody used for applying force and torque.
    /// </summary>
    private Rigidbody rb;

    /// <summary>
    /// Multiplier used to control random torque at spawn.
    /// </summary>
    private readonly float torqueForce = 0.1f;

    private void Start()
    {
        // Randomly assign a color from the list
        GetComponent<MeshRenderer>().material = color[Random.Range(0, color.Count)];

        // Configure Rigidbody for custom gravity
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // Disable Unity gravity
        rb.mass = 0.01f;
        rb.drag = 1f;
        rb.angularDrag = 2f;

        // Apply a small random torque (spin)
        var torque = new Vector3(
            Random.Range(-torqueForce, torqueForce),
            Random.Range(-torqueForce, torqueForce),
            Random.Range(-torqueForce, torqueForce)
        );
        rb.AddTorque(torque, ForceMode.Impulse);
    }

    private void Update()
    {
        // Manually apply downward force while floating
        if (!hasLanded) rb.AddForce(Vector3.down * fallSpeed, ForceMode.Acceleration);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check all contact points to see if the object landed on something below it
        foreach (var contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.5f)
            {
                hasLanded = true;
                rb.useGravity = true;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                enabled = false; // Disable further updates
                break;
            }
        }
    }
}