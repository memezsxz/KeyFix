using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class SpeedersInteraction : MonoBehaviour
{
    #region Push Settings

    [Header("Push Settings")] [SerializeField]
    private float pushSpeed = 8f;

    [SerializeField] private float stoppingDistance = 0.05f;

    [Tooltip("Slight downward correction to ensure the player lands properly.")] [SerializeField]
    private float downwardNudge = 0.01f;

    #endregion

    #region Animation

    [Header("Animation")] [Tooltip("Name of the animation blend parameter.")] [SerializeField]
    private string blendParam = "Blend";

    private Animator anim;

    #endregion

    #region State and References

    private CharacterController controller;
    private Coroutine moveToPositionRoutine;

    /// <summary>
    /// Whether the player is currently being pushed by a speeder.
    /// </summary>
    public bool IsBeingPushed { get; private set; }

    #endregion

    #region Events

    /// <summary>
    /// Invoked when push movement starts.
    /// </summary>
    public event Action OnPushStart;

    /// <summary>
    /// Invoked when push movement ends.
    /// </summary>
    public event Action OnPushEnd;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    #endregion

    #region Movement API

    /// <summary>
    /// Begins smoothly moving the player toward the given target position using physics movement.
    /// </summary>
    /// <param name="position">Target world position to move to.</param>
    public void MoveTowards(Vector3 position)
    {
        if (moveToPositionRoutine != null)
            StopCoroutine(moveToPositionRoutine);

        moveToPositionRoutine = StartCoroutine(MoveToTarget(position));
    }

    /// <summary>
    /// Instantly teleports the player to a position without animation.
    /// </summary>
    /// <param name="position">Destination position.</param>
    public void TeleportTo(Vector3 position)
    {
        controller.enabled = false;
        transform.position = position;
        controller.enabled = true;
    }

    #endregion

    #region Push Coroutine

    /// <summary>
    /// Coroutine that performs frame-based smooth movement toward a point.
    /// </summary>
    /// <param name="targetPosition">Destination to reach.</param>
    private IEnumerator MoveToTarget(Vector3 targetPosition)
    {
        IsBeingPushed = true;
        OnPushStart?.Invoke();

        // Reset animation blend to idle (optional)
        if (anim != null && !string.IsNullOrEmpty(blendParam))
            anim.SetFloat(blendParam, 0);

        while (true)
        {
            var toTarget = targetPosition - transform.position;
            var dist = toTarget.magnitude;

            if (dist <= stoppingDistance)
                break;

            var step = toTarget.normalized * pushSpeed * Time.deltaTime;

            // Prevent overshooting
            if (step.magnitude > dist)
                step = toTarget;

            controller.Move(step);
            yield return null;
        }

        // Snap into position
        controller.enabled = false;
        transform.position = targetPosition;
        controller.enabled = true;

        // Force grounding
        yield return new WaitForFixedUpdate();
        controller.Move(Vector3.down * downwardNudge);

        IsBeingPushed = false;
        OnPushEnd?.Invoke();
    }

    #endregion
}