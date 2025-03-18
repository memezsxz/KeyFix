using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.OpenVR;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugController : MonoBehaviour
{
    [SerializeField] private bool showConsole;
    private string _input;

    private Dictionary<string, DebugCommand> _commands = new Dictionary<string, DebugCommand>();

    private void Start()
    {
        RegisterDefaultCommands();
    }

    private void OnToggleDebug(InputValue value)
    {
        showConsole = !showConsole;
    }

    private void OnGUI()
    {
        if (!showConsole) return;

        float y = 0f;
        GUI.Box(new Rect(0f, y, Screen.width, 30), "");
        GUI.backgroundColor = new Color(0, 0, 0, 0);
        _input = GUI.TextField(new Rect(10f, y + 5f, Screen.width - 20f, 20f), _input);
    }

    private void OnReturn(InputValue value)
    {
        if (showConsole)
        {
            HandedInput(_input);
            _input = "";
        }
    }

    private void HandedInput(string command)
    {
        _commands[command]?.Invoke();
    }
    
    private void RegisterDefaultCommands()
    {
        RegisterCommand(new DebugCommand("help", "list all available commands", "", ShowHelp));
        RegisterCommand(new DebugCommand("test", "test debuger is working", "", () => {print("working");}));
    }
    public void RegisterCommand(DebugCommand command)
    {
        if (_commands.TryAdd(command.CommandId, command))
        {
            Debug.Log($"Registered debug command: {command.CommandId}");
        }
        else
        {
            Debug.LogWarning($"Command {command.CommandId} is already registered.");
        }
    }

    private void ShowHelp()
    {
        Debug.Log("Available Commands:");
        foreach (var command in _commands.Keys)
        {
            Debug.Log($"- {command}");
        }
    }
}