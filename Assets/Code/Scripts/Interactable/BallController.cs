using UnityEngine;
using UnityEngine.InputSystem;

public class BallController : MonoBehaviour
{
    public float ballSpeed; // Use this variable to store value for ball's speed.

    private Rigidbody rb; // Use this object to access the properties of the GameObject's Rigidbody component.
    private float xInput, zInput; // Use these variables to store user input.

    // Start is called before the first frame update
    private void Start()
    {
        ballSpeed = 5.0f;
        rb = GetComponent<Rigidbody>();
        xInput = 0.0f;
        zInput = 0.0f;
    }

    // Update is called once per frame
    private void Update()
    {
        var movement = new Vector3(xInput, 0f, zInput);
        rb.AddForce(movement * ballSpeed);
    }

    private void OnMove(InputValue movementValue)
    {
        var movementVector = movementValue.Get<Vector2>();
        xInput = movementVector.x;
        zInput = movementVector.y;
    }
}