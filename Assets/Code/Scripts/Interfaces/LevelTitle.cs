using System.Collections;
using TMPro;
using UnityEngine;

public class LevelTitle : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI levelDescText;

    [Header("Settings")] public string levelName = "LEVEL 1";

    public string levelDescription = "Nothing Here";
    public float fadeInDuration = 1f;
    public float fadeOutDuration = 1f;
    [SerializeField] private float displayDuration = 2f;

    private void Start()
    {
        levelText.text = levelName;
        levelDescText.text = levelDescription;
        StartCoroutine(ShowBanner());
    }

    private void OnDisable()
    {
        GameManager.Instance.HandleLevelTitleDone();
        print("level title disabled");
    }


    public void showLevelTitle()
    {
        gameObject.SetActive(true);
        Start();
    }

    private IEnumerator ShowBanner()
    {
        yield return Fade(0, 1, fadeInDuration); // Fade In
        yield return new WaitForSeconds(displayDuration);
        yield return Fade(1, 0, fadeOutDuration); // Fade Out
        gameObject.SetActive(false); // Optional: hide after fade
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float time = 0;
        while (time < duration)
        {
            var t = time / duration;
            canvasGroup.alpha = Mathf.Lerp(from, to, t);
            time += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = to;
    }
}