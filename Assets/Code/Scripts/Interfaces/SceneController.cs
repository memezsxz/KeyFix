using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField]
    private float sceneFadeDuration;

    private SceneFader SceneFader;

    private void Awake()
    {
        SceneFader = GetComponentInChildren<SceneFader>();
    }


    private IEnumerator Start()
    {
        yield return SceneFader.FadeInCoroutine(sceneFadeDuration);
    }


    public void LoadScene(string sceneName) {
        StartCoroutine(loadScreenCoroutine(sceneName));
    }

    private IEnumerator loadScreenCoroutine(string sceneName)
    {
        yield return SceneFader.FadeOutCoroutine(sceneFadeDuration);
        yield return SceneManager.LoadSceneAsync(sceneName);
    }
}
