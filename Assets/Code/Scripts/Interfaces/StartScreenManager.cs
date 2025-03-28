using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScreenManager : MonoBehaviour
{
    public GameObject pressScreenCanvas; // Canvas with logo and press space text
    public GameObject mainMenuCanvas;
    public AudioSource spaceSFX; // Assign this in Inspector


    private bool hasPressedSpace = false;

    void Start()
    {
        // Show the press screen and hide the main menu
        if (GameStateTracker.returningFromGame)
        {
            pressScreenCanvas.SetActive(false);
            mainMenuCanvas.SetActive(true);
            GameStateTracker.returningFromGame = false; // reset
        }
    }

    void Update()
    {
        if (!hasPressedSpace && Input.GetKeyDown(KeyCode.Escape))
        {
            hasPressedSpace = true;
            pressScreenCanvas.SetActive(false);

            if (spaceSFX != null)
                spaceSFX.Play();

            if (mainMenuCanvas != null)
                mainMenuCanvas.SetActive(true);
        }
    }
}
