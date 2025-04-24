using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleteController : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    private float fadeInDuration = 1f;
    private float displayDuration = 3f;
    private float fadeOutDuration = 1f;
    public string nextSceneName;

    public Animator animator;
    public Transform animatedObject;

    public AudioSource victoryAudio;
    private float delay = 0.7f;
    public GameObject scene;



    //useed to show the complete level scene 
    public void ShowCompleteScene() {

        
        scene.SetActive(true);
        StartCoroutine(PlayAfterDelay());
        StartCoroutine(PlayAnimation());
        StartCoroutine(ShowLevelComplete());
    }




    //delay the animation scale of the image.
    IEnumerator PlayAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("start", true); // trigger fade-in
        yield return new WaitForSeconds(displayDuration+0.5f);
        animator.SetBool("done", true); // trigger fade-out and shrink

    }


    
    //to delay the sound playing time
    IEnumerator PlayAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        victoryAudio.Play();
    }


    //fade in and out
    IEnumerator ShowLevelComplete()
    {
        yield return Fade(0, 1, fadeInDuration); // Fade In
        yield return new WaitForSeconds(displayDuration);
        yield return Fade(1, 0, fadeOutDuration); // Fade Out

        SceneManager.LoadScene(nextSceneName);
    }


    IEnumerator Fade(float from, float to, float duration)
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
}
