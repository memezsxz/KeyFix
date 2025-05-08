using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Code.Scripts.Units.Heroes;
using JetBrains.Annotations;
using UnityEditor;
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
        private Scenes previousSceneBeforeVictory;

        public static event Action<GameState> OnBeforeGameStateChanged;
        public static event Action<GameState> OnAfterGameStateChanged;

        // private bool IntroScenePlayed = false;

        [SerializeField] private GameObject gameOverCanvas;
        [SerializeField] private GameObject pauseMenuCanvas;
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private GameObject levelTitleCanvas;
        [SerializeField] private GameObject victoryCanvas;
        [SerializeField] private LoadingManager loadingScript;
        [SerializeField] private LevelCompleteController victoryController;
        [SerializeField] private LevelTitle levelTitleScript;

        private static readonly Dictionary<GameManager.Scenes, string> SceneNameMap = new()
        {
            { GameManager.Scenes.HALLWAYS, "maryam doors test" },
            { GameManager.Scenes.ESC_KEY, "ESC_Key" },
            { GameManager.Scenes.W_KEY, "W_Key" },
            { GameManager.Scenes.A_KEY, "A_Key" },
            { GameManager.Scenes.SPACE_KEY, "Space_Key" },
            { GameManager.Scenes.G_KEY, "G_Key" },
            { GameManager.Scenes.ARROW_KEYS, "Arrow_Keys" },
            { GameManager.Scenes.P_KEY, "P_Key" },
            { GameManager.Scenes.Main_Menu, "Main_Menu" },
            { GameManager.Scenes.INCIDENT, "Incident_Cutscene" },
            { GameManager.Scenes.W_CUTSCENE, "W_Cutscene" },
            { GameManager.Scenes.A_CUTSCENE, "A_Cutscene" },
            { GameManager.Scenes.ARROW_CUTSCENE, "Arrow_Cutscene" },
            { GameManager.Scenes.SPACE_CUTSCENE, "Space_Cutscene" },
            { GameManager.Scenes.P_CUTSCENE, "P_Cutscene" },
            { GameManager.Scenes.GAME_VICTORY_CUTSCENE, "Game_Victory_Cutscene" },
        };

        private bool shouldLoadSaveDataAfterSceneLoad = false;

        private bool isVictory;

        #endregion

        #region Enums

        public enum GameState
        {
            Initial,
            CutScene,
            Playing,
            Paused,
            GameOver,
            Victory,
        }

        [Serializable]
        public enum Scenes
        {
            Main_Menu,
            INCIDENT,
            ESC_KEY,
            HALLWAYS,
            W_KEY,
            W_CUTSCENE,
            A_KEY,
            A_CUTSCENE,
            G_KEY,
            SPACE_KEY,
            SPACE_CUTSCENE,
            ARROW_KEYS,
            ARROW_CUTSCENE,
            P_KEY,
            P_CUTSCENE,
            GAME_VICTORY_CUTSCENE,
        }

        #endregion

        #region Unity Methods

        void Start()
        {
            DebugController.Instance?.AddDebugCommand(new DebugCommand("gm_test", "testing from the game manager", "",
                () => Debug.Log("working in game manager")));


            var sceneName = SceneManager.GetActiveScene().name;
            var match = SceneNameMap.FirstOrDefault(pair => pair.Value == sceneName);

            if (!string.IsNullOrEmpty(match.Value))
            {
                CurrentScene = match.Key;
                ChangeState(GameState.Initial);
            }
            else
            {
                CurrentScene = GameManager.Scenes.HALLWAYS;
                ChangeState(GameState.Playing);

                Debug.LogWarning($"Scene name '{sceneName}' not found in SceneNameMap.");
            }


            DebugController.Instance.RegisterCommand(new DebugCommand(
                "load_level",
                "Loads a specific level by scene name",
                "load_level <SceneName>",
                (args) =>
                {
                    if (args.Length == 0)
                    {
                        Debug.LogWarning("No scene name provided.");
                        return;
                    }

                    string sceneName = args[0];

                    if (Enum.TryParse(typeof(GameManager.Scenes), sceneName, out var scene))
                    {
                        GameManager.Instance.HandleSceneLoad((GameManager.Scenes)scene);
                        Debug.Log($"Jumping to level: {sceneName}");
                    }
                    else
                    {
                        Debug.LogWarning($"Scene '{sceneName}' is not recognized. Check your GameManager.Scenes enum.");
                    }
                }
            ));


            DebugController.Instance.AddDebugCommand(new DebugCommand("win",
                "trigers the victory state for the current level", "win",
                () => ChangeState(GameState.Victory)));
        }

        #endregion

        #region Game State Management

        public void ChangeState(GameState newState)
        {
            // Invalid transitions
            if ((State == GameState.Victory && newState == GameState.GameOver) ||
                (State == GameState.GameOver && newState == GameState.Victory))
            {
                Debug.LogWarning($"Cannot change from {State} to {newState}");
                return;
            }

            if (State == newState && newState != GameState.Initial) return;

            OnBeforeGameStateChanged?.Invoke(newState);

            switch (newState)
            {
                case GameState.Initial:
                    StartCoroutine(HandelInitialState());
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
                case GameState.Victory:
                    if (CurrentScene != Scenes.Main_Menu) HandleVictory();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }

            State = newState;

            OnAfterGameStateChanged?.Invoke(newState);
        }

        private IEnumerator HandelInitialState()
        {
            // print("here");
            if (levelTitleScript == null)
            {
                Debug.LogWarning("Level title script not assigned.");
                yield break;
            }

            // print(CurrentScene + " from inisial state hadel");
            if (!CanPause())
                yield break;


            levelTitleScript.levelName = GetLevelName(CurrentScene);
            levelTitleScript.levelDescription = GetLevelDescription(CurrentScene);
            levelTitleScript.fadeInDuration = 0;
            TogglePlayerMovement(false);

            var canvases = GameObject.FindObjectsOfType<Canvas>()
                .Where(c => c.gameObject.scene.name != "DontDestroyOnLoad").ToList();
            canvases.ForEach(c => c.enabled = (false));

            levelTitleScript.showLevelTitle();

            while (levelTitleScript.gameObject.activeSelf)
            {
                yield return null;
            }
        }

        public bool CanPause()
        {
            if (CurrentScene == Scenes.Main_Menu) return false;
            if (CurrentScene == Scenes.G_KEY) return false;
            if (State == GameState.CutScene) return false;
            if (loadingScreen.activeSelf) return false;
            if (gameOverCanvas.activeSelf) return false;

            return true;
        }

        #endregion

        #region Scene & Level Management

        public void RestartLevel()
        {
            HandleSceneLoad(CurrentScene, GameState.Initial);
        }

        public void LoadLastSavedLevel()
        {
            shouldLoadSaveDataAfterSceneLoad = true;
            HandleSceneLoad(SaveManager.Instance.SaveData.Progress.CurrentScene, GameManager.GameState.Playing);
        }

        public AsyncOperation LoadLevelAsync(Scenes scene, GameState newState = GameState.Initial)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(SceneNameMap[scene]);
            CurrentScene = scene;
            return op;
        }

        public void HandleSceneLoad(Scenes newScene, GameState newState = GameState.Initial)
        {
            if (newScene == Scenes.Main_Menu && CurrentScene == Scenes.Main_Menu)
                newScene = Scenes.HALLWAYS;
            DisableAllCanvases();
            gameOverCanvas.SetActive(false);
            pauseMenuCanvas.SetActive(false);
            loadingScript.sceneToLoad = newScene;
            loadingScript.stateToLoadIn = newState;
            TogglePlayerMovement(false);
            loadingScreen.SetActive(true);
            loadingScript.BeginLoading();

            SoundManager.Instance.StopAllAudio();
        }

        public void HandleSceneLoaded()
        {
            loadingScreen.SetActive(false);

            TogglePlayerMovement(true);

            StartCoroutine(ReapplyBindingsNextFrame());
            if (CurrentScene == Scenes.HALLWAYS && State != GameState.Initial)
            {
                StartCoroutine(SetPlayerPositionNextFrame());
            }
        }

        private IEnumerator WaitForSceneObjects()
        {
            // Wait a few frames to ensure all scene objects are loaded and initialized
            yield return new WaitForSeconds(0.1f); // or yield return null; multiple times if needed

            var canvases = GameObject.FindObjectsOfType<Canvas>()
                .Where(c => c.gameObject.scene.name != "DontDestroyOnLoad").ToList();

            if (CurrentScene == Scenes.HALLWAYS && State == GameState.Initial)
            {
                TogglePlayerMovement(false);
                canvases.ForEach(c => c.enabled = false);

                Debug.Log($"[HandleSceneLoaded] In Hallways - Found {canvases.Count} canvases.");

                if (levelTitleScript != null)
                {
                    levelTitleScript.levelName = "Hallways";
                    levelTitleScript.levelDescription = "Find the doors for each malfunctioning key";
                    levelTitleScript.showLevelTitle();
                }
                else
                {
                    Debug.LogWarning("levelTitleScript is not assigned.");
                }
            }
        }

        private IEnumerator ReapplyBindingsNextFrame()
        {
            yield return null; // wait one frame to ensure PlayerInput is initialized
            SaveManager.Instance.LoadPlayerBindings();
        }

        private IEnumerator SetPlayerPositionNextFrame()
        {
            yield return null; // wait one frame to ensure PlayerInput is initialized
            if (isVictory)
            {
                isVictory = false; // reset it now
                HallwaysManager.Instance.HandleVictory(previousSceneBeforeVictory);
                // Debug.Log("Set player position next to exit door of: " + previousSceneBeforeVictory);
            }
            else if (State != GameState.Initial) SaveManager.Instance.LoadHallwayPosition();

            // print("set player position");
            HallwaysManager.Instance.UpdateInteractablesUI(SaveManager.Instance.SaveData.Progress.CollectablesCount);
        }

        #endregion

        #region Game Over & Fade

        private IEnumerator HandleGameOver()
        {
            float fadeInDuration = 1;

            SoundManager.Instance.StopAllAudio();
            yield return new WaitForSeconds(0.3f);

            gameOverCanvas.SetActive(true);
            var canvasGroup = gameOverCanvas.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;

            yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 0, 1, fadeInDuration));
        }

        #endregion

        #region Pause & Resume

        private void HandlePause()
        {
            SoundManager.Instance.StopAllAudio();
            Time.timeScale = 0f;
            pauseMenuCanvas.SetActive(true);
        }

        private void HandleResume()
        {
            if (SoundManager.Instance.IsMusicPlaying) SoundManager.Instance.StopMusic();
            if (SoundManager.Instance.IsSoundPlaying) SoundManager.Instance.StopSound();

            pauseMenuCanvas.SetActive(false);
            TogglePlayerMovement(true);
            Time.timeScale = 1f;
        }

        #endregion

        #region Vectory

        private void HandleVictory()
        {
            isVictory = true;
            previousSceneBeforeVictory = CurrentScene;
            SoundManager.Instance.StopAllAudio();
            DisableAllCanvases();
            TogglePlayerMovement(false);
            victoryController.ShowCompleteScene(); // automatically redirects to HALLWAYS


            var playerBindingManager = GameObject.FindObjectOfType<PlayerBindingManage>();

            if (playerBindingManager == null)
            {
                Debug.LogWarning("Cannot find player bindings manager");
                return;
            }

            var pbm = playerBindingManager.GetComponent<PlayerBindingManage>();
            if (pbm)
            {
                switch (CurrentScene)
                {
                    case Scenes.W_KEY:
                    {
                        pbm.EnableBinding("Move", "up");
                        break;
                    }
                    case Scenes.A_KEY:
                    {
                        pbm.EnableBinding("Move", "left");
                        break;
                    }
                    case Scenes.SPACE_KEY:
                    {
                        pbm.EnableBinding("Jump");
                        break;
                    }
                }
            }

            SaveManager.Instance.SaveData.Progress.RepairedKeys.Add(CurrentScene);
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

        #region SenceManager Events

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (shouldLoadSaveDataAfterSceneLoad)
            {
                shouldLoadSaveDataAfterSceneLoad = false;
                SaveManager.Instance.LoadGame(SaveManager.Instance.SaveData.Meta.SaveName);
                Debug.Log("Save data loaded after loading last saved level.");
            }
            //
            // if (isVictory)
            // {
            //     isVictory = false;
            //     HallwaysManager.Instance.HandleVictory(CurrentScene);
            //     Debug.Log("call to set the player position next to exit door.");
            // }
        }

        #endregion

        #region Utility

        public IEnumerator FadeCanvasGroup(CanvasGroup group, float from, float to, float duration)
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

        #endregion

        #region Player

        public void TogglePlayerMovement(bool value)
        {
            FindPlayer()?.GetComponent<PlayerMovement>().ToggleMovement(value);
        }

        public Transform GetPlayerTransform()
        {
            return FindPlayer()?.GetComponent<Transform>();
        }

        public void MovePlayerTo(Vector3 targetPosition, Quaternion targetRotation)
        {
            var playerTransform = GetPlayerTransform();
            var controller = playerTransform.GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = false;
                playerTransform.position = targetPosition;
                playerTransform.rotation = targetRotation;
                controller.enabled = true;
            }
        }


        [CanBeNull]
        private GameObject FindPlayer()
        {
            return GameObject.FindGameObjectWithTag("Player");
        }

        public void IncrementCollectables()
        {
            Collector collector = FindPlayer()?.GetComponent<Collector>();
            collector.AddCollectable();
            HallwaysManager.Instance.UpdateInteractablesUI(collector.CollectablesCount);
        }


        public int GetCollectablesCount()
        {
            return FindPlayer()?.GetComponent<Collector>().CollectablesCount ?? 0;
        }

        #endregion


        #region Level Title

        private bool ShouldShowLevelTitle(Scenes scene, GameState state)
        {
            return state == GameState.Initial;
            return scene == Scenes.HALLWAYS;
            // Add other conditions here if needed
        }

        private string GetLevelName(GameManager.Scenes scene)
        {
            return scene switch
            {
                Scenes.HALLWAYS => "Hallways",
                _ => "Level"
            };
        }

        private string GetLevelDescription(GameManager.Scenes scene)
        {
            return scene switch
            {
                Scenes.HALLWAYS => "Find the doors for each malfunctioning key",
                _ => "Get ready"
            };
        }

        public void HandleLevelTitleDone()
        {
            var canvases = GameObject.FindObjectsOfType<Canvas>()
                .Where(c => c.gameObject.scene.name != "DontDestroyOnLoad").ToList();
            canvases.ForEach(c => c.enabled = (true));

            ChangeState(GameState.Playing);
        }

        #endregion
    }
}
//
// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using Code.Scripts.Units.Heroes;
// using JetBrains.Annotations;
// using UnityEditor;
// using UnityEngine;
// using UnityEngine.SceneManagement;
// using UnityEngine.UI;
//
// namespace Code.Scripts.Managers
// {
//     public class GameManager : Singleton<GameManager>, IDataPersistence
//     {
//         #region Enums
//
//         public enum GameState
//         {
//             Initial,
//             CutScene,
//             Playing,
//             Paused,
//             GameOver,
//             Victory,
//         }
//
//         [Serializable]
//         public enum Scenes
//         {
//             Main_Menu,
//             INCIDENT,
//             ESC_KEY,
//             HALLWAYS,
//             W_KEY,
//             W_CUTSCENE,
//             A_KEY,
//             A_CUTSCENE,
//             G_KEY,
//             SPACE_KEY,
//             SPACE_CUTSCENE,
//             ARROW_KEYS,
//             ARROW_CUTSCENE,
//             P_KEY,
//             P_CUTSCENE,
//             GAME_VICTORY_CUTSCENE,
//         }
//
//         #endregion
//
//         #region Fields
//
//         [Header("UI References")] [SerializeField]
//         private GameObject gameOverCanvas;
//
//         [SerializeField] private GameObject pauseMenuCanvas;
//         [SerializeField] private GameObject loadingScreen;
//         [SerializeField] private GameObject levelTitleCanvas;
//         [SerializeField] private GameObject victoryCanvas;
//
//         [Header("Manager References")] [SerializeField]
//         private LoadingManager loadingScript;
//
//         [SerializeField] private LevelCompleteController victoryController;
//         [SerializeField] private LevelTitle levelTitleScript;
//
//         private static readonly Dictionary<Scenes, string> SceneNameMap = new()
//         {
//             { Scenes.HALLWAYS, "maryam doors test" },
//             { Scenes.ESC_KEY, "ESC_Key" },
//             { Scenes.W_KEY, "W_Key" },
//             { Scenes.A_KEY, "A_Key" },
//             { Scenes.SPACE_KEY, "Space_Key" },
//             { Scenes.G_KEY, "G_Key" },
//             { Scenes.ARROW_KEYS, "Arrow_Keys" },
//             { Scenes.P_KEY, "P_Key" },
//             { Scenes.Main_Menu, "Main_Menu" },
//             { Scenes.INCIDENT, "Incident_Cutscene" },
//             { Scenes.W_CUTSCENE, "W_Cutscene" },
//             { Scenes.A_CUTSCENE, "A_Cutscene" },
//             { Scenes.SPACE_CUTSCENE, "Space_Cutscene" },
//             { Scenes.P_CUTSCENE, "P_Cutscene" },
//             { Scenes.GAME_VICTORY_CUTSCENE, "Game_Victory_Cutscene" },
//         };
//
//         private bool shouldLoadSaveDataAfterSceneLoad = false;
//         private bool isVictory;
//         private Scenes previousSceneBeforeVictory;
//
//         #endregion
//
//         #region Properties
//
//         public Scenes CurrentScene { get; private set; }
//         public GameState State { get; private set; }
//
//         #endregion
//
//         #region Events
//
//         public static event Action<GameState> OnBeforeGameStateChanged;
//         public static event Action<GameState> OnAfterGameStateChanged;
//
//         #endregion
//
//         #region Unity Lifecycle
//
//         private void Start()
//         {
//             InitializeDebugCommands();
//             InitializeCurrentScene();
//             ChangeState(GameState.Initial);
//         }
//
//         private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
//         private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;
//
//         #endregion
//
//         #region Initialization
//
//         private void InitializeDebugCommands()
//         {
//             DebugController.Instance?.AddDebugCommand(new DebugCommand(
//                 "gm_test",
//                 "testing from the game manager",
//                 "",
//                 () => Debug.Log("working in game manager")));
//
//             DebugController.Instance.RegisterCommand(new DebugCommand(
//                 "load_level",
//                 "Loads a specific level by scene name",
//                 "load_level <SceneName>",
//                 HandleLoadLevelCommand));
//
//             DebugController.Instance.AddDebugCommand(new DebugCommand(
//                 "win",
//                 "triggers the victory state for the current level",
//                 "win",
//                 () => ChangeState(GameState.Victory)));
//         }
//
//         private void InitializeCurrentScene()
//         {
//             var sceneName = SceneManager.GetActiveScene().name;
//             var match = SceneNameMap.FirstOrDefault(pair => pair.Value == sceneName);
//
//             CurrentScene = !string.IsNullOrEmpty(match.Value)
//                 ? match.Key
//                 : Scenes.HALLWAYS;
//
//             if (string.IsNullOrEmpty(match.Value))
//             {
//                 Debug.LogWarning($"Scene name '{sceneName}' not found in SceneNameMap.");
//             }
//         }
//
//         private void HandleLoadLevelCommand(string[] args)
//         {
//             if (args.Length == 0)
//             {
//                 Debug.LogWarning("No scene name provided.");
//                 return;
//             }
//
//             string sceneName = args[0];
//
//             if (Enum.TryParse(typeof(Scenes), sceneName, out var scene))
//             {
//                 HandleSceneLoad((Scenes)scene);
//                 Debug.Log($"Jumping to level: {sceneName}");
//             }
//             else
//             {
//                 Debug.LogWarning($"Scene '{sceneName}' is not recognized. Check your GameManager.Scenes enum.");
//             }
//         }
//
//         #endregion
//
//         #region State Management
//
//         public void ChangeState(GameState newState)
//         {
//             if (!IsValidStateTransition(newState)) return;
//             if (State == newState && newState != GameState.Initial) return;
//
//             OnBeforeGameStateChanged?.Invoke(newState);
//             State = newState;
//
//             try
//             {
//                 HandleNewState(newState);
//             }
//             catch (Exception ex)
//             {
//                 Debug.LogError($"Error handling state {newState}: {ex.Message}");
//             }
//
//             OnAfterGameStateChanged?.Invoke(newState);
//         }
//
//         private bool IsValidStateTransition(GameState newState)
//         {
//             if ((State == GameState.Victory && newState == GameState.GameOver) ||
//                 (State == GameState.GameOver && newState == GameState.Victory))
//             {
//                 Debug.LogWarning($"Invalid state transition from {State} to {newState}");
//                 return false;
//             }
//
//             return true;
//         }
//
//         private void HandleNewState(GameState newState)
//         {
//             switch (newState)
//             {
//                 case GameState.Initial:
//                     StartCoroutine(HandleInitialState());
//                     break;
//                 case GameState.Paused:
//                     if (CurrentScene != Scenes.Main_Menu) HandlePause();
//                     break;
//                 case GameState.Playing:
//                     if (CurrentScene != Scenes.Main_Menu) HandleResume();
//                     break;
//                 case GameState.CutScene:
//                     break;
//                 case GameState.GameOver:
//                     if (CurrentScene != Scenes.Main_Menu) StartCoroutine(HandleGameOver());
//                     break;
//                 case GameState.Victory:
//                     if (CurrentScene != Scenes.Main_Menu) HandleVictory();
//                     break;
//                 default:
//                     throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
//             }
//         }
//
//         public bool CanPause()
//         {
//             return CurrentScene != Scenes.Main_Menu &&
//                    CurrentScene != Scenes.G_KEY &&
//                    State != GameState.CutScene &&
//                    !loadingScreen.activeSelf &&
//                    !gameOverCanvas.activeSelf;
//         }
//
//         #endregion
//
//         #region Scene Management
//
//         public void RestartLevel() => HandleSceneLoad(CurrentScene, GameState.Initial);
//
//         public void LoadLastSavedLevel()
//         {
//             shouldLoadSaveDataAfterSceneLoad = true;
//             HandleSceneLoad(SaveManager.Instance.SaveData.Progress.CurrentScene, GameState.Playing);
//         }
//
//         public AsyncOperation LoadLevelAsync(Scenes scene, GameState newState = GameState.Initial)
//         {
//             AsyncOperation op = SceneManager.LoadSceneAsync(SceneNameMap[scene]);
//             CurrentScene = scene;
//             return op;
//         }
//
//         public void HandleSceneLoad(Scenes newScene, GameState newState = GameState.Initial)
//         {
//             if (newScene == Scenes.Main_Menu && CurrentScene == Scenes.Main_Menu)
//                 newScene = Scenes.HALLWAYS;
//
//             PrepareForSceneLoad(newScene, newState);
//             loadingScript.BeginLoading();
//         }
//
//         private void PrepareForSceneLoad(Scenes newScene, GameState newState)
//         {
//             DisableAllCanvases();
//             gameOverCanvas.SetActive(false);
//             pauseMenuCanvas.SetActive(false);
//             loadingScript.sceneToLoad = newScene;
//             loadingScript.stateToLoadIn = newState;
//             TogglePlayerMovement(false);
//             loadingScreen.SetActive(true);
//             SoundManager.Instance.StopAllAudio();
//         }
//
//         public void HandleSceneLoaded()
//         {
//             loadingScreen.SetActive(false);
//             TogglePlayerMovement(true);
//
//             StartCoroutine(ReapplyBindingsNextFrame());
//
//             if (CurrentScene == Scenes.HALLWAYS && State != GameState.Initial)
//             {
//                 StartCoroutine(SetPlayerPositionNextFrame());
//             }
//         }
//
//         private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
//         {
//             if (shouldLoadSaveDataAfterSceneLoad)
//             {
//                 shouldLoadSaveDataAfterSceneLoad = false;
//                 SaveManager.Instance.LoadGame(SaveManager.Instance.SaveData.Meta.SaveName);
//                 Debug.Log("Save data loaded after loading last saved level.");
//             }
//         }
//
//         #endregion
//
//         #region Coroutines
//
//         private IEnumerator HandleInitialState()
//         {
//             if (levelTitleScript == null)
//             {
//                 Debug.LogWarning("Level title script not assigned.");
//                 yield break;
//             }
//
//             if (!CanPause()) yield break;
//
//             SetupLevelTitle();
//             DisableSceneCanvases();
//             yield return ShowLevelTitle();
//         }
//
//         private IEnumerator ShowLevelTitle()
//         {
//             levelTitleScript.showLevelTitle();
//             while (levelTitleScript.gameObject.activeSelf)
//             {
//                 yield return null;
//             }
//         }
//
//         private IEnumerator HandleGameOver()
//         {
//             float fadeInDuration = 1;
//             SoundManager.Instance.StopAllAudio();
//             yield return new WaitForSeconds(0.3f);
//
//             gameOverCanvas.SetActive(true);
//             var canvasGroup = gameOverCanvas.GetComponent<CanvasGroup>();
//             canvasGroup.alpha = 0;
//
//             yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 0, 1, fadeInDuration));
//         }
//
//         private IEnumerator ReapplyBindingsNextFrame()
//         {
//             yield return null;
//             SaveManager.Instance.LoadPlayerBindings();
//         }
//
//         private IEnumerator SetPlayerPositionNextFrame()
//         {
//             yield return null;
//             if (isVictory)
//             {
//                 isVictory = false;
//                 HallwaysManager.Instance.HandleVictory(previousSceneBeforeVictory);
//             }
//             else if (State != GameState.Initial)
//             {
//                 SaveManager.Instance.LoadHallwayPosition();
//             }
//
//             HallwaysManager.Instance.UpdateInteractablesUI(SaveManager.Instance.SaveData.Progress.CollectablesCount);
//         }
//
//         public IEnumerator FadeCanvasGroup(CanvasGroup group, float from, float to, float duration)
//         {
//             float time = 0f;
//             while (time < duration)
//             {
//                 float t = time / duration;
//                 group.alpha = Mathf.Lerp(from, to, t);
//                 time += Time.deltaTime;
//                 yield return null;
//             }
//
//             group.alpha = to;
//         }
//
//         #endregion
//
//         #region UI Management
//
//         private void SetupLevelTitle()
//         {
//             levelTitleScript.levelName = GetLevelName(CurrentScene);
//             levelTitleScript.levelDescription = GetLevelDescription(CurrentScene);
//             levelTitleScript.fadeInDuration = 0;
//         }
//
//         private void DisableSceneCanvases()
//         {
//             var canvases = GameObject.FindObjectsOfType<Canvas>()
//                 .Where(c => c.gameObject.scene.name != "DontDestroyOnLoad").ToList();
//             canvases.ForEach(c => c.enabled = false);
//         }
//
//         private void DisableAllCanvases()
//         {
//             var allCanvases = GameObject.FindObjectsOfType<Canvas>(true);
//             foreach (var canvas in allCanvases)
//             {
//                 if (canvas.gameObject.scene.name != "DontDestroyOnLoad")
//                 {
//                     canvas.gameObject.SetActive(false);
//                 }
//             }
//         }
//
//         public void HandleLevelTitleDone()
//         {
//             var canvases = GameObject.FindObjectsOfType<Canvas>()
//                 .Where(c => c.gameObject.scene.name != "DontDestroyOnLoad").ToList();
//             canvases.ForEach(c => c.enabled = true);
//
//             TogglePlayerMovement(true);
//         }
//
//         #endregion
//
//         #region Game Flow
//
//         private void HandlePause()
//         {
//             SoundManager.Instance.StopAllAudio();
//             Time.timeScale = 0f;
//             pauseMenuCanvas.SetActive(true);
//         }
//
//         private void HandleResume()
//         {
//             SoundManager.Instance.StopAllAudio();
//             pauseMenuCanvas.SetActive(false);
//             TogglePlayerMovement(true);
//             Time.timeScale = 1f;
//         }
//
//         private void HandleVictory()
//         {
//             isVictory = true;
//             previousSceneBeforeVictory = CurrentScene;
//             SoundManager.Instance.StopAllAudio();
//             DisableAllCanvases();
//             TogglePlayerMovement(false);
//             victoryController.ShowCompleteScene();
//
//             EnableKeyBinding();
//             SaveVictoryProgress();
//         }
//
//         private void EnableKeyBinding()
//         {
//             var playerBindingManager = FindObjectOfType<PlayerBindingManage>();
//             if (playerBindingManager == null)
//             {
//                 Debug.LogWarning("Cannot find player bindings manager");
//                 return;
//             }
//
//             var pbm = playerBindingManager.GetComponent<PlayerBindingManage>();
//             if (!pbm) return;
//
//             switch (CurrentScene)
//             {
//                 case Scenes.W_KEY:
//                     pbm.EnableBinding("Move", "up");
//                     break;
//                 case Scenes.A_KEY:
//                     pbm.EnableBinding("Move", "left");
//                     break;
//                 case Scenes.SPACE_KEY:
//                     pbm.EnableBinding("Jump");
//                     break;
//             }
//         }
//
//         private void SaveVictoryProgress()
//         {
//             SaveManager.Instance.SaveData.Progress.RepairedKeys.Add(CurrentScene);
//         }
//
//         #endregion
//
//         #region Player Management
//
//         public void TogglePlayerMovement(bool value)
//         {
//             FindPlayer()?.GetComponent<PlayerMovement>().ToggleMovement(value);
//         }
//
//         public Transform GetPlayerTransform()
//         {
//             return FindPlayer()?.GetComponent<Transform>();
//         }
//
//         public void MovePlayerTo(Vector3 targetPosition, Quaternion targetRotation)
//         {
//             var playerTransform = GetPlayerTransform();
//             if (playerTransform == null) return;
//
//             var controller = playerTransform.GetComponent<CharacterController>();
//             if (controller != null)
//             {
//                 controller.enabled = false;
//                 playerTransform.position = targetPosition;
//                 playerTransform.rotation = targetRotation;
//                 controller.enabled = true;
//             }
//         }
//
//         [CanBeNull]
//         private GameObject FindPlayer()
//         {
//             return GameObject.FindGameObjectWithTag("Player");
//         }
//
//         public void IncrementCollectables()
//         {
//             var collector = FindPlayer()?.GetComponent<Collector>();
//             if (collector == null) return;
//
//             collector.AddCollectable();
//             HallwaysManager.Instance.UpdateInteractablesUI(collector.CollectablesCount);
//         }
//
//         public int GetCollectablesCount()
//         {
//             return FindPlayer()?.GetComponent<Collector>().CollectablesCount ?? 0;
//         }
//
//         #endregion
//
//
//         #region Level Info
//
//         private string GetLevelName(Scenes scene)
//         {
//             return scene switch
//             {
//                 Scenes.HALLWAYS => "Hallways",
//                 _ => "Level"
//             };
//         }
//
//         private string GetLevelDescription(Scenes scene)
//         {
//             return scene switch
//             {
//                 Scenes.HALLWAYS => "Find the doors for each malfunctioning key",
//                 _ => "Get ready"
//             };
//         }
//
//         #endregion
//
//         #region Save System
//
//         public void SaveData(ref SaveData data)
//         {
//             data.Progress.CurrentScene = CurrentScene;
//         }
//
//         public void LoadData(ref SaveData data)
//         {
//             // Implementation if needed
//         }
//
//         #endregion
//     }
// }