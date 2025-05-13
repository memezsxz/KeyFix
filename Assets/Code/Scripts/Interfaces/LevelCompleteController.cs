using System.Collections;
using Code.Scripts.Managers;
using UnityEngine;

public class LevelCompleteController : MonoBehaviour
{
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


    void OnEnable()
    {
        ResetAnimationState();
    }
    //useed to show the complete level scene 
    public void ShowCompleteScene()
    {
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
        animator.SetBool("start", true); // trigger fade-in
        yield return new WaitForSeconds(displayDuration + 0.5f);
        animator.SetBool("done", true); // trigger fade-out and shrink
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