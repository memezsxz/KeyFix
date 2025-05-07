using System;
using System.Collections;
using System.Collections.Generic;
using Code.Scripts.Managers;
using UnityEngine;
using UnityEngine.Serialization;

public class PressButton : MonoBehaviour
{
    [SerializeField] Transform insidePart;
    public Vector3 pressedOffset = new Vector3(0, -0.1f, 0);
    private Vector3 originalPosition;
    private Vector3 targetPosition;
    [SerializeField] float moveSpeed = 5f;
    public PressedColor AssignedColor
    {
        get { return pressedColor;} }
    public bool IsPressed { get; private set; }

    private Material material;
    [SerializeField] AudioClip pressSound;

    private PressedColor pressedColor;

    public Action<PressedColor> OnPressed;
    public Action<PressedColor> OnReleased;

    public enum PressedColor
    {
        Red,
        Green,
        Blue,
    }

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
        {
            insidePart.localPosition =
                Vector3.Lerp(insidePart.localPosition, targetPosition, moveSpeed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player") && !IsPressed)
        {
            targetPosition = originalPosition + pressedOffset;
            IsPressed = true;
            if (pressSound != null && !SoundManager.Instance.IsSoundPlaying)
            {
                SoundManager.Instance.PlaySound(pressSound);
            }
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
        pressedColor = color;
        material.color = pressedColor switch
        {
            PressedColor.Blue => Color.blue,
            PressedColor.Red => Color.red,
            PressedColor.Green => Color.green,
            _ => material.color
        };
    }
}