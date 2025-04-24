using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Code.Scripts.Managers
{
    public class GameManager : Singleton<GameManager>, IDataPersistence
    {
        private Scenes _currentScene;

        public static event Action<GameState> OnBeforeGameStateChanged;
        public static event Action<GameState> OnAfterGameStateChanged;

        private bool IntroScenePlayed = false;
        [SerializeField] GameObject gameOverCanvas;
        [SerializeField] GameObject pauseMenuCanvas;

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

        public GameState State { get; private set; }

        void Start()
        {
            DebugController.Instance?.AddDebugCommand(new DebugCommand("gm_test", "testing from the game manager", "",
                () => Debug.Log("working in game manager"))); // command format hint in case of args
            ChangeState(GameState.Initial);
        }


        /// <summary>
        /// Change the state of the game play
        /// </summary>
        /// <param name="newState">The new state</param>
        /// <exception cref="ArgumentOutOfRangeException">If the new state is not valid</exception>
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
                    HandlePause();
                    break;
                case GameState.Playing:
                    HandleResume();
                    break;
                case GameState.CutScene:
                    break;
                case GameState.GameOver:
                    StartCoroutine( HandleGameOver());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }

            State = newState;

            OnAfterGameStateChanged?.Invoke(newState);
        }

        // TODO Maryam: Make sure all scripts unsubscribe to events when they are done with them, to avoid memory leaks // in the OnDestroy method or when the state is never coming back to 
        private void HandelInitialState()
        {
            // inisialize the player in the right spot
            
        }


        public enum GameState
        {
            /// <summary>
            /// load the data on OnBeforeGameStateChanged command
            /// </summary>
            Initial,
            CutScene,
            Playing,
            Paused,
            GameOver,
        }

        public void RestartLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            ChangeState(GameState.Playing);
        }

        public void LoadLastSavedLevel()
        {
            _currentScene = SaveManager.Instance.SaveData.Progress.CurrentScene;
            SceneManager.LoadScene(SceneNameMap[_currentScene]);
            ChangeState(GameState.Playing);
        }

        public void LoadLevel(Scenes scene, GameState newState = GameState.Initial)
        {
            SceneManager.LoadScene(SceneNameMap[scene]);
            _currentScene = scene;
            ChangeState(newState); // should check what state this should change to  
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
            // any other scenes that we might want
        }


        public AsyncOperation LoadLevelAsync(Scenes scene, GameState newState = GameState.Initial)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(SceneNameMap[scene]);
            _currentScene = scene;
            ChangeState(newState);
            return op;
        }

        public void SaveData(ref SaveData data)
        {
            data.Progress.CurrentScene = _currentScene;
        }

        public void LoadData(ref SaveData data)
        {
        }

        private IEnumerator HandleGameOver()
        {
            if (SoundManager.Instance.IsMusicPlaying) SoundManager.Instance.StopMusic();
            if (SoundManager.Instance.IsSoundPlaying) SoundManager.Instance.StopSound();

            yield return new WaitForSeconds(0.3f);
            gameOverCanvas.SetActive(true);
        }
        private void HandlePause()
        {
            if (SoundManager.Instance.IsMusicPlaying) SoundManager.Instance.StopMusic();
            if (SoundManager.Instance.IsSoundPlaying) SoundManager.Instance.StopSound();
            Time.timeScale = 0f; // Resume game time

            pauseMenuCanvas.SetActive(true);
        }
        
        private void HandleResume()
        {
            if (SoundManager.Instance.IsMusicPlaying) SoundManager.Instance.StopMusic();
            if (SoundManager.Instance.IsSoundPlaying) SoundManager.Instance.StopSound();
            
            pauseMenuCanvas.SetActive(false);
            Time.timeScale = 1f; // Resume game time
        }

    }
}