using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;




[Serializable]
public class InputBindingConfig
{
    public string actionName; // e.g., "Move", "Jump"
    public string bindingName; // e.g., "up", "left", or "" for simple actions
    public string defaultPath; // e.g., "<Keyboard>/w"
    public bool isUnlocked = true; // TODO Maryam: make false for game, later on
}
