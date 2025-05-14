using System.Collections;
using UnityEngine;

public class LevelCompleteController : MonoBehaviour
{

    private bool hasShown = false;

    public CanvasGroup canvasGroup;
    public string nextSceneName;

    public Animator animator;
    public Transform animatedObject;

    public AudioClip victoryAudio;
    public GameObject scene;
    private readonly float delay = 0.7f;
    private readonly float displayDuration = 3f;
    private readonly float fadeInDuration = 1f;
    private readonly float fadeOutDuration = 1f;

    void Start()
    {
        hasShown = false; // Each time the scene loads
    }

    void Awake()
    {
        hasShown = false;
    }

    void OnEnable()
    {
        hasShown = false;  //  Only reset it here
        ResetAnimationState();
    }
    //useed to show the complete level scene 
    public void ShowCompleteScene()
    {
        Debug.Log("ShowCompleteScene CALLED at time: " + Time.time);

        if (hasShown)
        {
            Debug.Log("ShowCompleteScene skipped (already shown).");
            return;
        }

        hasShown = true;

        animator.Rebind();    // Reset Animator to Entry
        animator.Update(0f);

        ResetAnimationState(); // Ensure clean state
        LogAnimatorState();
        scene.SetActive(true);
        StartCoroutine(PlayAfterDelay());
        StartCoroutine(PlayAnimation());
        StartCoroutine(ShowLevelComplete());
    }

    //delay the animation scale of the image.
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

    //to delay the sound playing time
    private IEnumerator PlayAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        SoundManager.Instance.PlaySound(victoryAudio);
    }

    //fade in and out
    private IEnumerator ShowLevelComplete()
    {
        yield return GameManager.Instance.FadeCanvasGroup(canvasGroup, 0, 1, fadeInDuration); // Fade In
        yield return new WaitForSeconds(displayDuration);
        yield return GameManager.Instance.FadeCanvasGroup(canvasGroup, 1, 0, fadeOutDuration); // Fade Out

        hasShown = false;

        GameManager.Instance.HandleSceneLoad(GameManager.Scenes.HALLWAYS, GameManager.GameState.Playing);
    }

    private void ResetAnimationState()
    {

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        animator.SetBool("start", false);
        animator.SetBool("done", false);
    }
    private void LogAnimatorState()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        // Debug.Log("Current Animator State: " + stateInfo.fullPathHash);
        // Debug.Log("Start Bool: " + animator.GetBool("start"));
        // Debug.Log("Done Bool: " + animator.GetBool("done"));
    }
}