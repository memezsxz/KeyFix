using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBindingManage : MonoBehaviour
{
    [SerializeField] private InputBindingSet bindingSet;
    
    public InputBindingSet BindingSet => bindingSet;

    private PlayerInput _playerInput;

    private void Awake()
    {
        foreach (var inputBindingConfig in bindingSet.bindings)
        {
            inputBindingConfig.isUnlocked = true;
        } // TODO Maryam: make false for game, later on

        _playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        ApplyAllBindings();
        SetupDebugCommand();
    }

    public void ApplyAllBindings()
    {
        foreach (var bindingConfig in bindingSet.bindings)
        {
            ApplyBinding(bindingConfig);
        }
    }

    private void ApplyBinding(InputBindingConfig config)
    {
        var action = _playerInput.actions[config.actionName];
        if (action == null)
        {
            Debug.LogWarning($"Action '{config.actionName}' not found.");
            return;
        }

        // for composit binding like Move.up
        var binding = action.bindings
            .Select((b, i) => new { binding = b, index = i })
            .FirstOrDefault(b => b.binding.name == config.bindingName && b.binding.isPartOfComposite);

        // for simple bindings like jump
        if (binding == null)
        {
            binding = action.bindings
                .Select((b, i) => new { binding = b, index = i })
                .FirstOrDefault(b => b.binding.name == "" && b.binding.path == config.defaultPath);
        }

        if (binding != null)
        {
            var path = config.isUnlocked ? config.defaultPath : " ";
            action.ApplyBindingOverride(binding.index, new InputBinding { overridePath = path });
        }
        else
        {
            Debug.LogWarning($"Binding '{config.bindingName}' not found in action '{config.actionName}'.");
        }
    }

    private void SetupDebugCommand()
    {
        DebugController.Instance?.AddDebugCommand(new DebugCommand(
            "update_binding",
            "Update a binding by name",
            "update_binding <action> <binding> <true/false>",
            (args) =>
            {
                if (args.Length < 3)
                {
                    Debug.LogWarning("Usage: update_binding <action> <binding> <bool>");
                    return;
                }

                var config = bindingSet.bindings
                    .FirstOrDefault(b => b.actionName == args[0] && b.bindingName == args[1]);

                if (config == null)
                {
                    Debug.LogWarning("Binding not found.");
                    return;
                }

                if (!bool.TryParse(args[2], out bool value))
                {
                    Debug.LogWarning("Value must be true or false.");
                    return;
                }

                config.isUnlocked = value;
                ApplyBinding(config);
            }
        ));
    }
}
