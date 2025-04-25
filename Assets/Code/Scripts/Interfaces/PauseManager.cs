using System;
using System.Collections;
using System.Collections.Generic;
using Code.Scripts.Managers;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    //scripts reference

    //panel refrences
    public GameObject pauseMenuUI;
    public GameObject helpPanel;
    public GameObject settingsPanel;
    public CanvasGroup pauseMenuGroup;

    [Header("Audio")] public AudioClip backgroundMusic;
    public AudioClip buttonSound;


    // private bool isPaused = false;


    private void Start()
    {
        // music.ignoreListenerPause = true;
    }

    void Update()
    {
        if (GameManager.Instance.CurrentScene == GameManager.Scenes.Main_Menu) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameManager.Instance.State == GameManager.GameState.Paused)
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
        GameManager.Instance.ChangeState(GameManager.GameState.Playing);

        // pauseMenuUI.SetActive(false);
        // Time.timeScale = 1f; // Resume game time
        // // isPaused = false;
        //
        // if (SoundManager.Instance.IsMusicPlaying) SoundManager.Instance.StopMusic();
    }

    public void PauseGame()
    {
        GameManager.Instance.ChangeState(GameManager.GameState.Paused);

        // pauseMenuUI.SetActive(true);
        // Time.timeScale = 0f; // Freeze game time
        // isPaused = true;

        if (!SoundManager.Instance.IsMusicPlaying) SoundManager.Instance.PlayMusic(backgroundMusic);
    }

    // public void ExitToMainMenu()
    // {
    //     Time.timeScale = 1f;
    //     GameStateTracker.returningFromGame = true;
    //     //SceneManager.LoadScene("Fatima_MainMenu"); // Replace with your actual main menu scene name
    //     pauseMenuUI.SetActive(false);
    //
    //
    //     // loadingScript.sceneToLoad = GameManager.Scenes.Main_Menu;
    //     // loadingScreen.SetActive(true);
    //     // loadingScript.BeginLoading();
    // }
    //
    // public void RestartLevel()
    // {
    //     Time.timeScale = 1f;
    //     SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    // }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        GameStateTracker.returningFromGame = true;

        pauseMenuUI.SetActive(false);

        GameManager.Instance.HandleSceneLoad(GameManager.Scenes.Main_Menu);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        GameManager.Instance.HandleSceneLoad(GameManager.Instance.CurrentScene);
    }

    public void ToggleHelpPanel()
    {
        helpPanel.SetActive(!helpPanel.activeSelf);
        pauseMenuGroup.interactable = false;
        pauseMenuGroup.blocksRaycasts = false;
    }

    public void SaveGame()
    {
        SaveManager.Instance.SaveGame();
        Debug.Log("Game saved!");
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