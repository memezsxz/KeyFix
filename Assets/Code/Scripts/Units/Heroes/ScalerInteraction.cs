using System;
using System.Collections;
using UnityEngine;

public class ScalerInteraction : MonoBehaviour
{
    private Coroutine scaleRoutine;
    private readonly Vector3 shrinkScale = new(0.5f, 0.5f, 0.5f);
    private readonly Vector3 stretchScale = new(1.5f, 1.5f, 1.5f);


    public void Shrink(float duration, Action onComplete = null)
    {
        if (scaleRoutine != null) StopCoroutine(scaleRoutine);
        scaleRoutine = StartCoroutine(ScaleOverTime(shrinkScale, duration, onComplete));
    }

    public void Stretch(float duration, Action onComplete = null)
    {
        if (scaleRoutine != null) StopCoroutine(scaleRoutine);
        scaleRoutine = StartCoroutine(ScaleOverTime(stretchScale, duration, onComplete));
    }

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
        onComplete?.Invoke(); // call the completion callback
    }

    public bool IsAlreadyShrunk(float tolerance = 0.01f)
    {
        return IsAtScale(shrinkScale, tolerance);
    }

    public bool IsAlreadyStretched(float tolerance = 0.01f)
    {
        return IsAtScale(stretchScale, tolerance);
    }

    public bool IsAtScale(Vector3 target, float tolerance = 0.01f)
    {
        return Vector3.Distance(transform.localScale, target) <= tolerance;
    }

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
}