using System;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "InputBindingSet", menuName = "Input/InputBindingSet")]
public class InputBindingSet : ScriptableObject
{
    public List<InputBindingConfig> bindings;
}
