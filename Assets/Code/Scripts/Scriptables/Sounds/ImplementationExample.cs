using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SoundsSOHolder))]
public class ImplementationExample : MonoBehaviour
{
    private SoundsSOHolder soundsSOHolder;

    private void Start()
    {
        soundsSOHolder = GetComponent<SoundsSOHolder>();
        DebugController.Instance.AddDebugCommand(new DebugCommand(
            "playSound",
            "playes the sound with the provided index in the soundholder",
            "PlaySound <int>",
            (args) =>
            {
                int index = int.Parse((string)args[0]);

           SoundManager.Instance.PlaySound(soundsSOHolder.GetRandomSound(index));      ;
            }));
    }
}