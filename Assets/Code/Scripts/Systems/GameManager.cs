using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Code.Scripts.Units.Heroes;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Central manager that handles game state transitions, level loading, UI control, player management, and save/load.
/// </summary>
public class GameManager : Singleton<GameManager>, IDataPersistence
{
    #region Enums & Types

    /// <summary>
    /// Represents the current state of the game.
    /// </summary>
    public enum GameState
    {
        Initial, // When the scene is first loaded and setup runs (e.g., title display)
        CutScene, // For cinematic scenes or transitions between levels
        Playing, // Active gameplay state
        Paused, // Game is paused
        GameOver, // Triggered when the player loses
        Victory // Triggered when the player wins a level
    }

    /// <summary>
    /// Enum defining all loadable scenes in the game.
    /// </summary>
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

    /// <summary>
    /// Serializable pair linking a scene enum to a background music clip.
    /// Used for automatic level-based audio playback.
    /// </summary>
    [Serializable]
    public class SceneAudioPair
    {
        public Scenes scene;
        public AudioClip clip;
    }

    #endregion

    #region Fields & Properties

    /// <summary>
    /// The current scene that is loaded and active.
    /// </summary>
    public Scenes CurrentScene { get; private set; }

    /// <summary>
    /// The current gameplay state (e.g., Playing, Paused).
    /// </summary>
    public GameState State { get; private set; }

    /// <summary>
    /// Tracks the scene from which victory was achieved, used for hallway re-entry.
    /// </summary>
    private Scenes previousSceneBeforeVictory;

    /// <summary>
    /// Fired before any game state change.
    /// </summary>
    public static event Action<GameState> OnBeforeGameStateChanged;

    /// <summary>
    /// Fired after any game state change.
    /// </summary>
    public static event Action<GameState> OnAfterGameStateChanged;

    [Header("UI & Logic References")] [SerializeField]
    private GameObject gameOverCanvas;

    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject levelTitleCanvas;
    [SerializeField] private GameObject victoryCanvas;

    [SerializeField] private LoadingManager loadingScript;
    [SerializeField] private LevelCompleteController victoryController;
    [SerializeField] private LevelTitle levelTitleScript;
    [SerializeField] private PauseManager pauseScript;

    [Header("Audio References")] [SerializeField]
    private List<SceneAudioPair> levelMusicList;

    /// <summary>
    /// Dictionary mapping scene enums to background music clips.
    /// </summary>
    private Dictionary<Scenes, AudioClip> levelMusicMap;

    /// <summary>
    /// Used to delay save data loading until the scene is fully initialized.
    /// </summary>
    private bool shouldLoadSaveDataAfterSceneLoad;

    /// <summary>
    /// Set true if the last scene transition was a level victory.
    /// </summary>
    private bool isVictory;

