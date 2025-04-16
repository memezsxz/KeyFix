using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{

    //scripts reference
    public LoadingManager loadingScript;

    //panel refrences
    public GameObject pauseMenuUI;
    public GameObject helpPanel;
    public GameObject settingsPanel;
    public CanvasGroup pauseMenuGroup;
    public GameObject loadingScreen;

    [Header("Audio")]
    public AudioSource music;
    public AudioMixer audioMixer;


    private bool isPaused = false;


    private void Start()
    {
        music.ignoreListenerPause = true;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Resume game time
        isPaused = false;

        if (music.isPlaying)
            music.Stop();
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Freeze game time
        isPaused = true;

        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.3f);
        float db = Mathf.Log10(Mathf.Clamp(savedVolume, 0.0001f, 1f)) * 20;

        // Apply to AudioMixer before playing
        audioMixer.SetFloat("MusicVolume", db);

        if (!music.isPlaying)
            music.Play();
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        GameStateTracker.returningFromGame = true;
        //SceneManager.LoadScene("Fatima_MainMenu"); // Replace with your actual main menu scene name
        pauseMenuUI.SetActive(false);
        loadingScript.sceneToLoad = "Fatima_MainMenu";
        loadingScreen.SetActive(true);
        loadingScript.BeginLoading();

    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ToggleHelpPanel()
    {
        helpPanel.SetActive(!helpPanel.activeSelf);
        pauseMenuGroup.interactable = false;
        pauseMenuGroup.blocksRaycasts = false;
    }
    public void SaveGame()
    {
        Debug.Log("Game saved!"); // Replace with your save logic later
    }


    public void ToggleSettingsPanel()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
        pauseMenuGroup.interactable = false;
        pauseMenuGroup.blocksRaycasts = false;
    }

    public void CloseAllPanels()
    {
        settingsPanel.SetActive(false);
        helpPanel.SetActive(false);
        pauseMenuGroup.interactable = true;
        pauseMenuGroup.blocksRaycasts = true;
    }

}

public static class GameStateTracker
{
    public static bool returningFromGame = false;
}
