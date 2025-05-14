using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Handles scaling interactions for the player — shrinking and stretching over time.
/// </summary>
public class ScalerInteraction : MonoBehaviour
{
    #region Fields

    private Coroutine scaleRoutine;

    // Predefined scale values
    private readonly Vector3 shrinkScale = new(0.5f, 0.5f, 0.5f);
    private readonly Vector3 stretchScale = new(1.5f, 1.5f, 1.5f);

    #endregion

    #region Public API

    /// <summary>
    /// Shrinks the player to a smaller size over time.
    /// </summary>
    /// <param name="duration">Time in seconds to complete the scale animation.</param>
    /// <param name="onComplete">Optional callback once the scaling is done.</param>
    public void Shrink(float duration, Action onComplete = null)
    {
        if (scaleRoutine != null)
            StopCoroutine(scaleRoutine);

        scaleRoutine = StartCoroutine(ScaleOverTime(shrinkScale, duration, onComplete));
    }

    /// <summary>
    /// Enlarges the player to a stretched size over time.
    /// </summary>
    /// <param name="duration">Time in seconds to complete the scale animation.</param>
    /// <param name="onComplete">Optional callback once the scaling is done.</param>
    public void Stretch(float duration, Action onComplete = null)
    {
        if (scaleRoutine != null) StopCoroutine(scaleRoutine);

        scaleRoutine = StartCoroutine(ScaleOverTime(stretchScale, duration, onComplete));
    }

    /// <summary>
    /// Checks if the object is already at the shrink scale.
    /// </summary>
    public bool IsAlreadyShrunk(float tolerance = 0.01f)
    {
        return IsAtScale(shrinkScale, tolerance);
    }

    /// <summary>
    /// Checks if the object is already at the stretch scale.
    /// </summary>
    public bool IsAlreadyStretched(float tolerance = 0.01f)
    {
        return IsAtScale(stretchScale, tolerance);
    }

    /// <summary>
    /// Determines if the object's current scale is approximately equal to a target scale.
    /// </summary>
    /// <param name="target">The scale to compare with.</param>
    /// <param name="tolerance">Allowed margin of error.</param>
    public bool IsAtScale(Vector3 target, float tolerance = 0.01f)
    {
        return Vector3.Distance(transform.localScale, target) <= tolerance;
    }

    #endregion

    #region Internal Coroutines

    /// <summary>
    /// Smoothly scales the object from its current size to a target size.
    /// </summary>
    private IEnumerator ScaleOverTime(Vector3 targetScale, float duration, Action onComplete)
    {
        var startScale = transform.localScale;
        var timer = 0f;

        while (timer < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
        onComplete?.Invoke();
    }

    /// <summary>
    /// Smoothly moves the player to the center of this object over time.
    /// Not currently used directly, but may support future interactions.
    /// </summary>
    private IEnumerator MoveToCenterOverTime(Transform player, CharacterController controller, float duration)
    {
        var startPos = player.position;
        var targetPos = transform.position;
        var elapsed = 0f;

        while (elapsed < duration)
        {
            controller.enabled = false;
            player.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            controller.enabled = true;
            elapsed += Time.deltaTime;
            yield return null;
        }

        controller.enabled = false;
        player.position = targetPos;
        controller.enabled = true;
    }

    #endregion
}