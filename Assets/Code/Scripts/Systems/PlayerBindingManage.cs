using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBindingManage : Singleton<PlayerBindingManage>, IDataPersistence 
{
    // private InputBindingSet bindingSet;
    public SerializableInputBindingSet BindingSet => bindingSet;

    private PlayerInput _playerInput;
    [SerializeField] private InputBindingSet defaultSet; // For fallback

    private SerializableInputBindingSet bindingSet;

    private void Awake()
    {
        if (GetComponent<PlayerMovement>()?.CharecterType != CharacterType.Robot)
        {
            // This is not the Robot — disable or destroy this component to prevent interfering with other players
            Destroy(this);
            return;
        }

        _playerInput = GetComponent<PlayerInput>();
 
    }

    private void Start()
    {

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
        if (_playerInput == null) return;
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

    public void EnableBinding(string actionName, string bindingName = "")
    {
        var config = bindingSet.bindings.FirstOrDefault(b =>
            b.actionName == actionName && b.bindingName == bindingName);

        if (config != null)
        {
            config.isUnlocked = true;
            ApplyBinding(config);
            Debug.Log($"Enabled binding: {actionName}.{bindingName}");
        }
        else
        {
            Debug.LogWarning($"Binding not found: {actionName}.{bindingName}");
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

    public void SaveData(ref SaveData data)
    {
        if (bindingSet == null)
        {
            Debug.LogWarning("Binding set is null. Nothing to save.");
            return;
        }

        data.Progress.BindingOverrides = bindingSet;
        print("saving bindings");
    }

    public void LoadData(ref SaveData data)
    {
        
        bindingSet  = data.Progress.BindingOverrides;
        ApplyAllBindings();
        print("loading bindings " + bindingSet.bindings.Count);
    }
    
    
    public void ApplyFromScriptableObject(InputBindingSet source)
    {
        bindingSet = new SerializableInputBindingSet
        {
            bindings = source.bindings.Select(b => new InputBindingConfig
            {
                actionName = b.actionName,
                bindingName = b.bindingName,
                defaultPath = b.defaultPath,
                isUnlocked = b.isUnlocked
            }).ToList()
        };

        ApplyAllBindings();
    }
}