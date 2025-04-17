using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    public string sceneToLoad;
    public Slider progressBar;
    public float minLoadTime = 3f;

    private float loadingProgress = 0f;

 

    public void BeginLoading()
    {
        StartCoroutine(LoadAsyncWithDelay());
    }

    IEnumerator LoadAsyncWithDelay()
    {
        float timer = 0f;
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            // Calculate progress (from 0 to 1)
            float targetProgress = Mathf.Clamp01(operation.progress / 0.9f);

            // Smoothly increase the visual progress bar
            loadingProgress = Mathf.MoveTowards(loadingProgress, targetProgress, Time.deltaTime);

            if (progressBar != null)
                progressBar.value = loadingProgress;

            timer += Time.deltaTime;

            if (operation.progress >= 0.9f && timer >= minLoadTime)
            {
                // Final smooth fill to 100%
                while (loadingProgress < 1f)
                {
                    loadingProgress = Mathf.MoveTowards(loadingProgress, 1f, Time.deltaTime);
                    if (progressBar != null)
                        progressBar.value = loadingProgress;

                    yield return null;
                }

                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
