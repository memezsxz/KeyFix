using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Code.Scripts.Units.Heroes;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Scripts.Managers
{
    public class GameManager : Singleton<GameManager>, IDataPersistence
    {
        #region Unity Methods

        private void Start()
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
                CurrentScene = Scenes.HALLWAYS;
                ChangeState(GameState.Playing);

                Debug.LogWarning($"Scene name '{sceneName}' not found in SceneNameMap.");
            }


            DebugController.Instance?.RegisterCommand(new DebugCommand(
                "load_level",
                "Loads a specific level by scene name",
                "load_level <SceneName>",
                args =>
                {
                    if (args.Length == 0)
                    {
                        Debug.LogWarning("No scene name provided.");
                        return;
                    }

                    var sceneName = args[0];

                    if (Enum.TryParse(typeof(Scenes), sceneName, out var scene))
                    {
                        Instance.HandleSceneLoad((Scenes)scene);
                        Debug.Log($"Jumping to level: {sceneName}");
                    }
                    else
                    {
                        Debug.LogWarning($"Scene '{sceneName}' is not recognized. Check your GameManager.Scenes enum.");
                    }
                }
            ));


            DebugController.Instance?.AddDebugCommand(new DebugCommand("win",
                "trigers the victory state for the current level", "win",
                () => ChangeState(GameState.Victory)));
            DebugController.Instance?.AddDebugCommand(new DebugCommand("current_state",
                "prints the current game state", "current_state",
                () => Debug.Log(State)));

            DebugController.Instance?.AddDebugCommand(new DebugCommand("simi_win",
                "simulates a win", "simi_win",
                () =>
                {
                    SaveManager.Instance.SaveData.Progress.RepairedKeys.AddRange(new List<Scenes>()
                        { Scenes.W_KEY, Scenes.A_KEY, Scenes.SPACE_KEY, Scenes.G_KEY, Scenes.ARROW_KEYS });
                    CurrentScene = Scenes.ARROW_KEYS;
                    var pbm = GetPlayerTransform().gameObject.GetComponent<PlayerBindingManage>();

                    pbm?.EnableBinding("Move", "up");
                    pbm?.EnableBinding("Move", "left");
                    pbm?.EnableBinding("Jump");

                    ChangeState(GameState.Victory);
                }));
        }

        #endregion


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

        private static readonly Dictionary<Scenes, string> SceneNameMap = new()
        {
            { Scenes.HALLWAYS, "Hallways" },
            { Scenes.ESC_KEY, "ESC_Key" },
            { Scenes.W_KEY, "W_Key" },
            { Scenes.A_KEY, "A_Key" },
            { Scenes.SPACE_KEY, "Space_Key" },
            { Scenes.G_KEY, "G_Key" },
            { Scenes.ARROW_KEYS, "Arrow_Keys" },
            { Scenes.P_KEY, "P_Key" },
            { Scenes.Main_Menu, "Main_Menu" },
            { Scenes.INCIDENT, "Incident_Cutscene" },
            { Scenes.GAME_VICTORY_CUTSCENE, "Game_Victory_Cutscene" }
        };

        private bool shouldLoadSaveDataAfterSceneLoad;

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
            Victory
        }

        [Serializable]
        public enum Scenes
        {
            Main_Menu,
            INCIDENT,
            ESC_KEY,
            HALLWAYS,
            W_KEY,
            A_KEY,
            G_KEY,
            SPACE_KEY,
            ARROW_KEYS,
            P_KEY,
            GAME_VICTORY_CUTSCENE
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

            if (State == newState && newState != GameState.Initial && newState != GameState.CutScene) return;

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
                    if (CurrentScene != Scenes.Main_Menu) HandleCutscene();
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
            if (levelTitleScript == null)
            {
                Debug.LogWarning("Level title script not assigned.");
                yield break;
            }

            // print(CurrentScene + " from inisial state hadel");
            if (CurrentScene == Scenes.Main_Menu) yield break;
            if (CurrentScene == Scenes.G_KEY) yield break;
            if (CurrentScene == Scenes.ESC_KEY) yield break;
            if (CurrentScene == Scenes.INCIDENT) yield break;
            if (CurrentScene == Scenes.GAME_VICTORY_CUTSCENE) yield break;

            // print("shoing title");

            levelTitleScript.levelName = GetLevelName(CurrentScene);
            levelTitleScript.levelDescription = GetLevelDescription(CurrentScene);
            levelTitleScript.fadeInDuration = 0;
            TogglePlayerMovement(false);

            var canvases = FindObjectsOfType<Canvas>()
                .Where(c => c.gameObject.scene.name != "DontDestroyOnLoad").ToList();
            canvases.ForEach(c => c.enabled = false);

            levelTitleScript.showLevelTitle();

            while (levelTitleScript.gameObject.activeSelf) yield return null;
        }

        private void HandleCutscene()
        {
            // if (CurrentScene == Scenes.GAME_VICTORY_CUTSCENE)

            print(CurrentScene);

            if (CurrentScene == Scenes.G_KEY) SaveManager.Instance.SaveData.Progress.RepairedKeys.Add(CurrentScene);

            var nextScene = CurrentScene == Scenes.INCIDENT ? Scenes.ESC_KEY : Scenes.HALLWAYS;

            if (nextScene == Scenes.HALLWAYS)
            {
                print(CurrentScene == Scenes.G_KEY ? GameState.Playing : GameState.Initial);
                if (CurrentScene == Scenes.G_KEY)
                {
                    isVictory = true;
                    previousSceneBeforeVictory = CurrentScene;
                }

                var previousScene = CurrentScene;
                CurrentScene = nextScene;

                HandleSceneLoad(Scenes.HALLWAYS, previousScene == Scenes.G_KEY ? GameState.Playing : GameState.Initial);
            }
            else
            {
                DisableAllCanvases();
                gameOverCanvas.SetActive(false);
                pauseMenuCanvas.SetActive(false);
                TogglePlayerMovement(false);
                SoundManager.Instance.StopAllAudio();
                print(SceneNameMap.GetValueOrDefault(nextScene));
                SceneManager.LoadScene(SceneNameMap.GetValueOrDefault(nextScene));
                CurrentScene = nextScene;
                ChangeState(GameState.Initial);
            }
        }

        public bool CanPause()
        {
            print(CurrentScene == Scenes.Main_Menu);
            print(loadingScreen.activeSelf);
            print(gameOverCanvas.activeSelf);
            print(victoryCanvas.activeSelf);

            if (CurrentScene == Scenes.Main_Menu) return false;
            if (CurrentScene == Scenes.G_KEY) return false;
            if (CurrentScene == Scenes.ESC_KEY) return false;
            if (CurrentScene == Scenes.INCIDENT) return false;
            if (CurrentScene == Scenes.GAME_VICTORY_CUTSCENE) return false;
            if (State == GameState.CutScene) return false;
            if (State == GameState.Initial) return false;
            if (loadingScreen.activeSelf) return false;
            if (gameOverCanvas.activeSelf) return false;

            return true;
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

        #region Vectory

        private void HandleVictory()
        {
            isVictory = true;
            previousSceneBeforeVictory = CurrentScene;
            SoundManager.Instance.StopAllAudio();
            DisableAllCanvases();
            TogglePlayerMovement(false);

            victoryController.ShowCompleteScene(); // automatically redirects to HALLWAYS


            var playerBindingManager = FindObjectOfType<PlayerBindingManage>();

            if (playerBindingManager == null)
            {
                Debug.LogWarning("Cannot find player bindings manager");
                return;
            }

            var pbm = playerBindingManager.GetComponent<PlayerBindingManage>();
            if (pbm)
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

            SaveManager.Instance.SaveData.Progress.RepairedKeys.Add(CurrentScene);
        }

        #endregion

        #region Scene & Level Management

        public void RestartLevel()
        {
            HandleSceneLoad(CurrentScene);
        }

        public void LoadLastSavedLevel()
        {
            // print("here");
            // SaveManager.Instance.LoadGame(SaveManager.Instance.SaveSlotName);
            shouldLoadSaveDataAfterSceneLoad = true;
            HandleSceneLoad(SaveManager.Instance.SaveData.Progress.CurrentScene, GameState.Playing);
        }

        public void GoToHallways()
        {
            HandleSceneLoad(Scenes.HALLWAYS);
        }

        public AsyncOperation LoadLevelAsync(Scenes scene, GameState newState = GameState.Initial)
        {
            var op = SceneManager.LoadSceneAsync(SceneNameMap[scene]);
            CurrentScene = scene;
            return op;
        }

        public void HandleSceneLoad(Scenes newScene, GameState newState = GameState.Initial)
        {
            if (new List<Scenes>
                    { Scenes.W_KEY, Scenes.A_KEY, Scenes.SPACE_KEY, Scenes.G_KEY, Scenes.ARROW_KEYS, Scenes.P_KEY }
                .All(k => SaveManager.Instance.SaveData.Progress.RepairedKeys.Contains(k)))
            {
                SceneManager.LoadScene(SceneNameMap[Scenes.GAME_VICTORY_CUTSCENE]);
                ChangeState(GameState.Initial);
                return;
            }

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
                StartCoroutine(SetPlayerPositionNextFrame());
        }

        private IEnumerator WaitForSceneObjects()
        {
            // Wait a few frames to ensure all scene objects are loaded and initialized
            yield return new WaitForSeconds(0.1f); // or yield return null; multiple times if needed

            var canvases = FindObjectsOfType<Canvas>()
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
                Debug.Log("Set player position next to exit door of: " + previousSceneBeforeVictory);
            }
            else if (State != GameState.Initial)
            {
                SaveManager.Instance.LoadHallwayPosition();
            }

            // print("set player position");
            HallwaysManager.Instance.UpdateInteractablesUI(SaveManager.Instance.SaveData.Progress.CollectablesCount);
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
            var time = 0f;
            while (time < duration)
            {
                var t = time / duration;
                group.alpha = Mathf.Lerp(from, to, t);
                time += Time.deltaTime;
                yield return null;
            }

            group.alpha = to;
        }

        private void DisableAllCanvases()
        {
            var allCanvases = FindObjectsOfType<Canvas>(true);

            foreach (var canvas in allCanvases)
                if (canvas.gameObject.scene.name != "DontDestroyOnLoad")
                    canvas.gameObject.SetActive(false);
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
            var collector = FindPlayer()?.GetComponent<Collector>();
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

        private string GetLevelName(Scenes scene)
        {
            return scene switch
            {
                Scenes.HALLWAYS => "Hallways",
                Scenes.W_KEY => "W Key",
                Scenes.A_KEY => "A Key",
                Scenes.SPACE_KEY => "Space Key",
                Scenes.ARROW_KEYS => "Arrow Keys",
                Scenes.P_KEY => "P Key",
                _ => "Level"
            };
        }

        private string GetLevelDescription(Scenes scene)
        {
            return scene switch
            {
                Scenes.HALLWAYS => "Find the doors for each malfunctioning key",

                Scenes.W_KEY => "Solve the puzzles one by one to restore the power",

                Scenes.A_KEY => "Dodge periodic zaps and use speed pads to reach and restart the generator",

                Scenes.SPACE_KEY => "Bypass the security system to enable the key",

                Scenes.ARROW_KEYS => "Fix the corrupted color system to unlock Robota’s movement",

                Scenes.P_KEY => "Stop the print jobs in the correct sequence before the room is flooded with papers",

                _ => "Get ready"
            };
        }

        public void HandleLevelTitleDone()
        {
            var canvases = FindObjectsOfType<Canvas>()
                .Where(c => c.gameObject.scene.name != "DontDestroyOnLoad").ToList();
            canvases.ForEach(c => c.enabled = true);

            ChangeState(GameState.Playing);
        }

        #endregion

        public void HandleGameVictory()
        {
            SaveManager.Instance.DeleteGame(SaveManager.Instance.SaveSlotName);
            SaveManager.Instance.LoadGame(SaveManager.Instance.SaveSlotName);
            HandleSceneLoad(Scenes.Main_Menu);
        }
    }
}