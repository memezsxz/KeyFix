using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles saving, loading, and applying custom input bindings for the Robot player.
/// Also supports runtime updates and debug commands.
/// </summary>
public class PlayerBindingManage : MonoBehaviour, IDataPersistence
{
    #region Fields & Properties

    private PlayerInput _playerInput;

    /// <summary>
    /// Serializable container holding overridden input bindings for saving.
    /// </summary>
    public SerializableInputBindingSet BindingSet { get; private set; }

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        // Ensure this is only active on the Robot character
        if (GetComponent<PlayerMovement>()?.CharecterType != CharacterType.Robot)
        {
            Destroy(this);
            return;
        }

        _playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        SetupDebugCommand();
    }

    #endregion

    #region Save System

    /// <summary>
    /// Saves the current binding set into the game's save data.
    /// </summary>
    public void SaveData(ref SaveData data)
    {
        if (BindingSet == null)
        {
            Debug.LogWarning("Binding set is null. Nothing to save.");
            return;
        }

        data.Progress.BindingOverrides = BindingSet;
        print("saving bindings");
    }

    /// <summary>
    /// Loads bindings from save data and applies them to the player's input system.
    /// </summary>
    public void LoadData(ref SaveData data)
    {
        BindingSet = data.Progress.BindingOverrides;
        ApplyAllBindings();
        print("loading bindings " + BindingSet.bindings.Count);
    }

    #endregion

    #region Binding Application

    /// <summary>
    /// Applies all bindings in the current BindingSet to the PlayerInput component.
    /// </summary>
    public void ApplyAllBindings()
    {
        if (_playerInput == null)
            _playerInput = GetComponent<PlayerInput>();

        if (_playerInput == null)
        {
            Debug.LogWarning("[PlayerBindingManage] PlayerInput still null.");
            return;
        }

        if (BindingSet == null)
        {
            BindingSet = SaveManager.Instance.SaveData.Progress.BindingOverrides;
            return;
        }

        // Clear all existing overrides first
        foreach (var action in _playerInput.actions)
            action.RemoveAllBindingOverrides();

        // Apply each saved binding override
        foreach (var bindingConfig in BindingSet.bindings)
        {
            _playerInput.actions.Disable(); // Prevent update conflict
            ApplyBinding(bindingConfig);
            _playerInput.actions.Enable();
        }
    }

    /// <summary>
    /// Applies a specific binding override based on a config entry.
    /// </summary>
    /// <param name="config">The input binding configuration to apply.</param>
    private void ApplyBinding(InputBindingConfig config)
    {
        if (_playerInput == null) return;

        var action = _playerInput.actions[config.actionName];
        if (action == null)
        {
            Debug.LogWarning($"Action '{config.actionName}' not found.");
            return;
        }

        // Try matching a composite binding (e.g., Move.up)
        var binding = action.bindings
            .Select((b, i) => new { binding = b, index = i })
            .FirstOrDefault(b => b.binding.name == config.bindingName && b.binding.isPartOfComposite);

        // Otherwise match simple binding like Jump
        if (binding == null)
        {
            binding = action.bindings
                .Select((b, i) => new { binding = b, index = i })
                .FirstOrDefault(b => b.binding.name == "" && b.binding.path == config.defaultPath);
        }

        // Apply override if binding is found
        if (binding != null)
        {
            action.Disable();

            // Locked bindings are disabled by overriding with an empty string
            var path = config.isUnlocked ? config.defaultPath : " ";
            action.ApplyBindingOverride(binding.index, new InputBinding { overridePath = path });

            action.Enable();
        }
        else
        {
            Debug.LogWarning($"Binding '{config.bindingName}' not found in action '{config.actionName}'.");
        }
    }

    /// <summary>
    /// Enables a specific binding in the set and reapplies it to the input system.
    /// </summary>
    /// <param name="actionName">The name of the input action.</param>
    /// <param name="bindingName">The specific binding variant (optional).</param>
    public void EnableBinding(string actionName, string bindingName = "")
    {
        var config = BindingSet.bindings.FirstOrDefault(b =>
            b.actionName == actionName && b.bindingName == bindingName);

        if (config != null)
        {
            config.isUnlocked = true;
            ApplyBinding(config);
        }
    }

    #endregion

    #region Debug Commands

    /// <summary>
    /// Registers debug command to enable or disable input bindings at runtime.
    /// </summary>
    private void SetupDebugCommand()
    {
        DebugController.Instance?.AddDebugCommand(new DebugCommand(
            "update_binding",
            "Update a binding by name",
            "update_binding <action> <binding> <true/false>",
            args =>
            {
                if (args.Length < 3)
                {
                    Debug.LogWarning("Usage: update_binding <action> <binding> <bool>");
                    return;
                }

                var config = BindingSet.bindings
                    .FirstOrDefault(b => b.actionName == args[0] && b.bindingName == args[1]);

                if (config == null)
                {
                    Debug.LogWarning("Binding not found.");
                    return;
                }

                if (!bool.TryParse(args[2], out var value))
                {
                    Debug.LogWarning("Value must be true or false.");
                    return;
                }

                config.isUnlocked = value;
                ApplyBinding(config);
            }
        ));
    }

    #endregion

    #region Scriptable Binding Support

    /// <summary>
    /// Loads binding settings from a scriptable asset and applies them.
    /// </summary>
    /// <param name="source">InputBindingSet ScriptableObject to read from.</param>
    public void ApplyFromScriptableObject(InputBindingSet source)
    {
        BindingSet = new SerializableInputBindingSet
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

    #endregion
}
