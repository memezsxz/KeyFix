using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BallController : MonoBehaviour
{
    public float ballSpeed; // Use this variable to store value for ball's speed.

    private Rigidbody rb; // Use this object to access the properties of the GameObject's Rigidbody component.
    private float xInput, zInput; // Use these variables to store user input.

    // Start is called before the first frame update
    void Start()
    {
        ballSpeed = 5.0f;
        rb = GetComponent<Rigidbody>();
        xInput = 0.0f;
        zInput = 0.0f;
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        xInput = movementVector.x;
        zInput = movementVector.y;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movement = new Vector3(xInput, 0f, zInput);
        rb.AddForce(movement * ballSpeed);
    }
}
