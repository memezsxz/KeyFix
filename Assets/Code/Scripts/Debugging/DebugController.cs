using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

internal class DebugController : Singleton<DebugController>
{
    [SerializeField] private bool showConsole;
    [SerializeField] private bool showHelp;
    private readonly Dictionary<string, DebugCommand> _commands = new();
    private readonly bool TESTING = false;
    private string _input;
    private Vector2 _scroll;

    private void Start()
    {
        RegisterDefaultCommands();
    }

    private void OnGUI()
    {
        if (!showConsole) return; // If the console is not active, do not draw anything

        var y = 0f;

        // Initialize UI Styles
        var textFieldStyle = GetTextFieldStyle();
        var labelStyle = GetLabelStyle();

        // Show the Help Dropdown (if enabled)
        if (showHelp) y += RenderHelpDropdown(y, labelStyle);

        // Render Input Field
        RenderInputField(y, textFieldStyle);
    }

    private void OnToggleDebug()
    {
        showConsole = !showConsole;
    }

    /// <summary>
    ///     Returns a GUIStyle for the input text field.
    /// </summary>
    private GUIStyle GetTextFieldStyle()
    {
        return new GUIStyle(GUI.skin.textField)
        {
            fontSize = 30,
            normal = { textColor = Color.white }
        };
    }

    /// <summary>
    ///     Returns a GUIStyle for command labels.
    /// </summary>
    private GUIStyle GetLabelStyle()
    {
        return new GUIStyle(GUI.skin.label)
        {
            fontSize = 30,
            wordWrap = false, // No wrapping
            clipping = TextClipping.Clip // Clips overflowing text
        };
    }

    /// <summary>
    ///     Renders the Help Dropdown with all available commands.
    /// </summary>
    private float RenderHelpDropdown(float y, GUIStyle labelStyle)
    {
        var scrollViewHeight = 150f;
        var commandViewHeight = 50f;

        GUI.Box(new Rect(0f, y, Screen.width, scrollViewHeight), "");

        var helpRect = new Rect(0f, y, Screen.width - 20f, commandViewHeight * _commands.Count);
        _scroll = GUI.BeginScrollView(new Rect(0, y + 10f, Screen.width, scrollViewHeight - 20f), _scroll, helpRect);

        for (var i = 0; i < _commands.Count; i++)
        {
            DebugCommandBase command = _commands.Values.ElementAt(i);
            var label = $"{command.CommandId} - {command.CommandDescription}" +
                        (string.IsNullOrEmpty(command.CommandFormat) ? "" : $" - {command.CommandFormat}");

            var labelRect = new Rect(10f, commandViewHeight * i, helpRect.width - 40f, commandViewHeight);
            GUI.Label(labelRect, label, labelStyle);
        }

        GUI.EndScrollView();
        return scrollViewHeight; // Return the height occupied
    }

    /// <summary>
    ///     Renders the Input Field for entering commands.
    /// </summary>
    private void RenderInputField(float y, GUIStyle textFieldStyle)
    {
        GUI.Box(new Rect(0f, y, Screen.width, 50), "");
        GUI.backgroundColor = new Color(0, 0, 0, 0);
        _input = GUI.TextField(new Rect(10f, y + 5f, Screen.width - 20f, 50f), _input, textFieldStyle);
    }

    /// <summary>
    ///     Invoked when the user clicks the return key while the debugger is showing
    /// </summary>
    /// <param name="value"></param>
    private void OnReturn(InputValue value)
    {
        if (showConsole)
        {
            HandleInput(_input);
            _input = "";
        }
    }

    /// <summary>
    ///     Handles the user input after return is clicked
    /// </summary>
    /// <param name="input">the id of the command</param>
    private void HandleInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return;

        var splitInput = input.Split(' ');
        var commandId = splitInput[0];
        var args = splitInput.Length > 1 ? splitInput[1..] : new string[0]; // Remaining parts are arguments

        if (_commands.TryGetValue(commandId, out var command))
        {
            try
            {
                command.Invoke(args);
            }
            catch (Exception ex)
            {
                if (TESTING) Debug.LogError($"Error executing command {commandId}: {ex.Message}");
            }
        }
        else
        {
            if (TESTING) Debug.LogWarning($"Unknown command: {commandId}");
        }
    }

    /// <summary>
    ///     Instantiates the default commands
    ///     1. help
    ///     2. test
    ///     in the debugger
    /// </summary>
    private void RegisterDefaultCommands()
    {
        RegisterCommand(new DebugCommand("help", "list all available commands", "", ShowHelp));
        RegisterCommand(new DebugCommand("test", "test debuger is working", "", () => { print("working"); }));
    }

    /// <summary>
    ///     Adds the provided
    ///     <para>command</para>
    ///     to the debugger in the scene
    /// </summary>
    /// <param name="command">the command to be added</param>
    public void RegisterCommand(DebugCommand command)
    {
        if (_commands.TryAdd(command.CommandId, command))
        {
            if (TESTING) Debug.Log($"Registered debug command: {command.CommandId}"); // for testing only 
        }
        else
        {
            if (TESTING) Debug.LogWarning($"Command {command.CommandId} is already registered.");
        }
    }

    /// <summary>
    ///     the function to be called when the help command is triggered
    /// </summary>
    private void ShowHelp()
    {
        showHelp = !showHelp;
        var list = "Available Commands:\n";
        foreach (var command in _commands.Values) list += $"\t- {command.CommandId}\t \t{command.CommandDescription}\n";

        Debug.Log(list);
    }

    /// <summary>
    ///     Add a debug command to the debugger
    /// </summary>
    /// <param name="command">The command to be added</param>
    /// <code>AddDebugCommand(
    ///      new DebugCommand(
    ///      "delete_all_enemies",
    ///      "deletes all enemies",
    ///      "",
    ///      () => { print("deleted all"); })
    /// );</code>
    public void AddDebugCommand(DebugCommand command)
    {
        if (Instance != null)
        {
            Instance.RegisterCommand(command);
            if (TESTING) Debug.Log($"Command {command.CommandId} added.");
        }
        else
        {
            if (TESTING) Debug.LogWarning("No debug controller found.");
        }
    }
}