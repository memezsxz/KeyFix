using System.Collections;
using UnityEngine;

/// <summary>
/// Handles the display logic for the level complete scene,
/// including animations, sounds, and scene transition.
/// </summary>
public class LevelCompleteController : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Ensures the level complete scene is only shown once per session.
    /// </summary>
    private bool hasShown = false;

    /// <summary>
    /// The canvas group used for fading the level complete overlay.
    /// </summary>
    [Header("UI & Scene References")] public CanvasGroup canvasGroup;

    public string nextSceneName;

    /// <summary>
    /// Name of the next scene to load (not currently used).
    /// </summary>
    /// <summary>
    /// Animator controlling the completion animation.
    /// </summary>
    [Header("Animation")] public Animator animator;

    private static readonly int Start1 = Animator.StringToHash("start");
    private static readonly int Done = Animator.StringToHash("done");

    /// <summary>
    /// The animated object to apply transformations to.
    /// </summary>
    public Transform animatedObject;

    /// <summary>
    /// Audio clip to play when the level is completed.
    /// </summary>
    [Header("Audio")] public AudioClip victoryAudio;

    /// <summary>
    /// GameObject that contains the level complete UI.
    /// </summary>
    public GameObject scene;

    // Timing variables
    private readonly float delay = 0.7f;
    private readonly float displayDuration = 3f;
    private readonly float fadeInDuration = 1f;
    private readonly float fadeOutDuration = 1f;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        hasShown = false;
    }

    private void Start()
    {
        hasShown = false;
    }

    private void OnEnable()
    {
        hasShown = false;
        ResetAnimationState();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Triggers the level complete scene with animation, audio, and transition logic.
    /// </summary>
    public void ShowCompleteScene()
    {
        // Debug.Log("ShowCompleteScene CALLED at time: " + Time.time);

        if (hasShown)
        {
            // Debug.Log("ShowCompleteScene skipped (already shown).");
            return;
        }

        hasShown = true;

        // Reset the animator before starting
        animator.Rebind();
        animator.Update(0f);
        ResetAnimationState();
        LogAnimatorState();

        // Activate the scene UI
        scene.SetActive(true);

        // Start all feedback routines
        StartCoroutine(PlayAfterDelay());
        StartCoroutine(PlayAnimation());
        StartCoroutine(ShowLevelComplete());
    }

    #endregion

    #region Coroutines

    /// <summary>
    /// Plays animation on the object after a short delay.
    /// </summary>
    private IEnumerator PlayAnimation()
    {
        yield return new WaitForSeconds(0.5f);

        if (animator == null || animator.runtimeAnimatorController == null)
        {
            Debug.LogWarning("Animator is missing or not set. Skipping animation.");
            yield break;
        }

        animator.SetBool("start", true);
        yield return new WaitForSeconds(displayDuration + 0.5f);
        animator.SetBool("done", true);
    }

    /// <summary>
    /// Plays the victory sound after a short delay.
    /// </summary>
    private IEnumerator PlayAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        SoundManager.Instance.PlaySound(victoryAudio);
    }

    /// <summary>
    /// Fades in the completion UI, waits, then fades out and transitions to the Hallways scene.
    /// </summary>
    private IEnumerator ShowLevelComplete()
    {
        yield return GameManager.Instance.FadeCanvasGroup(canvasGroup, 0, 1, fadeInDuration); // Fade In
        yield return new WaitForSeconds(displayDuration);
        yield return GameManager.Instance.FadeCanvasGroup(canvasGroup, 1, 0, fadeOutDuration); // Fade Out

        hasShown = false;

        GameManager.Instance.HandleSceneLoad(GameManager.Scenes.HALLWAYS, GameManager.GameState.Playing);
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Resets all animator and UI state to default.
    /// </summary>
    private void ResetAnimationState()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        animator.SetBool(Start1, false);
        animator.SetBool(Done, false);
    }

    /// <summary>
    /// Logs the current animator state (for debugging).
    /// </summary>
    private void LogAnimatorState()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        // Debug.Log("Current Animator State: " + stateInfo.fullPathHash);
        // Debug.Log("Start Bool: " + animator.GetBool("start"));
        // Debug.Log("Done Bool: " + animator.GetBool("done"));
    }

    #endregion
}