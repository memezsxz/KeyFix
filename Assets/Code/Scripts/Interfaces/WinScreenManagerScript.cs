using System.Collections;
using UnityEngine;

public class WinScreenManagerScript : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public AudioSource winAudio;
    public float fadeDuration = 1f;
    public float displayDuration = 2.5f;

    private void Start()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void ShowWinScreen()
    {
        StartCoroutine(PlayWinSequence());
    }

    private IEnumerator PlayWinSequence()
    {
        // Fade In
        yield return StartCoroutine(FadeCanvas(0f, 1f, fadeDuration));

        // Play Sound
        if (winAudio != null) winAudio.Play();

        // Hold
        yield return new WaitForSeconds(displayDuration);

        // Fade Out
        yield return StartCoroutine(FadeCanvas(1f, 0f, fadeDuration));
    }

    private IEnumerator FadeCanvas(float from, float to, float duration)
    {
        var elapsed = 0f;
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