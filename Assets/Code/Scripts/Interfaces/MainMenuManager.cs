using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{

    //audio refrence
    //public AudioSource audioSource;
    //public AudioClip clickSound;
    public AudioSource spaceSFX;


    //buttons refrence
    public Button continueGameButton;
    private Dictionary<GameObject, Vector3> originalScales = new Dictionary<GameObject, Vector3>();


    //panels refrence
    public GameObject settingsPanel;
    public GameObject instructionsPanel;
    public GameObject creditsPanel;
    public GameObject exitPanel;
    public CanvasGroup mainMenuGroup;


    //script refrenc
    public SceneFader sceneFader;

    private void Start()
    {

        continueGameButton.interactable = PlayerPrefs.HasKey("LastLevel");

        //this code must be set after the complete a level
        //PlayerPrefs.SetInt("LastLevel", SceneManager.GetActiveScene().buildIndex);

    }



    //panels methods
    public void StartNewGame()
    {
        //SceneManager.LoadScene("Level1");
        sceneFader.FadeToScene("Fatima_PauseMenu");
    }

    public void ContinueGame()
    {
        if (PlayerPrefs.HasKey("LastLevel"))
        {
            PlayClickSound();
            int sceneToLoad = PlayerPrefs.GetInt("LastLevel");
            SceneManager.LoadScene(sceneToLoad);
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
        if (spaceSFX != null)
            spaceSFX.Play();
    }

}
