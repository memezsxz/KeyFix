using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private AudioClip gameOverSound;
    // public LoadingManager loadingScript;
    // public GameObject loadingScreen;
    // public GameObject GameOverScene;


    private void OnEnable()
    {
        print("enabled");
        SoundManager.Instance.PlaySound(gameOverSound);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        GameStateTracker.returningFromGame = true;
        GameManager.Instance.HandleSceneLoad(GameManager.Scenes.Main_Menu);
    }

    public void Retry()
    {
        GameManager.Instance.RestartLevel();
    }

    //
    // private float fadeInDuration = 1f;
    //
    // //the scene object
    // public GameObject scene;
    // public CanvasGroup canvasGroup;

    // public void ShowGameOverScene()
    // {
    //     GameManager.Instance.ChangeState(GameState.GameOver);
    //     scene.SetActive(true);
    //     StartCoroutine(ShowGameOver());
    // }

    //fade in and out
    // IEnumerator ShowGameOver()
    // {
    //     yield return Fade(0, 1, fadeInDuration);
    // }
    // IEnumerator Fade(float from, float to, float duration)
    // {
    //     float time = 0f;
    //     while (time < duration)
    //     {
    //         float t = time / duration;
    //         canvasGroup.alpha = Mathf.Lerp(from, to, t);
    //         time += Time.deltaTime;
    //         yield return null;
    //     }
    //     canvasGroup.alpha = to;
    // }
}