using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the color-matching light puzzle in the arrow room.
/// Players use RGB sliders to match a target color.
/// </summary>
public class BulbColorController : MonoBehaviour
{
    /// <summary>
    /// The main bulb image that reflects the current selected color.
    /// </summary>
    public Image bulbImage;

    /// <summary>
    /// Secondary effect overlay for the bulb (e.g., glow).
    /// </summary>
    public Image bulbEffect;

    /// <summary>
    /// Slider controlling red component.
    /// </summary>
    public Slider redSlider;

    /// <summary>
    /// Slider controlling green component.
    /// </summary>
    public Slider greenSlider;

    /// <summary>
    /// Slider controlling blue component.
    /// </summary>
    public Slider blueSlider;

    /// <summary>
    /// UI images displaying the target color for player reference.
    /// </summary>
    public List<Image> TargetColorImages;

    /// <summary>
    /// Image that displays the currently selected RGB color.
    /// </summary>
    [SerializeField] private Image currentColorImage;

    /// <summary>
    /// Text element used to display a match confirmation message.
    /// </summary>
    public TMP_Text matchMessageText;


    /// <summary>
    /// Sound to play upon successful color match.
    /// </summary>
    public AudioClip doneSound;

    /// <summary>
    /// UI container for the color-matching interface.
    /// </summary>
    public GameObject ArrowRoomInterface;

    /// <summary>
    /// The color players are attempting to match.
    /// </summary>
    [SerializeField] private Color targetColor;

    /// <summary>
    /// Maximum total RGB difference allowed for a successful match.
    /// </summary>
    [SerializeField] private int minimumMatch = 30;

    /// <summary>
    /// References to the actual scene lights affected by slider values.
    /// </summary>
    private List<Light> lights = new();

    /// <summary>
    /// Value used to round RGB floats into integers for comparison.
    /// </summary>
    private readonly int roundTo = 100;

    /// <summary>
    /// Indicates whether the puzzle has been successfully completed.
    /// </summary>
    public bool IsDone { get; private set; }

    /// <summary>
    /// Initialize light list and slider listeners.
    /// </summary>
    private void Start()
    {
        lights = MazeController.Instance.Lights;

        // Add listeners to all sliders to update light color live
        redSlider.onValueChanged.AddListener(_ => UpdateLightColor());
        greenSlider.onValueChanged.AddListener(_ => UpdateLightColor());
        blueSlider.onValueChanged.AddListener(_ => UpdateLightColor());
    }

    /// <summary>
    /// Initializes and displays the arrow color matching UI.
    /// </summary>
    public void ShowArrowChallenge()
    {
        // Set slider values based on current light color or defaults
        if (lights.Count > 1)
        {
            redSlider.value = lights[0].color.r;
            greenSlider.value = lights[0].color.g;
            blueSlider.value = lights[0].color.b;
        }
        else
        {
            redSlider.value = 1;
            greenSlider.value = 1;
            blueSlider.value = 1;
        }

        // Enable sliders and interface
        redSlider.interactable = true;
        greenSlider.interactable = true;
        blueSlider.interactable = true;

        ArrowRoomInterface.SetActive(true);

        // Set target color on all display elements
        targetColor.a = 1;
        TargetColorImages.ForEach(i => i.color = targetColor);

        // Hide match message until matched
        if (matchMessageText != null)
            matchMessageText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Updates the bulb and environment lights based on slider values.
    /// </summary>
    private void UpdateLightColor()
    {
        var currentColor = new Color(redSlider.value, greenSlider.value, blueSlider.value, 1f);

        // Apply color to all lights and UI previews
        lights.ForEach(l => l.color = currentColor);
        currentColorImage.color = currentColor;
        bulbImage.color = currentColor;
        bulbEffect.color = currentColor;

        // Check if color matches the target
        if (ColorsAreClose(currentColor, targetColor))
        {
            if (matchMessageText.gameObject.activeSelf) return;

            // Disable further input and show success
            redSlider.interactable = false;
            greenSlider.interactable = false;
            blueSlider.interactable = false;

            matchMessageText.gameObject.SetActive(true);
            SoundManager.Instance.PlaySound(doneSound);
            StartCoroutine(HideArrowInterfaceAfterDelay());
        }
        else
        {
            matchMessageText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Determines whether two colors are close enough to be considered a match.
    /// </summary>
    private bool ColorsAreClose(Color a, Color b)
    {
        // Round and compare individual RGB channels
        var r1 = (int)(a.r * roundTo);
        var g1 = (int)(a.g * roundTo);
        var b1 = (int)(a.b * roundTo);

        var r2 = (int)(b.r * roundTo);
        var g2 = (int)(b.g * roundTo);
        var b2 = (int)(b.b * roundTo);

        var total = Mathf.Abs(r1 - r2) + Mathf.Abs(g1 - g2) + Mathf.Abs(b1 - b2);
        return total < minimumMatch;
    }

    /// <summary>
    /// Hides the interface after success and finalizes the puzzle.
    /// </summary>
    private IEnumerator HideArrowInterfaceAfterDelay()
    {
        lights.ForEach(l => l.color = targetColor);
        yield return new WaitForSeconds(2f);
        ArrowRoomInterface.SetActive(false);
        IsDone = true;
        MazeController.Instance.BoardIsDone();
    }
}