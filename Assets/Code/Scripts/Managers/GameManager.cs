using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Scripts.Managers
{
    public class GameManager : Singleton<GameManager>, IDataPersistence
    {
        private Scenes _currentScene;

        public static event Action<GameState> OnBeforeGameStateChanged;
        public static event Action<GameState> OnAfterGameStateChanged;

        public bool IntroScenePlayed = false;

        private static readonly Dictionary<GameManager.Scenes, string> SceneNameMap = new()
        {
            { GameManager.Scenes.HALLWAYS, "Hallways_Scene" },
            { GameManager.Scenes.ESC_KEY, "ESC_Key_Scene" },
            { GameManager.Scenes.W_KEY, "W_Key_Scene" },
            { GameManager.Scenes.A_KEY, "A_Key_Scene" },
            { GameManager.Scenes.SPACE_KEY, "Space_Key_Scene" },
            { GameManager.Scenes.G_KEY, "G_Key_Scene" },
            { GameManager.Scenes.ARROW_KEYS, "Arrow_Keys_Scene" },
            { GameManager.Scenes.P_KEY, "P_Key_Scene" },
            { GameManager.Scenes.Main_Menu, "Main_Menu_Scene" }
        };

        private GameState State { get; set; }

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
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }

            State = newState;

            OnAfterGameStateChanged?.Invoke(newState);
        }

        // TODO Maryam: Make sure all scripts unsubscribe to events when they are done with them, to avoid memory leaks // in the OnDestroy method or when the state is never coming back to 
        private void HandelInitialState()
        {
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


        public void SaveData(ref SaveData data)
        {
            data.Progress.CurrentScene = _currentScene;
        }

        public void LoadData(ref SaveData data)
        {
        }
    }
}