using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class SpeedersInteraction : MonoBehaviour
{
    [Header("Push Settings")] [SerializeField]
    private float pushSpeed = 8f;

    [SerializeField] private float stoppingDistance = 0.05f;
    [SerializeField] private float downwardNudge = 0.01f;

    [Header("Animation")] [SerializeField]
    private string blendParam = "Blend"; // In case animation uses different naming

    private Animator anim;

    private CharacterController controller;
    private Coroutine moveToPositionRoutine;

    public bool IsBeingPushed { get; private set; }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    // Optional hooks (public events)
    public event Action OnPushStart;
    public event Action OnPushEnd;

    public void MoveTowards(Vector3 position)
    {
        if (moveToPositionRoutine != null)
            StopCoroutine(moveToPositionRoutine);

        moveToPositionRoutine = StartCoroutine(MoveToTarget(position));
    }

    // workes perfectly
    private IEnumerator MoveToTarget(Vector3 targetPosition)
    {
        IsBeingPushed = true;
        OnPushStart?.Invoke();

        if (anim != null && !string.IsNullOrEmpty(blendParam))
            anim.SetFloat(blendParam, 0); // Halt blend animation if applicable

        while (true)
        {
            var toTarget = targetPosition - transform.position;
            var dist = toTarget.magnitude;

            if (dist <= stoppingDistance)
                break;

            var step = toTarget.normalized * pushSpeed * Time.deltaTime;

            // Clamp movement to avoid jittering overshoots
            if (step.magnitude > dist)
                step = toTarget;

            controller.Move(step);
            yield return null;
        }

        // Snap to final position
        controller.enabled = false;
        transform.position = targetPosition;
        controller.enabled = true;

        // Ground stabilization
        yield return new WaitForFixedUpdate();
        controller.Move(Vector3.down * downwardNudge);

        IsBeingPushed = false;
        OnPushEnd?.Invoke();
    }

    public void TeleportTo(Vector3 position)
    {
        controller.enabled = false;
        transform.position = position;
        controller.enabled = true;
    }
}