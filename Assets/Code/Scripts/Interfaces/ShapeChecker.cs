using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Compares the player's current bulb pattern to a target pattern and provides feedback.
/// </summary>
public class ShapeChecker : MonoBehaviour
{
    #region Serialized Fields

    /// <summary>
    /// The parent panel containing the entire shape challenge.
    /// </summary>
    [Header("Panels")] [SerializeField] private GameObject wholeGamePanel;

    /// <summary>
    /// The panel shown when the shape is correct.
    /// </summary>
    [SerializeField] private GameObject successfulPanel;

    /// <summary>
    /// The panel shown when the shape is incorrect.
    /// </summary>
    [SerializeField] private GameObject failedPanel;

    /// <summary>
    /// Audio clip to play on success.
    /// </summary>
    [Header("Sounds")] [SerializeField] private AudioClip successfulSound;

    /// <summary>
    /// Audio clip to play on failure.
    /// </summary>
    [SerializeField] private AudioClip failedSound;

    /// <summary>
    /// Parent transform containing all LightBulb components.
    /// </summary>
    [Header("Board")] public Transform bulbGrid;

    /// <summary>
    /// Boolean pattern representing the correct on/off state of each bulb.
    /// </summary>
    public bool[] targetPattern;

    /// <summary>
    /// Optional callback to trigger on success.
    /// </summary>
    [Header("Actions")] public Action successCallback;

    #endregion

    #region Public Methods

    /// <summary>
    /// Checks the current bulb configuration against the target pattern.
    /// Shows either the success or failure panel based on the result.
    /// </summary>
    public void CheckShape()
    {
        var match = true;

        // Compare each bulb state with the expected pattern
        for (var i = 0; i < bulbGrid.childCount; i++)
        {
            var bulb = bulbGrid.GetChild(i).GetComponent<LightBulb>();
            var bulbState = bulb.IsOn();

            if (bulbState != targetPattern[i])
            {
                match = false;
                break;
            }
        }

        // Show appropriate result panel
        if (match)
        {
            StartCoroutine(ShowSuccessfulPanel());
        }
        else
        {
            StartCoroutine(ShowFailedPanel());
        }
    }

    #endregion

    #region Feedback Routines

    /// <summary>
    /// Displays the success panel, plays audio, disables UI, and invokes callback.
    /// </summary>
    private IEnumerator ShowSuccessfulPanel()
    {
        successfulPanel.SetActive(true);
        SoundManager.Instance.PlaySound(successfulSound);

        yield return new WaitForSeconds(1.5f);

        wholeGamePanel.SetActive(false);
        SoundManager.Instance.StopSound();

        // Notify any listener that the shape was correct
        successCallback?.Invoke();
    }

    /// <summary>
    /// Displays the failure panel, plays audio, then hides the panel after a delay.
    /// </summary>
    private IEnumerator ShowFailedPanel()
    {
        failedPanel.SetActive(true);
        SoundManager.Instance.PlaySound(failedSound);

        yield return new WaitForSeconds(1.5f); // Wait for 1 second

        failedPanel.SetActive(false);
        SoundManager.Instance.StopSound();
    }

    #endregion
}