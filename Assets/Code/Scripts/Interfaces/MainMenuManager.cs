using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls main menu logic including navigation, settings, and transition to the game.
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    #region UI References

    /// <summary>
    /// Sound played on button click.
    /// </summary>
    public AudioClip clickSound;

    /// <summary>
    /// The continue game button (enabled only if a save is found).
    /// </summary>
    public Button continueGameButton;

    [Header("Panels")] public GameObject settingsPanel;
    public GameObject instructionsPanel;
    public GameObject creditsPanel;
    public GameObject exitPanel;

    /// <summary>
    /// The main menu canvas group used for interaction control.
    /// </summary>
    public CanvasGroup mainMenuGroup;


    /// <summary>
    /// Reference to the main scene root object (e.g., 3D background).
    /// </summary>
    [Header("Settings reference")] public GameObject mainScene;

    #endregion

    #region Script References

    /// <summary>
    /// Reference to the settings manager used within the menu.
    /// </summary>
    public SettingsManager settingsScript;

    #endregion

    #region Animation State

    /// <summary>
    /// Tracks original scales of buttons for hover animation.
    /// </summary>
    private readonly Dictionary<GameObject, Vector3> originalScales = new();

    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        // Disable continue button if no save exists
        continueGameButton.interactable = !SaveManager.Instance.IsNewGame;
    }

    #endregion

    #region Game Start Methods

    /// <summary>
    /// Starts a new game and loads the first scene.
    /// </summary>
    public void StartNewGame()
    {
        SaveManager.Instance.StartNewGame();
        GameManager.Instance.HandleSceneLoad(GameManager.Scenes.INCIDENT);
        CloseAllPanels();
        mainScene.SetActive(false);
    }

    /// <summary>
    /// Continues from the last saved scene, if available.
    /// </summary>
    public void ContinueGame()
    {
        if (!SaveManager.Instance.IsNewGame)
        {
            GameManager.Instance.LoadLastSavedLevel();
            PlayClickSound();
            mainScene.SetActive(false);
        }
        else
        {
            Debug.Log("No saved game found!");
        }
    }

    #endregion

    #region Panel Control Methods

    /// <summary>
    /// Opens the settings panel and disables menu input.
    /// </summary>
    public void OpenSettings()
    {
        // Debug.Log("Open Settings");
        PlayClickSound();
        settingsPanel.SetActive(true);
        settingsScript.enabled = true;
        DisableMainMenuInput();
    }

    /// <summary>
    /// Opens the instructions panel and disables menu input.
    /// </summary>
    public void OpenInstructions()
    {
        // Debug.Log("Open Instructions");
        PlayClickSound();
        instructionsPanel.SetActive(true);
        DisableMainMenuInput();
    }

    /// <summary>
    /// Opens the credits panel and disables menu input.
    /// </summary>
    public void OpenCredits()
    {
        // Debug.Log("Open Credits");
        PlayClickSound();
        creditsPanel.SetActive(true);
        DisableMainMenuInput();
    }

    /// <summary>
    /// Opens the exit confirmation panel and disables menu input.
    /// </summary>
    public void OpenExitPanel()
    {
        // Debug.Log("Open Exit");
        PlayClickSound();
        exitPanel.SetActive(true);
        DisableMainMenuInput();
    }

    /// <summary>
    /// Closes all active panels and re-enables main menu input.
    /// </summary>
    public void CloseAllPanels()
    {
        PlayClickSound();

        if (settingsPanel.activeSelf) // save settings to file when the settings panel closes
        {
            settingsScript.enabled = false;
            SaveManager.Instance.SaveSettings();
        }

        settingsPanel.SetActive(false);
        instructionsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        exitPanel.SetActive(false);

        EnableMainMenuInput();
    }

    /// <summary>
    /// Exits the game.
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Exit Game");
    }

    #endregion

    #region UI Animation Methods

    /// <summary>
    /// Scales up the button when hovered.
    /// </summary>
    public void OnHoverEnter(GameObject btn)
    {
        var button = btn.GetComponent<Button>();
        if (button != null && button.interactable)
        {
            if (!originalScales.ContainsKey(btn))
                originalScales[btn] = btn.transform.localScale;

            PlayClickSound();
            btn.transform.localScale = originalScales[btn] * 1.1f;
        }
    }

    /// <summary>
    /// Resets button scale when hover exits.
    /// </summary>
    public void OnHoverExit(GameObject btn)
    {
        if (originalScales.ContainsKey(btn))
            btn.transform.localScale = originalScales[btn];
    }

    #endregion

    #region Sound

    /// <summary>
    /// Plays the configured UI click sound.
    /// </summary>
    public void PlayClickSound()
    {
        SoundManager.Instance.PlaySound(clickSound);
    }

    #endregion

    #region Helpers

    private void DisableMainMenuInput()
    {
        mainMenuGroup.interactable = false;
        mainMenuGroup.blocksRaycasts = false;
    }

    private void EnableMainMenuInput()
    {
        mainMenuGroup.interactable = true;
        mainMenuGroup.blocksRaycasts = true;
    }

    #endregion
}