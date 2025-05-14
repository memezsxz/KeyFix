using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles UI fade-in and fade-out effects using an Image overlay.
/// Used during scene transitions.
/// </summary>
public class SceneFader : MonoBehaviour
{
    /// <summary>
    /// The UI image used for fading.
    /// </summary>
    private Image fadeImage;

    private void Awake()
    {
        fadeImage = GetComponent<Image>();
    }

    /// <summary>
    /// Fades from black to transparent over the specified duration.
    /// </summary>
    /// <param name="duration">Time in seconds for the fade-in effect.</param>
    public IEnumerator FadeInCoroutine(float duration)
    {
        var startColor = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1f);
        var targetColor = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0f);

        yield return FadeCoroutine(startColor, targetColor, duration);

        // Disable the object after fade-in is complete
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Fades from transparent to black over the specified duration.
    /// </summary>
    /// <param name="duration">Time in seconds for the fade-out effect.</param>
    public IEnumerator FadeOutCoroutine(float duration)
    {
        var startColor = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0f);
        var targetColor = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1f);

        // Ensure the fader is visible during the effect
        gameObject.SetActive(true);
        yield return FadeCoroutine(startColor, targetColor, duration);
    }

    /// <summary>
    /// Performs the actual color interpolation over time.
    /// </summary>
    private IEnumerator FadeCoroutine(Color startColor, Color targetColor, float duration)
    {
        float elapsedTime = 0f;
        float progress = 0f;

        while (progress < 1f)
        {
            progress = elapsedTime / duration;
            fadeImage.color = Color.Lerp(startColor, targetColor, progress);

            yield return null;
            elapsedTime += Time.deltaTime;
        }

        fadeImage.color = targetColor; // Ensure final color is exact
    }
}
