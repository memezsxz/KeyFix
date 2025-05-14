using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles scene transitions with fade-in and fade-out effects.
/// Uses a child SceneFader to manage UI transitions.
/// </summary>
public class SceneController : MonoBehaviour
{
    /// <summary>
    /// Duration for scene fade in/out animations.
    /// </summary>
    [SerializeField] private float sceneFadeDuration;

    /// <summary>
    /// Reference to the scene fader component.
    /// </summary>
    private SceneFader SceneFader;

    private void Awake()
    {
        SceneFader = GetComponentInChildren<SceneFader>();
    }

    private IEnumerator Start()
    {
        yield return SceneFader.FadeInCoroutine(sceneFadeDuration);
    }

    /// <summary>
    /// Begins loading a new scene with a fade-out transition.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    public void LoadScene(string sceneName)
    {
        StartCoroutine(loadScreenCoroutine(sceneName));
    }

    /// <summary>
    /// Coroutine that fades out, loads a new scene, and completes transition.
    /// </summary>
    private IEnumerator loadScreenCoroutine(string sceneName)
    {
        yield return SceneFader.FadeOutCoroutine(sceneFadeDuration);
        yield return SceneManager.LoadSceneAsync(sceneName);
    }
}