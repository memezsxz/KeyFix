using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Scripts.Managers
{
    public class ColorCoordinator : Singleton<ColorCoordinator>
    {
        [SerializeField] List<PressButton> buttons = new();
        [SerializeField] List<Panel> panels = new();

        private void Start()
        {
            DebugController.Instance?.AddDebugCommand(new DebugCommand("switch_button_colors", "changes button colors", "switch_button_colors", () => SwitchButtonsColors()));
            DebugController.Instance?.AddDebugCommand(new DebugCommand("switch_panel_colors", "changes panel colors", "switch_panel_colors", () => SwitchPanelColors()));
            StartCoroutine(DelayedInit());
        }

        public void CheckButtonMatch()
        {
            if (panels.Count == 0)
            {
                // Debug.Log("All panels solved! Victory!");
                GameManager.Instance.ChangeState(GameManager.GameState.Victory);
                return;
            }
            var pressedButtons = buttons.FindAll(b => b.IsPressed);

            if (pressedButtons.Count != 2)
                return;

            var sameColor = pressedButtons[0].AssignedColor == pressedButtons[1].AssignedColor;
            var panelColor = panels[0]?.AssignedColor;

            if (sameColor && pressedButtons[0].AssignedColor == panelColor)
            {
                // Debug.Log("Panel match successful!");

                panels[0].gameObject.SetActive(false);
                panels.RemoveAt(0);

                if (panels.Count == 0)
                {
                    // Debug.Log("All panels solved! Victory!");
                    GameManager.Instance.ChangeState(GameManager.GameState.Victory);
                }
                else
                {
                    // Optional: highlight next panel
                    // Debug.Log("Proceed to next panel.");
                }
            }
        }

        private IEnumerator DelayedInit()
        {
            yield return null; // Wait 1 frame

            SwitchPanelColors();
            SwitchButtonsColors();
        }

        private void SwitchButtonsColors()
        {
            if (buttons.Count < 6)
            {
                Debug.LogWarning("Not enough buttons to assign colors (need at least 6).");
                return;
            }

            // Ensure exactly 2 of each color
            List<PressButton.PressedColor> colorPool = new()
            {
                PressButton.PressedColor.Blue,
                PressButton.PressedColor.Blue,
                PressButton.PressedColor.Red,
                PressButton.PressedColor.Red,
                PressButton.PressedColor.Green,
                PressButton.PressedColor.Green
            };

            // Shuffle the color assignments
            for (int i = 0; i < colorPool.Count; i++)
            {
                int rnd = Random.Range(i, colorPool.Count);
                (colorPool[i], colorPool[rnd]) = (colorPool[rnd], colorPool[i]);
            }

            // Assign to buttons in order
            for (int i = 0; i < 6; i++)
            {
                buttons[i].ChangeColor(colorPool[i]);
            }
        }

        private void SwitchPanelColors()
        {
            if (panels.Count < 3)
            {
                Debug.LogWarning("Not enough panels to assign at least one of each color.");
                return;
            }

            // Ensure at least one of each color
            List<PressButton.PressedColor> requiredColors = new()
            {
                PressButton.PressedColor.Red,
                PressButton.PressedColor.Green,
                PressButton.PressedColor.Blue
            };

            // Fill the remaining with random colors
            while (requiredColors.Count < panels.Count)
            {
                requiredColors.Add((PressButton.PressedColor)Random.Range(0, 3));
            }

            // Shuffle the colors only, not the panels
            for (int i = 0; i < requiredColors.Count; i++)
            {
                int rnd = Random.Range(i, requiredColors.Count);
                (requiredColors[i], requiredColors[rnd]) = (requiredColors[rnd], requiredColors[i]);
            }

            // Assign colors in order to the panels list (no shuffling)
            for (int i = 0; i < panels.Count; i++)
            {
                panels[i].ChangeColor(requiredColors[i]);
            }
        }

    }
}