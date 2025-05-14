using System.Collections;
using UnityEngine;

/// <summary>
/// Handles the visual and audio sequence for displaying the win screen.
/// </summary>
public class WinScreenManagerScript : MonoBehaviour
{
    /// <summary>
    /// Canvas group used to fade in and out the win screen UI.
    /// </summary>
    public CanvasGroup canvasGroup;

    /// <summary>
    /// Audio source to play when the win screen is shown.
    /// </summary>
    public AudioSource winAudio;

    /// <summary>
    /// Duration of fade-in and fade-out animations.
    /// </summary>
    public float fadeDuration = 1f;

    /// <summary>
    /// How long the win screen should stay fully visible before fading out.
    /// </summary>
    public float displayDuration = 2.5f;

    private void Start()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    /// <summary>
    /// Public method to begin the win screen sequence.
    /// </summary>
    public void ShowWinScreen()
    {
        StartCoroutine(PlayWinSequence());
    }

    /// <summary>
    /// Executes the full win sequence: fade in, play sound, wait, and fade out.
    /// </summary>
    private IEnumerator PlayWinSequence()
    {
        // Fade in the win screen
        yield return StartCoroutine(FadeCanvas(0f, 1f, fadeDuration));

        // Play win sound if available
        if (winAudio != null) winAudio.Play();

        // Wait while win screen is fully visible
        yield return new WaitForSeconds(displayDuration);

        // Fade out the win screen
        yield return StartCoroutine(FadeCanvas(1f, 0f, fadeDuration));
    }

    /// <summary>
    /// Fades the canvas group alpha from one value to another over the given duration.
    /// Also updates input interactivity based on visibility.
    /// </summary>
    private IEnumerator FadeCanvas(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = to;
        canvasGroup.interactable = to > 0;
        canvasGroup.blocksRaycasts = to > 0;
    }
}