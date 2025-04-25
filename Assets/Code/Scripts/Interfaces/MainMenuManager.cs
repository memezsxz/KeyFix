using System;
using System.Collections;
using System.Collections.Generic;
using Code.Scripts.Managers;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    //audio refrence
    public AudioClip clickSound;


    //buttons refrence
    public Button continueGameButton;
    private Dictionary<GameObject, Vector3> originalScales = new Dictionary<GameObject, Vector3>();


    //panels refrence
    public GameObject settingsPanel;
    public GameObject instructionsPanel;
    public GameObject creditsPanel;
    public GameObject exitPanel;
    public CanvasGroup mainMenuGroup;
    public GameObject loadingScreen;
    public GameObject mainScene;


    //script refrenc
    public SceneFader sceneFader;
    public LoadingManager loadingScript;
    public SettingsManager settingsScript;


    private void Start()
    {
        continueGameButton.interactable = !SaveManager.Instance.IsNewGame;

        //this code must be set after the complete a level

        //PlayerPrefs.SetInt("LastLevel", SceneManager.GetActiveScene().buildIndex);
    }


    //panels methods
    public void StartNewGame()
    {
        loadingScript.sceneToLoad = GameManager.Scenes.HALLWAYS;
        loadingScreen.SetActive(true);
        loadingScript.BeginLoading();
        SaveManager.Instance.SaveGame();
        CloseAllPanels();
        mainScene.SetActive(false);
    }


    public void ContinueGame()
    {
        if (!SaveManager.Instance.IsNewGame)
        {
            loadingScript.sceneToLoad = SaveManager.Instance.SaveData.Progress.CurrentScene;
            loadingScript.stateToLoadIn = GameManager.GameState.Playing;
            loadingScreen.SetActive(true);
            loadingScript.BeginLoading();

            PlayClickSound();
            mainScene.SetActive(false);
            // Debug.Log("will be going to the last saved level: " + SaveManager.Instance.SaveData.Progress.CurrentScene);
            // SceneManager.LoadScene("maryam city test");
            // will be done with use of the game manager and scene manager
            // int sceneToLoad = PlayerPrefs.GetInt("LastLevel");
            // SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.Log("No saved game found!");
        }
    }

    public void OpenSettings()
    {
        Debug.Log("Open Settings");
        PlayClickSound();
        settingsPanel.SetActive(true);
        settingsScript.enabled = true;
        mainMenuGroup.interactable = false;
        mainMenuGroup.blocksRaycasts = false;
    }

    public void OpenInstructions()
    {
        Debug.Log("Open Instructions");
        PlayClickSound();
        instructionsPanel.SetActive(true);
        mainMenuGroup.interactable = false;
        mainMenuGroup.blocksRaycasts = false;
        mainMenuGroup.interactable = false;
        mainMenuGroup.blocksRaycasts = false;
    }

    public void OpenCredits()
    {
        Debug.Log("Open Credits");
        PlayClickSound();
        creditsPanel.SetActive(true);
        mainMenuGroup.interactable = false;
        mainMenuGroup.blocksRaycasts = false;
    }

    public void OpenExitPanel()
    {
        Debug.Log("Open Exit");
        PlayClickSound();
        exitPanel.SetActive(true);
        mainMenuGroup.interactable = false;
        mainMenuGroup.blocksRaycasts = false;
    }

    public void CloseAllPanels()
    {
        PlayClickSound();
        if (settingsPanel.activeSelf)
        {
            settingsScript.enabled = false;
            SaveManager.Instance.SaveSettings();
        }

        settingsPanel.SetActive(false);
        instructionsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        exitPanel.SetActive(false);
        mainMenuGroup.interactable = true;
        mainMenuGroup.blocksRaycasts = true;
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Exit Game");
    }


    //Animation methods
    public void OnHoverEnter(GameObject btn)
    {
        Button button = btn.GetComponent<Button>();
        if (button != null && button.interactable == true)
        {
            if (!originalScales.ContainsKey(btn))
            {
                originalScales[btn] = btn.transform.localScale;
            }

            PlayClickSound();
            btn.transform.localScale = originalScales[btn] * 1.1f;
        }
    }

    public void OnHoverExit(GameObject btn)
    {
        if (originalScales.ContainsKey(btn))
            btn.transform.localScale = originalScales[btn];
    }


    //Audio methods
    public void PlayClickSound()
    {
        SoundManager.Instance.PlaySound(clickSound);
    }
}