using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{
    private Image fadeImage;

    private void Awake()
    {
        fadeImage = GetComponent<Image>();
    }

    public IEnumerator FadeInCoroutine(float duration)
    {
        var startColor = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1);
        var targetColor = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0);

        yield return FadeCoroutine(startColor, targetColor, duration);

        gameObject.SetActive(false);
    }

    public IEnumerator FadeOutCoroutine(float duration)
    {
        var startColor = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0);
        var targetColor = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1);


        gameObject.SetActive(true);
        yield return FadeCoroutine(startColor, targetColor, duration);
    }

    private IEnumerator FadeCoroutine(Color statrColor, Color targetColor, float duration)
    {
        float elapsedTime = 0;
        float elapsedPersentage = 0;

        while (elapsedPersentage < 1)
        {
            elapsedPersentage = elapsedTime / duration;
            fadeImage.color = Color.Lerp(statrColor, targetColor, elapsedPersentage);

            yield return null;

            elapsedTime += Time.deltaTime;
        }
    }
}