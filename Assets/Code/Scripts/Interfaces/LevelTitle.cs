using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Displays a level title and description using fade-in and fade-out effects.
/// Called at the beginning of a level to introduce the area.
/// </summary>
public class LevelTitle : MonoBehaviour
{
    #region UI References

    /// <summary>
    /// Canvas group for controlling the alpha fade of the title UI.
    /// </summary>
    [SerializeField] private CanvasGroup canvasGroup;

    /// <summary>
    /// Text element for the level name/title.
    /// </summary>
    [SerializeField] private TextMeshProUGUI levelText;

    /// <summary>
    /// Text element for the level description.
    /// </summary>
    [SerializeField] private TextMeshProUGUI levelDescText;

    #endregion

    #region Settings

    /// <summary>
    /// The name/title of the level to be shown.
    /// </summary>
    [Header("Settings")] public string levelName = "LEVEL 1";

    /// <summary>
    /// A short description for the level.
    /// </summary>
    public string levelDescription = "Nothing Here";

    /// <summary>
    /// Duration for the fade-in effect.
    /// </summary>
    public float fadeInDuration = 1f;

    /// <summary>
    /// Duration for the fade-out effect.
    /// </summary>
    public float fadeOutDuration = 1f;

    /// <summary>
    /// How long the title remains fully visible before fading out.
    /// </summary>
    [SerializeField] private float displayDuration = 2f;

    #endregion

    #region Unity Methods

    private void Start()
    {
        // Set the text fields and start the banner coroutine
        levelText.text = levelName;
        levelDescText.text = levelDescription;
        StartCoroutine(ShowBanner());
    }

    private void OnDisable()
    {
        // Notify the game manager when the level title finishes showing
        GameManager.Instance.HandleLevelTitleDone();
        // Debug.Log("Level title disabled");
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Activates and begins showing the level title with fade effects.
    /// </summary>
    public void showLevelTitle()
    {
        gameObject.SetActive(true);
        Start();
    }

    #endregion

    #region Coroutines

    /// <summary>
    /// Handles the full sequence: fade in, display, and fade out.
    /// </summary>
    private IEnumerator ShowBanner()
    {
        yield return Fade(0, 1, fadeInDuration); // Fade In
        yield return new WaitForSeconds(displayDuration);
        yield return Fade(1, 0, fadeOutDuration); // Fade Out
        gameObject.SetActive(false); // Optional: disable object after display
    }

    /// <summary>
    /// Fades the canvas group's alpha from one value to another over time.
    /// </summary>
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

    #endregion
}