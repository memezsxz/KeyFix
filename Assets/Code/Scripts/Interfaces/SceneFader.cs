using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{
    private Image fadeImage;

    private void Awake()
    {
        fadeImage = GetComponent<Image>();
    }

    public IEnumerator FadeInCoroutine(float duration) {

        Color startColor = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1);
        Color targetColor = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0);

        yield return FadeCoroutine(startColor, targetColor, duration);

        gameObject.SetActive(false);

    }
    
    public IEnumerator FadeOutCoroutine(float duration)
    {
        Color startColor = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0);
        Color targetColor = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1);


        gameObject.SetActive(true);
        yield return FadeCoroutine(startColor, targetColor, duration);
    }

    private IEnumerator FadeCoroutine(Color statrColor, Color targetColor, float duration) {
        float elapsedTime = 0;
        float elapsedPersentage = 0;

        while (elapsedPersentage < 1) {
            elapsedPersentage = elapsedTime / duration;
            fadeImage.color = Color.Lerp(statrColor, targetColor, elapsedPersentage);

            yield return null;

            elapsedTime += Time.deltaTime;
        }
    }
}
