using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerLever : MonoBehaviour, IInteractable
{
    [SerializeField] HintUI hintUI;
    private static readonly int IsPowerOn = Animator.StringToHash("is_power_on");
    Animator animator;
    private LeverState leverState = LeverState.down;

    private enum LeverState
    {
        freeze = -1,
        down = 0,
        up = 1
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public string InteractMessage
    {
        get { return "e"; }
    }
    
    public void Interact()
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

    public void ShowHint(bool show)
    {
        if (hintUI == null) return;

        if (show)
        {
            hintUI.ShowHint();
        }
        else
        {
            hintUI.HideHint();
        }
    }
}