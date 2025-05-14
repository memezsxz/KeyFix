using UnityEngine;

/// <summary>
/// Manages actions triggered during the game over state,
/// including retrying the level or returning to the main menu.
/// </summary>
public class GameOverManager : MonoBehaviour
{
    /// <summary>
    /// The sound clip to play when the game over screen is shown.
    /// </summary>
    [Header("Audio")] [SerializeField] private AudioClip gameOverSound;

    /// <summary>
    /// Called automatically when this object is enabled (e.g. Game Over screen becomes visible).
    /// Plays the game over sound.
    /// </summary>
    private void OnEnable()
    {
        SoundManager.Instance.PlaySound(gameOverSound);
    }

    /// <summary>
    /// Returns the player to the main menu scene.
    /// </summary>
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        GameStateTracker.returningFromGame = true;
        GameManager.Instance.HandleSceneLoad(GameManager.Scenes.Main_Menu);
    }

    /// <summary>
    /// Restarts the current level from the beginning.
    /// </summary>
    public void Retry()
    {
        GameManager.Instance.RestartLevel();
    }

    /*
    [Header("Optional Fade-In UI")]
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private GameObject scene;
    [SerializeField] private CanvasGroup canvasGroup;

    /// <summary>
    /// Enables the game over scene UI with a fade-in effect.
    /// </summary>
    public void ShowGameOverScene()
    {
        GameManager.Instance.ChangeState(GameState.GameOver);
        scene.SetActive(true);
        StartCoroutine(ShowGameOver());
    }

    private IEnumerator ShowGameOver()
    {
        yield return Fade(0, 1, fadeInDuration);
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            float t = time / duration;
            canvasGroup.alpha = Mathf.Lerp(from, to, t);
            time += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = to;
    }
    */
}