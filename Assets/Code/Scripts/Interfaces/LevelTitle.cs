using System.Collections;
using System.Collections.Generic;
using GLTFast.Schema;
using TMPro;
using UnityEngine;

public class LevelTitle : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI levelDescText;

    [Header("Settings")]
    public string levelName = "LEVEL 1";
    public string levelDescription = "Nothing Here";
    public float fadeDuration = 1f;
    public float displayDuration = 2f;


    public void showLevelTitle() {
        gameObject.SetActive(true);
        Start();
    }

    private void Start()
    {
        levelText.text = levelName;
        levelDescText.text = levelDescription;
        StartCoroutine(ShowBanner());
    }

    IEnumerator ShowBanner()
    {
        yield return Fade(0, 1, fadeDuration); // Fade In
        yield return new WaitForSeconds(displayDuration);
        yield return Fade(1, 0, fadeDuration); // Fade Out
        gameObject.SetActive(false); // Optional: hide after fade
    }

    IEnumerator Fade(float from, float to, float duration)
    {
        float time = 0;
        while (time < duration)
        {
            float t = time / duration;
            canvasGroup.alpha = Mathf.Lerp(from, to, t);
            time += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = to;
    }
}
