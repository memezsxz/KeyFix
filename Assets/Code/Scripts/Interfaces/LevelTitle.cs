using System;
using System.Collections;
using System.Collections.Generic;
using Code.Scripts.Managers;
using GLTFast.Schema;
using TMPro;
using UnityEngine;

public class LevelTitle : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI levelDescText;

    [Header("Settings")]
    public string levelName = "LEVEL 1";
    public string levelDescription = "Nothing Here";
    public float fadeInDuration = 1f;
    public float fadeOutDuration = 1f;
    [SerializeField] float displayDuration = 2f;


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
        yield return Fade(0, 1, fadeInDuration); // Fade In
        yield return new WaitForSeconds(displayDuration);
        yield return Fade(1, 0, fadeOutDuration); // Fade Out
        gameObject.SetActive(false); // Optional: hide after fade
    }

    private void OnDisable()
    {
        GameManager.Instance.HandleLevelTitleDone();
        print("level title disabled");
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
