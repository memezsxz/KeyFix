using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents a light bulb UI element that can be toggled on and off,
/// with visual and audio feedback.
/// </summary>
public class LightBulb : MonoBehaviour
{
    #region Serialized Fields

    /// <summary>
    /// Sound to play when toggling the bulb.
    /// </summary>
    public AudioClip bulbSound;

    /// <summary>
    /// GameObject representing the bulb's off visual state.
    /// </summary>
    public GameObject offSprite;

    /// <summary>
    /// GameObject representing the bulb's on visual state.
    /// </summary>
    public GameObject onSprite;

    #endregion

    #region Private Fields

    /// <summary>
    /// Reference to the Image component (if needed for future styling).
    /// </summary>
    private Image bulbImage;

    /// <summary>
    /// Tracks whether the bulb is currently on.
    /// </summary>
    private bool isOn;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        // Automatically fetch the Image component (not directly used here)
        bulbImage = GetComponent<Image>();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Toggles the bulb state (on/off), updates visuals, and plays sound.
    /// </summary>
    public void ToggleBulb()
    {
        isOn = !isOn;

        // Update visuals and play sound for both states
        if (isOn)
        {
            offSprite.SetActive(false);
            onSprite.SetActive(true);
            SoundManager.Instance.PlaySound(bulbSound);
        }
        else
        {
            offSprite.SetActive(true);
            onSprite.SetActive(false);
            SoundManager.Instance.PlaySound(bulbSound);
        }
    }

    /// <summary>
    /// Returns whether the bulb is currently on.
    /// </summary>
    public bool IsOn()
    {
        return isOn;
    }

    #endregion
}