    /// <summary>
    /// Static mapping between scene enum and actual scene name strings for loading.
    /// </summary>
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

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// Initializes music map, scene validation, and registers debug commands.
    /// Sets initial game state depending on the active scene.
    /// </summary>
    private void Start()
    {
        levelMusicMap = levelMusicList.ToDictionary(pair => pair.scene, pair => pair.clip);
        RegisterDebugCommands();

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
            // Debug.LogWarning($"Scene name '{sceneName}' not found in SceneNameMap.");
        }
    }

    /// <summary>
    /// Subscribes to scene load event.
    /// </summary>
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// Unsubscribes from scene load event.
    /// </summary>
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    #endregion


    #region Game State Management

    /// <summary>
    /// Changes the current game state and invokes lifecycle events.
    /// Skips invalid or redundant transitions.
    /// </summary>
    public void ChangeState(GameState newState)
    {
        // Prevent mutually exclusive transitions
        if ((State == GameState.Victory && newState == GameState.GameOver) ||
            (State == GameState.GameOver && newState == GameState.Victory))
        {
            Debug.LogWarning($"Cannot change from {State} to {newState}");
            return;
        }

        // Skip redundant state change unless it's Initial or CutScene
        if (State == newState && newState != GameState.Initial && newState != GameState.CutScene) return;

        OnBeforeGameStateChanged?.Invoke(newState);

        switch (newState)
        {
            case GameState.Initial:
                StartCoroutine(HandelInitialState());
                break;
            case GameState.Paused:
                if (CurrentScene != Scenes.Main_Menu)
                    HandlePause();
                break;
            case GameState.Playing:
                if (CurrentScene != Scenes.Main_Menu)
                    HandleResume();
                break;
            case GameState.CutScene:
                if (CurrentScene != Scenes.Main_Menu)
                    HandleCutscene();
                break;
            case GameState.GameOver:
                if (CurrentScene != Scenes.Main_Menu)
                    StartCoroutine(HandleGameOver());
                break;
            case GameState.Victory:
                if (CurrentScene != Scenes.Main_Menu)
                    HandleVictory();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        State = newState;
        OnAfterGameStateChanged?.Invoke(newState);
    }

    /// <summary>
    /// Handles the "Initial" state, typically used for displaying level titles before gameplay begins.
    /// </summary>
    private IEnumerator HandelInitialState()
    {
        if (levelTitleScript == null)
        {
            Debug.LogWarning("Level title script not assigned.");
            yield break;
        }

        // Skip titles for cutscenes and menu-like scenes
        if (CurrentScene is Scenes.Main_Menu or Scenes.G_KEY or Scenes.ESC_KEY or Scenes.INCIDENT
            or Scenes.GAME_VICTORY_CUTSCENE)
            yield break;

        levelTitleScript.levelName = GetLevelName(CurrentScene);
        levelTitleScript.levelDescription = GetLevelDescription(CurrentScene);
        levelTitleScript.fadeInDuration = 0;
        TogglePlayerMovement(false);

        // Hide all scene-specific canvases
        var canvases = FindObjectsOfType<Canvas>()
            .Where(c => c.gameObject.scene.name != "DontDestroyOnLoad").ToList();
        canvases.ForEach(c => c.enabled = false);

        levelTitleScript.showLevelTitle();

        while (levelTitleScript.gameObject.activeSelf)
            yield return null;
    }

    /// <summary>
    /// Handles transition logic for CutScene state, including specific scene flow overrides.
    /// </summary>
    private void HandleCutscene()
    {
        if (CurrentScene == Scenes.G_KEY)
            SaveManager.Instance.SaveData.Progress.RepairedKeys.Add(CurrentScene);

        var nextScene = CurrentScene == Scenes.INCIDENT ? Scenes.ESC_KEY : Scenes.HALLWAYS;

        if (nextScene == Scenes.HALLWAYS)
        {
            if (CurrentScene == Scenes.G_KEY)
            {
                isVictory = true;
                previousSceneBeforeVictory = CurrentScene;
            }

            var previous = CurrentScene;
            CurrentScene = nextScene;

            HandleSceneLoad(nextScene, previous == Scenes.G_KEY ? GameState.Playing : GameState.Initial);
        }
        else
        {
            DisableAllCanvases();
            gameOverCanvas.SetActive(false);
            pauseMenuCanvas.SetActive(false);
            TogglePlayerMovement(false);
            SoundManager.Instance.StopAllAudio();

            SceneManager.LoadScene(SceneNameMap.GetValueOrDefault(nextScene));
            CurrentScene = nextScene;
            pauseScript.isTriggered = false;
            ChangeState(GameState.Initial);
        }
    }

    /// <summary>
    /// Determines if the game is currently in a state where pausing is allowed.
    /// </summary>
    public bool CanPause()
    {
        return CurrentScene is not (Scenes.Main_Menu or Scenes.G_KEY or Scenes.ESC_KEY or Scenes.INCIDENT
                   or Scenes.GAME_VICTORY_CUTSCENE)
               && State is not (GameState.CutScene or GameState.Initial)
               && !loadingScreen.activeSelf
               && !gameOverCanvas.activeSelf;
    }

    #endregion

    #region Scene Management

    /// <summary>
    /// Reloads the currently active scene.
    /// </summary>
    public void RestartLevel()
    {
        HandleSceneLoad(CurrentScene);
    }

    /// <summary>
    /// Loads the last saved level from SaveData.Progress.CurrentScene.
    /// </summary>
    public void LoadLastSavedLevel()
    {
        shouldLoadSaveDataAfterSceneLoad = true;
        HandleSceneLoad(SaveManager.Instance.SaveData.Progress.CurrentScene, GameState.Playing);
    }

    /// <summary>
    /// Loads the Hallways scene explicitly.
    /// </summary>
    public void GoToHallways()
    {
        HandleSceneLoad(Scenes.HALLWAYS);
    }

    /// <summary>
    /// Handles scene load logic, with special handling for full game win condition.
    /// </summary>
    public void HandleSceneLoad(Scenes newScene, GameState newState = GameState.Initial)
    {
        pauseScript.isTriggered = false;

        // If all keys are repaired, go to the victory cutscene
        if (new List<Scenes>
                { Scenes.W_KEY, Scenes.A_KEY, Scenes.SPACE_KEY, Scenes.G_KEY, Scenes.ARROW_KEYS, Scenes.P_KEY }
            .All(k => SaveManager.Instance.SaveData.Progress.RepairedKeys.Contains(k)))
        {
            SceneManager.LoadScene(SceneNameMap[Scenes.GAME_VICTORY_CUTSCENE]);
            CurrentScene = Scenes.GAME_VICTORY_CUTSCENE;
            ChangeState(GameState.Initial);
            return;
        }

        // Redirect repeated main menu loads to Hallways
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

    /// <summary>
    /// Loads a scene asynchronously (used internally by LoadingManager).
    /// </summary>
    public AsyncOperation LoadLevelAsync(Scenes scene, GameState newState = GameState.Initial)
    {
        var op = SceneManager.LoadSceneAsync(SceneNameMap[scene]);
        CurrentScene = scene;
        return op;
    }

    /// <summary>
    /// Called by the loading screen once the scene has loaded.
    /// </summary>
    public void HandleSceneLoaded()
    {
        loadingScreen.SetActive(false);
        TogglePlayerMovement(true);
        StartCoroutine(ReapplyBindingsNextFrame());

        if (CurrentScene == Scenes.HALLWAYS && State != GameState.Initial)
            StartCoroutine(SetPlayerPositionNextFrame());
    }

    /// <summary>
    /// Triggered by Unity after a scene finishes loading.
    /// Used to optionally load save data after the scene is ready.
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (shouldLoadSaveDataAfterSceneLoad)
        {
            shouldLoadSaveDataAfterSceneLoad = false;
            SaveManager.Instance.LoadGame(SaveManager.Instance.SaveData.Meta.SaveName);
            // Debug.Log("Save data loaded after loading last saved level.");
        }
    }

    #endregion

    #region Victory & Game Over

    /// <summary>
    /// Handles all actions needed when a level is won.
    /// Disables UI, locks movement, and shows victory animation and title.
    /// </summary>
    private void HandleVictory()
    {
        isVictory = true;
        previousSceneBeforeVictory = CurrentScene;

        SoundManager.Instance.StopAllAudio();
        DisableAllCanvases();
        TogglePlayerMovement(false);

        // Show win screen → fades out and transitions to Hallways
        victoryController.ShowCompleteScene();

        var pbm = FindObjectOfType<PlayerBindingManage>();
        if (pbm == null)
        {
            Debug.LogWarning("Cannot find PlayerBindingManage.");
            return;
        }

        // Unlock abilities based on level
        switch (CurrentScene)
        {
            case Scenes.W_KEY:
                pbm.EnableBinding("Move", "up");
                break;
            case Scenes.A_KEY:
                pbm.EnableBinding("Move", "left");
                break;
            case Scenes.SPACE_KEY:
                pbm.EnableBinding("Jump");
                break;
        }

        SaveManager.Instance.SaveData.Progress.RepairedKeys.Add(CurrentScene);
    }

    /// <summary>
    /// Displays the Game Over canvas and fades it in.
    /// </summary>
    private IEnumerator HandleGameOver()
    {
        float fadeInDuration = 1f;

        SoundManager.Instance.StopAllAudio();
        TogglePlayerMovement(false);

        yield return new WaitForSeconds(0.3f);

        gameOverCanvas.SetActive(true);
        var canvasGroup = gameOverCanvas.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;

        yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 0, 1, fadeInDuration));
    }

    /// <summary>
    /// Handles the global end-of-game victory case, wipes save, and redirects to main menu.
    /// </summary>
    public void HandleGameVictory()
    {
        SaveManager.Instance.DeleteGame(SaveManager.Instance.SaveSlotName);
        SaveManager.Instance.LoadGame(SaveManager.Instance.SaveSlotName);
        HandleSceneLoad(Scenes.Main_Menu);
    }

    #endregion

    #region Pause & Resume

    /// <summary>
    /// Activates the pause menu and stops time and audio.
    /// </summary>
    private void HandlePause()
    {
        SoundManager.Instance.StopAllAudio();
        Time.timeScale = 0f;
        pauseMenuCanvas.SetActive(true);
    }

    /// <summary>
    /// Resumes gameplay by enabling movement, resuming time, and restarting music.
    /// </summary>
    private void HandleResume()
    {
        SoundManager.Instance.StopAllAudio();

        if (levelMusicMap.TryGetValue(CurrentScene, out var musicClip))
            SoundManager.Instance.PlayMusic(musicClip);

        pauseMenuCanvas.SetActive(false);
        TogglePlayerMovement(true);
        Time.timeScale = 1f;
    }

    #endregion

    #region Save System

    /// <summary>
    /// Stores the current scene into the save data.
    /// </summary>
    public void SaveData(ref SaveData data)
    {
        data.Progress.CurrentScene = CurrentScene;
    }

    /// <summary>
    /// No-op because loading is handled manually after scene loads.
    /// </summary>
    public void LoadData(ref SaveData data)
    {
        // SaveManager handles this post-scene
    }

    #endregion

    #region Level Title

    /// <summary>
    /// Gets the human-readable name of a scene based on its enum.
    /// </summary>
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

    /// <summary>
    /// Provides a short mission description for a given scene.
    /// </summary>
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

    /// <summary>
    /// Called after level title finishes showing. Resumes gameplay and re-enables canvas.
    /// </summary>
    public void HandleLevelTitleDone()
    {
        var canvases = FindObjectsOfType<Canvas>()
            .Where(c => c.gameObject.scene.name != "DontDestroyOnLoad").ToList();

        canvases.ForEach(c => c.enabled = true);

        ChangeState(GameState.Playing);
    }

    #endregion

    #region Player Management

    /// <summary>
    /// Enables or disables the player's movement logic.
    /// </summary>
    public void TogglePlayerMovement(bool value)
    {
        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            player.GetComponent<PlayerMovement>().ToggleMovement(value);
        }
    }

    /// <summary>
    /// Gets the player's transform in the scene.
    /// </summary>
    public Transform GetPlayerTransform()
    {
        return FindPlayer()?.transform;
    }

    /// <summary>
    /// Moves the player to a new position and rotation, safely re-enabling the CharacterController.
    /// </summary>
    public void MovePlayerTo(Vector3 targetPosition, Quaternion targetRotation)
    {
        var player = GetPlayerTransform();
        if (player == null) return;

        var controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
            player.position = targetPosition;
            player.rotation = targetRotation;
            controller.enabled = true;
        }
    }

    /// <summary>
    /// Finds the player GameObject using the "Player" tag.
    /// </summary>
    [CanBeNull]
    private GameObject FindPlayer()
    {
        return GameObject.FindGameObjectWithTag("Player");
    }

    /// <summary>
    /// Adds to the player's collectables and updates UI.
    /// </summary>
    public void IncrementCollectables()
    {
        var collector = FindPlayer()?.GetComponent<Collector>();
        collector?.AddCollectable();

        if (collector != null)
        {
            HallwaysManager.Instance.UpdateInteractablesUI(collector.CollectablesCount);
        }
    }

    /// <summary>
    /// Gets the player's collectables count from the Collector component.
    /// </summary>
    public int GetCollectablesCount()
    {
        return FindPlayer()?.GetComponent<Collector>()?.CollectablesCount ?? 0;
    }

    #endregion

    #region Utility

    /// <summary>
    /// Performs a fade animation on a canvas group over time.
    /// </summary>
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

    /// <summary>
    /// Disables all canvas objects in the current scene (excluding DontDestroyOnLoad).
    /// </summary>
    private void DisableAllCanvases()
    {
        var allCanvases = FindObjectsOfType<Canvas>(true);
        foreach (var canvas in allCanvases)
        {
            if (canvas.gameObject.scene.name != "DontDestroyOnLoad")
                canvas.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Delays input reapplication until next frame to allow PlayerInput initialization.
    /// </summary>
    private IEnumerator ReapplyBindingsNextFrame()
    {
        yield return null;
        SaveManager.Instance.LoadPlayerBindings();
    }

    /// <summary>
    /// Repositions player on level load based on prior scene state or default save data.
    /// </summary>
    private IEnumerator SetPlayerPositionNextFrame()
    {
        yield return null;

        if (isVictory)
        {
            isVictory = false;
            HallwaysManager.Instance.HandleVictory(previousSceneBeforeVictory);
            Debug.Log("Set player position next to exit door of: " + previousSceneBeforeVictory);
        }
        else if (State != GameState.Initial)
        {
            SaveManager.Instance.LoadHallwayPosition();
        }
        else
        {
            SaveManager.Instance.SaveGame();
        }

        HallwaysManager.Instance.UpdateInteractablesUI(SaveManager.Instance.SaveData.Progress.CollectablesCount);
    }

    #endregion

    #region Debug Command Setup

    /// <summary>
    /// Registers all debug console commands available in GameManager.
    /// </summary>
    private void RegisterDebugCommands()
    {
        DebugController.Instance?.AddDebugCommand(new DebugCommand("gm_test", "testing from the game manager", "",
            () => { Debug.Log("working in game manager"); }));

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

                if (Enum.TryParse(typeof(Scenes), args[0], out var scene))
                {
                    HandleSceneLoad((Scenes)scene);
                    Debug.Log($"Jumping to level: {args[0]}");
                }
                else
                {
                    Debug.LogWarning($"Scene '{args[0]}' is not recognized. Check your GameManager.Scenes enum.");
                }
            }));

        DebugController.Instance?.AddDebugCommand(new DebugCommand("win", "Triggers the victory state", "win", () =>
        {
            if (CurrentScene == Scenes.G_KEY)
            {
                ChangeState(GameState.CutScene);
                return;
            }

            ChangeState(GameState.Victory);
        }));

        DebugController.Instance?.AddDebugCommand(new DebugCommand("current_state", "Logs the current game state",
            "current_state",
            () => Debug.Log(State)));

        DebugController.Instance?.AddDebugCommand(new DebugCommand("simi_win", "Simulates a win", "simi_win", () =>
        {
            SaveManager.Instance.SaveData.Progress.RepairedKeys.AddRange(new List<Scenes>
            {
                Scenes.W_KEY, Scenes.A_KEY, Scenes.SPACE_KEY, Scenes.G_KEY, Scenes.ARROW_KEYS
            });

            CurrentScene = Scenes.ARROW_KEYS;

            var pbm = GetPlayerTransform()?.GetComponent<PlayerBindingManage>();
            pbm?.EnableBinding("Move", "up");
            pbm?.EnableBinding("Move", "left");
            pbm?.EnableBinding("Jump");

            ChangeState(GameState.Victory);
        }));
    }

    #endregion
}