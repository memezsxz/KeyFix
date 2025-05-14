using System;
using UnityEngine;

public class PressButton : MonoBehaviour
{
    public enum PressedColor
    {
        Red,
        Green,
        Blue
    }

    [SerializeField] private Transform insidePart;
    public Vector3 pressedOffset = new(0, -0.1f, 0);
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private AudioClip pressSound;

    private Material material;

    public Action<PressedColor> OnPressed;
    public Action<PressedColor> OnReleased;
    private Vector3 originalPosition;

    private Vector3 targetPosition;

    public PressedColor AssignedColor { get; private set; }

    public bool IsPressed { get; private set; }

    private void Start()
    {
        material = insidePart.GetComponent<MeshRenderer>().material;
        if (insidePart != null)
        {
            originalPosition = insidePart.localPosition;
            targetPosition = originalPosition;
        }
    }


    private void Update()
    {
        if (insidePart != null)
            insidePart.localPosition =
                Vector3.Lerp(insidePart.localPosition, targetPosition, moveSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player") && !IsPressed)
        {
            targetPosition = originalPosition + pressedOffset;
            IsPressed = true;
            if (pressSound != null && !SoundManager.Instance.IsSoundPlaying)
                SoundManager.Instance.PlaySound(pressSound);
            ColorCoordinator.Instance.CheckButtonMatch();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Player") && IsPressed)
        {
            targetPosition = originalPosition;
            IsPressed = false;
        }
    }

    public void ChangeColor(PressedColor color)
    {
        AssignedColor = color;
        material.color = AssignedColor switch
        {
            PressedColor.Blue => Color.blue,
            PressedColor.Red => Color.red,
            PressedColor.Green => Color.green,
            _ => material.color
        };
    }
}