using UnityEngine;

/// <summary>
/// Manages the start screen behavior, transitioning from logo screen to main menu on key press.
/// </summary>
public class StartScreenManager : MonoBehaviour
{
    /// <summary>
    /// Canvas shown at the start containing the logo and 'press key' prompt.
    /// </summary>
    [Header("Canvases")] public GameObject pressScreenCanvas;

    /// <summary>
    /// Canvas containing the actual main menu options.
    /// </summary>
    public GameObject mainMenuCanvas;

    /// <summary>
    /// Sound to play when a button is pressed.
    /// </summary>
    [Header("Sounds")] public AudioClip ButtonClickedClip;

    /// <summary>
    /// Background music to play on the start screen.
    /// </summary>
    public AudioClip BacgroundMusicClip;

    /// <summary>
    /// Ensures input only registers once.
    /// </summary>
    private bool hasPressedSpace;

    private void Start()
    {
        // If returning from a scene (like pause or game over), skip the press screen
        if (GameStateTracker.returningFromGame)
        {
            pressScreenCanvas.SetActive(false);
            mainMenuCanvas.SetActive(true);
            GameStateTracker.returningFromGame = false; // reset the flag
        }

        SoundManager.Instance.PlayMusic(BacgroundMusicClip);
    }

    private void Update()
    {
        // Wait for user input to move from the splash screen to the menu
        if (!hasPressedSpace && Input.GetKeyDown(KeyCode.Escape))
        {
            hasPressedSpace = true;

            pressScreenCanvas.SetActive(false);
            SoundManager.Instance.PlaySound(ButtonClickedClip);

            if (mainMenuCanvas != null)
                mainMenuCanvas.SetActive(true);
        }
    }
}