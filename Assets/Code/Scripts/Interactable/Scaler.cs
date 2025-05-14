using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Triggers a shrinking or stretching transformation when the player enters the trigger area.
/// Also plays a sound and smoothly moves the player to the scaler's center before scaling.
/// </summary>
public class Scaler : MonoBehaviour
{
    #region Enums

    /// <summary>
    /// Determines whether the player will shrink or stretch.
    /// </summary>
    public enum ScaleType
    {
        Shrink,
        Stretch
    }

    #endregion

    #region Serialized Fields

    /// <summary>
    /// Type of scaling to apply when the player enters.
    /// </summary>
    [SerializeField] private ScaleType scaleType;

    /// <summary>
    /// Sound to play when shrinking.
    /// </summary>
    [SerializeField] private AudioClip shrinkerSound;

    /// <summary>
    /// Sound to play when stretching.
    /// </summary>
    [SerializeField] private AudioClip stretcherSound;

    #endregion

    #region Private Fields

    /// <summary>
    /// The selected sound clip based on scaleType.
    /// </summary>
    private AudioClip sound;

    /// <summary>
    /// Ensures trigger only fires once until exited.
    /// </summary>
    private bool hasTriggered;

    /// <summary>
    /// Duration of the scaling transformation.
    /// </summary>
    private readonly float timeInSeconds = 2;

    #endregion

    #region Unity Methods

    private void Start()
    {
        // Choose the correct sound based on the configured scaling type
        sound = scaleType == ScaleType.Shrink ? shrinkerSound : stretcherSound;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered || !other.CompareTag("Player")) return;
        hasTriggered = true;

        var controller = other.GetComponent<CharacterController>();
        var scaler = other.GetComponent<ScalerInteraction>();

        // Ensure the player has necessary components
        if (controller != null && scaler != null)
        {
            // Skip scaling if already at target size
            var alreadyAtTarget = scaleType switch
            {
                ScaleType.Shrink => scaler.IsAlreadyShrunk(),
                ScaleType.Stretch => scaler.IsAlreadyStretched(),
                _ => true
            };

            if (alreadyAtTarget)
            {
                GameManager.Instance.TogglePlayerMovement(true);
                return;
            }

            // Assign the correct scaling action
            Action<float, Action> performScale = scaleType switch
            {
                ScaleType.Shrink => scaler.Shrink,
                ScaleType.Stretch => scaler.Stretch,
                _ => null
            };

            SoundManager.Instance.PlaySound(sound);

            // Move player to center of scaler, then perform the scaling
            StartCoroutine(MoveAndThenScale(other.transform, controller, scaler, performScale, 0.5f));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Allow the scaler to be triggered again when the player exits
        if (other.CompareTag("Player")) hasTriggered = false;
    }

    #endregion

    #region Scaling Logic

    /// <summary>
    /// Smoothly moves the player to the center of the scaler, then invokes the scaling behavior.
    /// </summary>
    private IEnumerator MoveAndThenScale(Transform player, CharacterController controller, ScalerInteraction scaler,
        Action<float, Action> performScale, float moveDuration)
    {
        yield return MoveToCenterOverTime(player, controller, moveDuration);

        // Disable movement while scaling
        GameManager.Instance.TogglePlayerMovement(false);

        performScale?.Invoke(timeInSeconds, () =>
        {
            // Re-enable movement after scaling is complete
            GameManager.Instance.TogglePlayerMovement(true);
        });
    }

    /// <summary>
    /// Moves the player to the scaler's center over a set duration using linear interpolation.
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
