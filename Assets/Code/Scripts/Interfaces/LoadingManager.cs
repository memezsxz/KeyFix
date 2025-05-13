using System.Collections;
using Code.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    public GameManager.Scenes sceneToLoad;
    public GameManager.GameState stateToLoadIn = GameManager.GameState.Initial;
    public Slider progressBar;
    public float minLoadTime = 3f;

    private float loadingProgress;

    public void BeginLoading()
    {
        StartCoroutine(LoadAsyncWithDelay());
    }


    private IEnumerator LoadAsyncWithDelay()
    {
        var timer = 0f;
        var operation = GameManager.Instance.LoadLevelAsync(sceneToLoad, stateToLoadIn);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            // Calculate progress (from 0 to 1)
            var targetProgress = Mathf.Clamp01(operation.progress / 0.9f);

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

                // ✅ Allow the scene to become visible
                operation.allowSceneActivation = true;


                // ✅ Wait one frame to allow Unity to show the new scene
                yield return null;

                // ✅ THEN call post-load logic
                GameManager.Instance.HandleSceneLoaded();
                GameManager.Instance.ChangeState(stateToLoadIn);
            }

            yield return null;
        }
    }
}