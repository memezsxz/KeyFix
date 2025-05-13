using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "InputBindingSet", menuName = "Input/InputBindingSet")]
public class InputBindingSet : ScriptableObject
{
    public List<InputBindingConfig> bindings;
}

[Serializable]
public class InputBindingConfig
{
    public string actionName; // e.g., "Move", "Jump"
    public string bindingName; // e.g., "up", "left", or "" for simple actions
    public string defaultPath; // e.g., "<Keyboard>/w"
    public bool isUnlocked = true; // TODO Maryam: make false for game, later on
}


[Serializable]
public class SerializableInputBindingSet
{
    public List<InputBindingConfig> bindings = new();
}