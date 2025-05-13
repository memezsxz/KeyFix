using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBindingManage : MonoBehaviour, IDataPersistence
{
    private PlayerInput _playerInput;

    // private InputBindingSet bindingSet;
    public SerializableInputBindingSet BindingSet { get; private set; }

    private void Awake()
    {
        if (GetComponent<PlayerMovement>()?.CharecterType != CharacterType.Robot)
        {
            // This is not the Robot — disable or destroy this component to prevent interfering with other players
            Destroy(this);
            return;
        }

        // print("PlayerBindingManage " + gameObject.name);
        _playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        SetupDebugCommand();
    }

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

    public void LoadData(ref SaveData data)
    {
        BindingSet = data.Progress.BindingOverrides;
        ApplyAllBindings();
        print("loading bindings " + BindingSet.bindings.Count);
    }

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

        if (BindingSet == null)
        {
            Debug.LogWarning("[PlayerBindingManage] bindingSet is null.");
            return;
        }

        foreach (var action in _playerInput.actions)
            action.RemoveAllBindingOverrides();

        foreach (var bindingConfig in BindingSet.bindings)
        {
            _playerInput.actions.Disable();

            ApplyBinding(bindingConfig);
            _playerInput.actions.Enable();
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
            binding = action.bindings
                .Select((b, i) => new { binding = b, index = i })
                .FirstOrDefault(b => b.binding.name == "" && b.binding.path == config.defaultPath);

        if (binding != null)
        {
            action.Disable();
            var path = config.isUnlocked ? config.defaultPath : " ";
            action.ApplyBindingOverride(binding.index, new InputBinding { overridePath = path });
            // print("applying binding " + config.bindingName + " to " + config.isUnlocked);
            action.Enable();
        }
        else
        {
            Debug.LogWarning($"Binding '{config.bindingName}' not found in action '{config.actionName}'.");
        }
    }

    public void EnableBinding(string actionName, string bindingName = "")
    {
        var config = BindingSet.bindings.FirstOrDefault(b =>
            b.actionName == actionName && b.bindingName == bindingName);

        if (config != null)
        {
            config.isUnlocked = true;
            ApplyBinding(config);
            // Debug.Log($"Enabled binding: {actionName}.{bindingName}");
        }
        // Debug.LogWarning($"Binding not found: {actionName}.{bindingName}");
    }

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
}