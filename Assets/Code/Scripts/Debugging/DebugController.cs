using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


class DebugController : MonoBehaviour
{
    [SerializeField] private bool showConsole;
    private string _input;

    private readonly Dictionary<string, DebugCommand> _commands = new Dictionary<string, DebugCommand>();

    private void Start()
    {
        RegisterDefaultCommands();
    }

    private void OnToggleDebug()
    {
        showConsole = !showConsole;
    }

    private void OnGUI()
    {
        if (!showConsole) return; // if the console is not triggered, do not paint anything

        float y = 0f;

        GUIStyle textFieldStyle = new GUIStyle(GUI.skin.textField) // setup the style for the textfield
        {
            fontSize = 30,
            normal =
            {
                textColor = Color.white
            }
        };

        // create the textfield and set it up wiht the _input variable
        GUI.Box(new Rect(0f, y, Screen.width, 50), "");
        GUI.backgroundColor = new Color(0, 0, 0, 0);
        _input = GUI.TextField(new Rect(10f, y + 5f, Screen.width - 20f, 50f), _input, textFieldStyle);
    }

    /// <summary>
    /// Invoked when the user clicks the return key while the debugger is showing
    /// </summary>
    /// <param name="value"></param>
    private void OnReturn(InputValue value)
    {
        if (showConsole)
        {
            HandelInput(_input);
            _input = "";
        }
    }

    /// <summary>
    /// Handles the user input after return is clicked
    /// </summary>
    /// <param name="commandId">the id of the command</param>
    private void HandelInput(string commandId)
    {
        DebugCommand command;
        _commands.TryGetValue(commandId, out command);
        command?.Invoke();
    }

    /// <summary>
    /// Instantiates the default commands
    ///     1. help
    ///     2. test
    /// in the debugger
    /// </summary>
    private void RegisterDefaultCommands()
    {
        RegisterCommand(new DebugCommand("help", "list all available commands", "", ShowHelp));
        RegisterCommand(new DebugCommand("test", "test debuger is working", "", () => { print("working"); }));
    }

    /// <summary>
    /// Adds the provided <para>command</para> to the debugger in the scene
    /// </summary>
    /// <param name="command">the command to be added</param>
    public void RegisterCommand(DebugCommand command)
    {
        if (_commands.TryAdd(command.CommandId, command))
        {
            // Debug.Log($"Registered debug command: {command.CommandId}"); // for testing only 
        }
        else
        {
            Debug.LogWarning($"Command {command.CommandId} is already registered.");
        }
    }

    /// <summary>
    /// the function to be called when the help command is triggered
    /// </summary>
    private void ShowHelp()
    {
        string list = "Available Commands:\n";
        foreach (DebugCommand command in _commands.Values)
        {
            list += ($"\t- {command.CommandId}\t \t{command.CommandDescription}\n");
        }

        Debug.Log(list);
    }
}