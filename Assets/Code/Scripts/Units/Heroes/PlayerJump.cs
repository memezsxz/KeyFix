using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : MonoBehaviour
{
    [SerializeField] private float jumpHeight = 20;
    private Animator _animator;

    private bool _isGrounded;
    private Rigidbody _rigidbody; // Freeze Y position, and X/Y/Z rotation in Rigidbody constraints

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }

    public void OnJump(InputValue context)
    {
        if (!context.isPressed || !IsGrounded()) return;

        _rigidbody.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
        // _animator?.SetTrigger("Jump"); 
        Debug.Log("Jump");
    }

    private bool IsGrounded()
    {
        var origin = transform.position + Vector3.up * 0.1f;
        var rayLength = 0.2f;
        var grounded = Physics.Raycast(origin, Vector3.down, rayLength);
        return grounded;
    }
}