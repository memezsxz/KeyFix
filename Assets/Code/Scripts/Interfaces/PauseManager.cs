using UnityEngine;

/// <summary>
/// Manages pause functionality, including toggling UI panels, saving, and returning to the main menu.
/// </summary>
public class PauseManager : MonoBehaviour
{
    #region UI References

    /// <summary>
    /// Main pause menu UI panel.
    /// </summary>
    public GameObject pauseMenuUI;

    /// <summary>
    /// Help panel shown from the pause menu.
    /// </summary>
    public GameObject helpPanel;

    /// <summary>
    /// Settings panel shown from the pause menu.
    /// </summary>
    public GameObject settingsPanel;

    /// <summary>
    /// Canvas group controlling pause menu input.
    /// </summary>
    public CanvasGroup pauseMenuGroup;

    #endregion

    #region Audio

    /// <summary>
    /// Background music to play when paused.
    /// </summary>
    [Header("Audio")] public AudioClip backgroundMusic;

    /// <summary>
    /// Sound to play when buttons are pressed.
    /// </summary>
    public AudioClip buttonSound;

    #endregion

    #region State

    /// <summary>
    /// Flag to control triggering pause during a cutscene once.
    /// </summary>
    public bool isTriggered = false;

    #endregion

    #region Unity Methods

    private void Start()
    {
        // Music can optionally be set to ignore pause (if needed)
        // music.ignoreListenerPause = true;
    }

    private void Update()
    {
        // Pause logic triggered by pressing Escape
        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        // Handle cutscene interruption only once
        if ((GameManager.Instance.CurrentScene is GameManager.Scenes.INCIDENT or GameManager.Scenes.ESC_KEY) &&
            !isTriggered)
        {
            isTriggered = true;
            GameManager.Instance.ChangeState(GameManager.GameState.CutScene);
            return;
        }

        // Only allow pausing if the game state allows it
        if (!GameManager.Instance.CanPause()) return;

        // Toggle pause state
        if (GameManager.Instance.State == GameManager.GameState.Paused)
        {
            // Debug.Log("Paused");
            ResumeGame();
        }
        else
        {
            // Debug.Log("Unpaused");
            PauseGame();
        }
    }

    #endregion

    #region Pause Control

    /// <summary>
    /// Resumes the game and exits the pause state.
    /// </summary>
    public void ResumeGame()
    {
        GameManager.Instance.ChangeState(GameManager.GameState.Playing);
    }

    /// <summary>
    /// Pauses the game and activates the pause menu state.
    /// </summary>
    public void PauseGame()
    {
        GameManager.Instance.ChangeState(GameManager.GameState.Paused);

        if (!SoundManager.Instance.IsMusicPlaying)
            SoundManager.Instance.PlayMusic(backgroundMusic);
    }

    #endregion

    #region Navigation Actions

    /// <summary>
    /// Loads the main menu scene and exits pause mode.
    /// </summary>
    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        GameStateTracker.returningFromGame = true;
        pauseMenuUI.SetActive(false);
        GameManager.Instance.HandleSceneLoad(GameManager.Scenes.Main_Menu);
    }

    /// <summary>
    /// Restarts the current level.
    /// </summary>
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        GameManager.Instance.RestartLevel();
    }

    /// <summary>
    /// Saves the current game progress.
    /// </summary>
    public void SaveGame()
    {
        SaveManager.Instance.SaveGame();
        // Debug.Log("Game saved!");
    }

    #endregion

    #region Panel Toggles

    /// <summary>
    /// Toggles the help panel visibility.
    /// </summary>
    public void ToggleHelpPanel()
    {
        helpPanel.SetActive(!helpPanel.activeSelf);
        pauseMenuGroup.interactable = false;
        pauseMenuGroup.blocksRaycasts = false;
    }

    /// <summary>
    /// Toggles the settings panel visibility.
    /// </summary>
    public void ToggleSettingsPanel()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
        pauseMenuGroup.interactable = false;
        pauseMenuGroup.blocksRaycasts = false;
    }

    /// <summary>
    /// Closes all open panels and restores input to the main pause menu.
    /// </summary>
    public void CloseAllPanels()
    {
        settingsPanel.SetActive(false);
        helpPanel.SetActive(false);
        pauseMenuGroup.interactable = true;
        pauseMenuGroup.blocksRaycasts = true;
    }

    #endregion
}