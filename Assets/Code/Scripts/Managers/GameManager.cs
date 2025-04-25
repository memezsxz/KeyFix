using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Code.Scripts.Managers
{
    public class GameManager : Singleton<GameManager>, IDataPersistence
    {
        #region Fields & Properties

        public Scenes CurrentScene { get; private set; }
        public GameState State { get; private set; }

        public static event Action<GameState> OnBeforeGameStateChanged;
        public static event Action<GameState> OnAfterGameStateChanged;

        // private bool IntroScenePlayed = false;

        [SerializeField] GameObject gameOverCanvas;
        [SerializeField] GameObject pauseMenuCanvas;
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private LoadingManager loadingScript;

        private static readonly Dictionary<GameManager.Scenes, string> SceneNameMap = new()
        {
            { GameManager.Scenes.HALLWAYS, "Hallways" },
            { GameManager.Scenes.ESC_KEY, "ESC_Key" },
            { GameManager.Scenes.W_KEY, "W_Key" },
            { GameManager.Scenes.A_KEY, "A_Key" },
            { GameManager.Scenes.SPACE_KEY, "Space_Key" },
            { GameManager.Scenes.G_KEY, "G_Key" },
            { GameManager.Scenes.ARROW_KEYS, "Arrow_Keys" },
            { GameManager.Scenes.P_KEY, "P_Key" },
            { GameManager.Scenes.Main_Menu, "Main_Menu" }
        };

        #endregion

        #region Enums

        public enum GameState
        {
            Initial,
            CutScene,
            Playing,
            Paused,
            GameOver,
        }

        [Serializable]
        public enum Scenes
        {
            HALLWAYS,
            ESC_KEY,
            W_KEY,
            A_KEY,
            SPACE_KEY,
            G_KEY,
            ARROW_KEYS,
            P_KEY,
            Main_Menu,
        }

        #endregion

        #region Unity Methods

        void Start()
        {
            DebugController.Instance?.AddDebugCommand(new DebugCommand("gm_test", "testing from the game manager", "",
                () => Debug.Log("working in game manager")));

            ChangeState(GameState.Initial);

            var sceneName = SceneManager.GetActiveScene().name;
            var match = SceneNameMap.FirstOrDefault(pair => pair.Value == sceneName);

            if (!EqualityComparer<GameManager.Scenes>.Default.Equals(match.Key, default))
            {
                CurrentScene = match.Key;
            }
            else
            {
                Debug.LogWarning($"Scene name '{sceneName}' not found in SceneNameMap.");
            }
        }

        #endregion

        #region Game State Management

        public void ChangeState(GameState newState)
        {
            if (State == newState) return;

            OnBeforeGameStateChanged?.Invoke(newState);

            switch (newState)
            {
                case GameState.Initial:
                    HandelInitialState();
                    break;
                case GameState.Paused:
                    if (CurrentScene != Scenes.Main_Menu) HandlePause();
                    break;
                case GameState.Playing:
                    if (CurrentScene != Scenes.Main_Menu) HandleResume();
                    break;
                case GameState.CutScene:
                    break;
                case GameState.GameOver:
                    if (CurrentScene != Scenes.Main_Menu) StartCoroutine(HandleGameOver());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }

            State = newState;

            OnAfterGameStateChanged?.Invoke(newState);
        }

        private void HandelInitialState()
        {
            // inisialize the player in the right spot
        }

        #endregion
        
        #region Scene & Level Management

        public void RestartLevel()
        {
            HandleSceneLoad(CurrentScene);
        }

        public void LoadLastSavedLevel()
        {
            HandleSceneLoad(SaveManager.Instance.SaveData.Progress.CurrentScene);
        }

        public AsyncOperation LoadLevelAsync(Scenes scene, GameState newState = GameState.Initial)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(SceneNameMap[scene]);
            CurrentScene = scene;
            return op;
        }

        public void HandleSceneLoad(Scenes newScene, GameState newState = GameState.Playing)
        {
            if (newScene == Scenes.Main_Menu && CurrentScene == Scenes.Main_Menu)
                newScene = Scenes.HALLWAYS;
            DisableAllCanvases();
            gameOverCanvas.SetActive(false);
            loadingScript.sceneToLoad = newScene;
            loadingScript.stateToLoadIn = newState;
            loadingScreen.SetActive(true);
            loadingScript.BeginLoading();

            StopAllSound();
        }

        public void HandleSceneLoaded()
        {
            loadingScreen.SetActive(false);
        }

        #endregion

        #region Game Over & Fade

        private IEnumerator HandleGameOver()
        {
            float fadeInDuration = 1;

            StopAllSound();
            yield return new WaitForSeconds(0.3f);

            gameOverCanvas.SetActive(true);
            var canvasGroup = gameOverCanvas.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;

            yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 0, 1, fadeInDuration));
        }

        private IEnumerator FadeCanvasGroup(CanvasGroup group, float from, float to, float duration)
        {
            float time = 0f;
            while (time < duration)
            {
                float t = time / duration;
                group.alpha = Mathf.Lerp(from, to, t);
                time += Time.deltaTime;
                yield return null;
            }

            group.alpha = to;
        }

        #endregion

        #region Pause & Resume

        private void HandlePause()
        {
            StopAllSound();
            Time.timeScale = 0f;
            pauseMenuCanvas.SetActive(true);
        }

        private void HandleResume()
        {
            if (SoundManager.Instance.IsMusicPlaying) SoundManager.Instance.StopMusic();
            if (SoundManager.Instance.IsSoundPlaying) SoundManager.Instance.StopSound();

            pauseMenuCanvas.SetActive(false);
            Time.timeScale = 1f;
        }

        #endregion

        #region Save System

        public void SaveData(ref SaveData data)
        {
            data.Progress.CurrentScene = CurrentScene;
        }

        public void LoadData(ref SaveData data)
        {
        }

        #endregion

        #region Utility

        private void DisableAllCanvases()
        {
            var allCanvases = GameObject.FindObjectsOfType<Canvas>(true);

            foreach (var canvas in allCanvases)
            {
                if (canvas.gameObject.scene.name != "DontDestroyOnLoad")
                {
                    canvas.gameObject.SetActive(false);
                }
            }

        }
        public void StopAllSound()
        {
            if (SoundManager.Instance.IsMusicPlaying) SoundManager.Instance.StopMusic();
            if (SoundManager.Instance.IsSoundPlaying) SoundManager.Instance.StopSound();
        }

        public void TogglePlayerMovement(bool value)
        {
            var player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                player.GetComponent<PlayerMovement>().ToggleMovement(value);
            }
        }

        #endregion
    }
}