using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Manages panel and button color assignment and matching logic for the color puzzle.
/// </summary>
public class ColorCoordinator : Singleton<ColorCoordinator>
{
    /// <summary>
    /// Buttons the player can press. Each will be assigned a color.
    /// </summary>
    [SerializeField] private List<PressButton> buttons = new();

    /// <summary>
    /// Panels that need to be matched with button colors.
    /// </summary>
    [SerializeField] private List<Panel> panels = new();

    private void Start()
    {
        // Add debug commands for manual testing
        DebugController.Instance?.AddDebugCommand(new DebugCommand(
            "switch_button_colors", "changes button colors", "switch_button_colors", () => SwitchButtonsColors()));
        DebugController.Instance?.AddDebugCommand(new DebugCommand(
            "switch_panel_colors", "changes panel colors", "switch_panel_colors", () => SwitchPanelColors()));

        // Wait 1 frame before randomizing to ensure all components are initialized
        StartCoroutine(DelayedInit());
    }

    /// <summary>
    /// Called by buttons to check for a valid panel match.
    /// If successful, removes the matching panel and checks for victory.
    /// </summary>
    public void CheckButtonMatch()
    {
        if (panels.Count == 0)
        {
            GameManager.Instance.ChangeState(GameManager.GameState.Victory);
            return;
        }

        var pressedButtons = buttons.FindAll(b => b.IsPressed);

        // Require exactly two pressed buttons
        if (pressedButtons.Count != 2)
            return;

        var sameColor = pressedButtons[0].AssignedColor == pressedButtons[1].AssignedColor;
        var panelColor = panels[0]?.AssignedColor;

        // Check if both buttons match each other and the current panel
        if (sameColor && pressedButtons[0].AssignedColor == panelColor)
        {
            panels[0].gameObject.SetActive(false);
            panels.RemoveAt(0);

            if (panels.Count == 0)
                GameManager.Instance.ChangeState(GameManager.GameState.Victory);
        }
    }

    /// <summary>
    /// Initializes the puzzle colors after one frame delay.
    /// </summary>
    private IEnumerator DelayedInit()
    {
        yield return null; // Wait a frame for all components to initialize

        SwitchPanelColors();
        SwitchButtonsColors();
    }

    /// <summary>
    /// Randomly assigns two buttons of each color (Red, Green, Blue).
    /// Ensures exactly six buttons are available.
    /// </summary>
    private void SwitchButtonsColors()
    {
        if (buttons.Count < 6)
        {
            Debug.LogWarning("Not enough buttons to assign colors (need at least 6).");
            return;
        }

        // Build the color pool: 2 of each color
        List<PressButton.PressedColor> colorPool = new()
        {
            PressButton.PressedColor.Blue,
            PressButton.PressedColor.Blue,
            PressButton.PressedColor.Red,
            PressButton.PressedColor.Red,
            PressButton.PressedColor.Green,
            PressButton.PressedColor.Green
        };

        // Shuffle the pool using Fisher-Yates algorithm
        for (var i = 0; i < colorPool.Count; i++)
        {
            var rnd = Random.Range(i, colorPool.Count);
            (colorPool[i], colorPool[rnd]) = (colorPool[rnd], colorPool[i]);
        }

        // Assign colors to the first six buttons
        for (var i = 0; i < 6; i++)
            buttons[i].ChangeColor(colorPool[i]);
    }

    /// <summary>
    /// Randomly assigns colors to panels while ensuring at least one of each color,
    /// and tries to avoid repeating the same color on adjacent panels.
    /// </summary>
    private void SwitchPanelColors()
    {
        if (panels.Count < 3)
        {
            Debug.LogWarning("Not enough panels to assign at least one of each color.");
            return;
        }

        // Ensure at least one of each required color
        List<PressButton.PressedColor> requiredColors = new()
        {
            PressButton.PressedColor.Red,
            PressButton.PressedColor.Green,
            PressButton.PressedColor.Blue
        };

        // Fill remaining panels with random colors
        while (requiredColors.Count < panels.Count)
            requiredColors.Add((PressButton.PressedColor)Random.Range(0, 3));

        List<PressButton.PressedColor> shuffledColors;
        int maxTries = 50;

        // Try to shuffle colors without placing duplicates adjacent to each other
        do
        {
            shuffledColors = new List<PressButton.PressedColor>(requiredColors);
            for (int i = 0; i < shuffledColors.Count; i++)
            {
                int rnd = Random.Range(i, shuffledColors.Count);
                (shuffledColors[i], shuffledColors[rnd]) = (shuffledColors[rnd], shuffledColors[i]);
            }

            bool hasAdjacentSameColor = false;
            for (int i = 1; i < shuffledColors.Count; i++)
            {
                if (shuffledColors[i] == shuffledColors[i - 1])
                {
                    hasAdjacentSameColor = true;
                    break;
                }
            }

            if (!hasAdjacentSameColor)
                break;

            maxTries--;
        } while (maxTries > 0);

        if (maxTries == 0)
            Debug.LogWarning("Could not find a non-repeating sequence after multiple attempts.");

        // Apply colors to panels
        for (int i = 0; i < panels.Count; i++)
            panels[i].ChangeColor(shuffledColors[i]);
    }
}