using System.Collections;
using System.Collections.Generic;
using Code.Scripts.Managers;
using GLTFast.Schema;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Code.Scripts.Managers.GameManager;

public class GameOverManager : MonoBehaviour
{

    public LoadingManager loadingScript;
    public GameObject loadingScreen;
    public GameObject GameOverScene;

    public void Retry()
    {
        GameManager.Instance.RestartLevel();
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        GameStateTracker.returningFromGame = true;
        GameOverScene.SetActive(false);

        loadingScript.sceneToLoad = GameManager.Scenes.Main_Menu;
        loadingScreen.SetActive(true);
        loadingScript.BeginLoading();
    }
    
    private float fadeInDuration = 1f;

    //the scene object
    public GameObject scene;
    public CanvasGroup canvasGroup;

    public void ShowGameOverScene()
    {
        scene.SetActive(true);
        StartCoroutine(ShowGameOver());
    }

    //fade in and out
    IEnumerator ShowGameOver()
    {
        yield return Fade(0, 1, fadeInDuration);
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
