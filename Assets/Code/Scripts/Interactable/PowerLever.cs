using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerLever : InteractableBase
{
    private static readonly int IsPowerOn = Animator.StringToHash("is_power_on");
    Animator animator;
    private LeverState leverState = LeverState.down;

    private enum LeverState
    {
        freeze = -1,
        up = 0,
        down = 1
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        InteractMessage = "E";
    }


    public override void Interact()
    {
        ToggleState();
    }

    private void ToggleState()
    {
        if (leverState == LeverState.down)
        {
            leverState = LeverState.freeze;
            SetAnimatorState();
            leverState = LeverState.up;
        }
        else if (leverState == LeverState.up)
        {
            leverState = LeverState.freeze;
            SetAnimatorState();
            leverState = LeverState.down;
        }

        SetAnimatorState();
    }

    private void SetAnimatorState()
    {
        animator.SetInteger(IsPowerOn, (int)leverState);
    }
}