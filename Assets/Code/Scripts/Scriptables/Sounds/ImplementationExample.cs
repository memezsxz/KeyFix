using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SoundsHolder))]
public class ImplementationExample : MonoBehaviour
{
    private SoundsHolder _soundsHolder;

    private void Start()
    {
        _soundsHolder = GetComponent<SoundsHolder>();
        DebugController.Instance.AddDebugCommand(new DebugCommand(
            "playSound",
            "Playes the sound with the provided index in the soundholder",
            "PlaySound <int>",
            (args) =>
            {
                int index = int.Parse((string)args[0]);
                _soundsHolder.PlaySound(index);      ;
            }));
    }

    private void OnDisable()
    {
        _soundsHolder.PlaySound(0);
    }

    private void OnEnable()
    {
        _soundsHolder.PlaySound("click");
    }
}
