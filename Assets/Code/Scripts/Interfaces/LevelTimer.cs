using TMPro;
using UnityEngine;

/// <summary>
/// Controls a countdown timer with tick sounds and periodic zap effects.
/// Triggers a game over when the timer reaches zero.
/// </summary>
public class LevelTimer : MonoBehaviour
{
    #region Serialized Fields

    /// <summary>
    /// Total countdown time in seconds.
    /// </summary>
  public float countdownTime = 20f;

    /// <summary>
    /// Text element used to display the timer countdown.
    /// </summary>
    public TextMeshProUGUI timerText;

    /// <summary>
    /// Zap visual effect GameObject shown at intervals.
    /// </summary>
    public GameObject zapEffect;

    /// <summary>
    /// Sound to play when the zap effect is shown.
    /// </summary>
    public AudioClip zapSound;

    /// <summary>
    /// Sound that plays once per second as the timer counts down.
    /// </summary>
    public AudioClip tickSound;

    /// <summary>
    /// The scene or section of UI associated with this timer.
    /// </summary>
    public GameObject scene;

    #endregion

    #region Private Fields

    /// <summary>
    /// Internal timer that counts down from the set time.
    /// </summary>
    private float currentTime;

    /// <summary>
    /// Prevents multiple game over triggers.
    /// </summary>
    private bool gameOverTriggered;

    /// <summary>
    /// Time until the next tick sound is allowed.
    /// </summary>
    private float nextTickTime;

    /// <summary>
    /// Time until the next zap effect is shown.
    /// </summary>
    private float nextZapTime;

    /// <summary>
    /// Interval between zap effects in seconds.
    /// </summary>
    private readonly float zapInterval = 4f;

    #endregion

    #region Unity Methods

    /// <summary>
    /// Initializes the timer and prepares the room.
    /// </summary>
    public void Start()
    {
        Room_A_Start();
    }

    /// <summary>
    /// Updates the timer, triggers zap effects, tick sounds, and game over.
    /// </summary>
    private void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;

            // Format and update timer display
            var minutes = Mathf.FloorToInt(currentTime / 60);
            var seconds = Mathf.FloorToInt(currentTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            // Show zap effect at regular intervals
            if (currentTime <= nextZapTime)
            {
                ShowZap();
                nextZapTime -= zapInterval;
            }

            // Play tick sound at each second
            if (currentTime <= nextTickTime && currentTime > 0f)
            {
                if (!SoundManager.Instance.IsMusicPlaying)
                    SoundManager.Instance.PlayMusic(tickSound);

                nextTickTime = Mathf.Floor(currentTime) - 1f;
            }
        }
        else if (!gameOverTriggered)
        {
            // Trigger game over when time runs out
            timerText.text = "00:00";
            gameOverTriggered = true;
            StopAllEffects();
            GameManager.Instance.ChangeState(GameManager.GameState.GameOver);
        }
    }

    #endregion

    #region Timer Setup and Effects

    /// <summary>
    /// Prepares the timer and scene for Room A.
    /// </summary>
    public void Room_A_Start()
    {
        scene.SetActive(true);
        currentTime = countdownTime;
        zapEffect.SetActive(false);
        nextZapTime = currentTime - zapInterval;
        nextTickTime = Mathf.Floor(currentTime) - 1f;
    }

    /// <summary>
    /// Shows the zap effect and plays the zap sound.
    /// </summary>
    private void ShowZap()
    {
        zapEffect.SetActive(true);
        if (zapSound != null)
            SoundManager.Instance.PlaySound(zapSound);

        // Hide zap after 1 second
        Invoke(nameof(HideZap), 1f);
    }

    /// <summary>
    /// Hides the zap effect.
    /// </summary>
    private void HideZap()
    {
        zapEffect.SetActive(false);
    }

    /// <summary>
    /// Stops all sound and visual effects when the timer ends.
    /// </summary>
    private void StopAllEffects()
    {
        SoundManager.Instance.StopAllAudio();

        if (zapEffect.activeSelf)
            zapEffect.SetActive(false);
    }

    #endregion
}