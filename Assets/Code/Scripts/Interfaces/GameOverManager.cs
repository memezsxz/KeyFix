using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    //public string retrySceneName = "Level1";      // Set your current level name
    //public string mainMenuSceneName = "MainMenu"; // Set your main menu scene

    public LoadingManager loadingScript;
    public GameObject loadingScreen;
    public GameObject GameOverScene;

    public void Retry()
    {
        //SceneManager.LoadScene(retrySceneName);
    }

    public void GoToMainMenu()
    {
        //SceneManager.LoadScene(mainMenuSceneName);
        Time.timeScale = 1f;
        GameStateTracker.returningFromGame = true;
        //SceneManager.LoadScene("Fatima_MainMenu"); // Replace with your actual main menu scene name
        GameOverScene.SetActive(false);

        loadingScript.sceneToLoad = "Fatima_MainMenu";
        loadingScreen.SetActive(true);
        loadingScript.BeginLoading();
    }


    //code for the displaying 
    
    //the duration of desplaying the screen 
    private float fadeInDuration = 1f;
    private float displayDuration = 3f;


    //game over audio 
    public AudioSource victoryAudio;
    //delay of the audio
    private float delay = 0.7f;

    //the scene object
    public GameObject scene;
    public CanvasGroup canvasGroup;



    public void ShowGameOverScene()
    {


        scene.SetActive(true);
        //StartCoroutine(PlayAfterDelay());
        StartCoroutine(ShowGameOver());
    }



    //to delay the sound playing time
    //IEnumerator PlayAfterDelay()
    //{
    //    yield return new WaitForSeconds(delay);
    //    victoryAudio.Play();
    //}


    //fade in and out
    IEnumerator ShowGameOver()
    {
        yield return Fade(0, 1, fadeInDuration); // Fade In
        //yield return new WaitForSeconds(displayDuration);

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
