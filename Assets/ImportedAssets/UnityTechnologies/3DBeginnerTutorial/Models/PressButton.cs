using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressButton : MonoBehaviour
{
    public Transform redPart;
    public Vector3 pressedOffset = new Vector3(0, -0.1f, 0);
    private Vector3 originalPosition;
    private Vector3 targetPosition;
    public float moveSpeed = 5f;

    private bool isPressed = false;

    private void Start()
    {
        if (redPart != null)
        {
            originalPosition = redPart.localPosition;
            targetPosition = originalPosition;
        }
    }

    private void Update()
    {
        if (redPart != null)
        {
            redPart.localPosition = Vector3.Lerp(redPart.localPosition, targetPosition, moveSpeed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player") && !isPressed)
        {
            targetPosition = originalPosition + pressedOffset;
            isPressed = true;
            Debug.Log("Button Pressed!");
        }

    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Player") && isPressed)
        {
            targetPosition = originalPosition;
            isPressed = false;
        }
    }
}
