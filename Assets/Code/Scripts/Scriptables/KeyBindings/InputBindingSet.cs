using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject that defines a named set of input bindings.
/// Used for default control configuration.
/// </summary>
[Serializable]
[CreateAssetMenu(fileName = "InputBindingSet", menuName = "Input/InputBindingSet")]
public class InputBindingSet : ScriptableObject
{
    /// <summary>
    /// List of all input bindings defined in this set.
    /// </summary>
    public List<InputBindingConfig> bindings;
}

/// <summary>
/// Represents a single input action binding, including its path and label.
/// </summary>
[Serializable]
public class InputBindingConfig
{
    /// <summary>
    /// Name of the input action, e.g., "Move", "Jump".
    /// </summary>
    public string actionName;

    /// <summary>
    /// Name of the binding variant (used for multi-binding inputs like "up" or "left").
    /// Leave empty for single bindings.
    /// </summary>
    public string bindingName;

    /// <summary>
    /// The default input path for this binding, e.g., "&lt;Keyboard&gt;/w".
    /// </summary>
    public string defaultPath;

    /// <summary>
    /// Whether this binding is currently unlocked and editable.
    /// </summary>
    public bool isUnlocked = true; // TODO Maryam: make false for game, later on
}

/// <summary>
/// Serializable container for saving and loading a copy of input bindings.
/// </summary>
[Serializable]
public class SerializableInputBindingSet
{
    /// <summary>
    /// List of copied input bindings for saving/loading.
    /// </summary>
    public List<InputBindingConfig> bindings = new();
}