using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerJump : MonoBehaviour
{
    private Rigidbody _rigidbody; // Freeze Y position, and X/Y/Z rotation in Rigidbody constraints
    private Animator _animator;

    private bool _isGrounded;
    [SerializeField] private float jumpHeight = 20;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }
    //
    // public void OnJump(InputValue context)
    // {
    //     if (!context.isPressed || !IsGrounded()) return;
    //     _rigidbody.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
    //     Debug.Log("Jump");
    // }


    public void OnJump(InputValue context)
    {
        if (!context.isPressed || !IsGrounded()) return;

        _rigidbody.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
        // _animator?.SetTrigger("Jump"); 
        Debug.Log("Jump");
    }

    bool IsGrounded()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        float rayLength = 0.2f;
        bool grounded = Physics.Raycast(origin, Vector3.down, rayLength);
        return grounded;
    }
}