using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages asynchronous scene loading with a visual progress bar and optional minimum display duration.
/// </summary>
public class LoadingManager : MonoBehaviour
{
    #region Serialized Fields

    /// <summary>
    /// The scene to load when loading begins.
    /// </summary>
    public GameManager.Scenes sceneToLoad;

    /// <summary>
    /// The game state to switch to after loading completes.
    /// </summary>
    public GameManager.GameState stateToLoadIn = GameManager.GameState.Initial;

    /// <summary>
    /// Optional UI slider to visually display loading progress.
    /// </summary>
    public Slider progressBar;

    /// <summary>
    /// Minimum time (in seconds) the loading screen should be displayed, even if loading is faster.
    /// </summary>
    public float minLoadTime = 3f;

    #endregion

    #region Private Fields

    /// <summary>
    /// The current progress value used for smooth visual transitions.
    /// </summary>
    private float loadingProgress;

    #endregion

    #region Public Methods

    /// <summary>
    /// Starts the asynchronous loading process with progress tracking and delay.
    /// </summary>
    public void BeginLoading()
    {
        StartCoroutine(LoadAsyncWithDelay());
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Loads the scene asynchronously, waits for minimum time, and handles progress bar animation.
    /// </summary>
    private IEnumerator LoadAsyncWithDelay()
    {
        float timer = 0f;
        var operation = GameManager.Instance.LoadLevelAsync(sceneToLoad, stateToLoadIn);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            // Unity loads scenes to 0.9 and waits for allowSceneActivation
            var targetProgress = Mathf.Clamp01(operation.progress / 0.9f);

            // Smoothly interpolate progress visually
            loadingProgress = Mathf.MoveTowards(loadingProgress, targetProgress, Time.deltaTime);

            if (progressBar != null)
                progressBar.value = loadingProgress;

            timer += Time.deltaTime;

            // When loading is done and minimum time has passed, complete the bar and activate scene
            if (operation.progress >= 0.9f && timer >= minLoadTime)
            {
                while (loadingProgress < 1f)
                {
                    loadingProgress = Mathf.MoveTowards(loadingProgress, 1f, Time.deltaTime);
                    if (progressBar != null)
                        progressBar.value = loadingProgress;

                    yield return null;
                }

                // Activate the scene
                operation.allowSceneActivation = true;

                // Wait one frame before executing post-load logic
                yield return null;

                GameManager.Instance.HandleSceneLoaded();
                GameManager.Instance.ChangeState(stateToLoadIn);
            }

            yield return null;
        }
    }

    #endregion
}