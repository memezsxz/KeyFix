using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerLever : MonoBehaviour, IInteractable
{
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

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        print("collition");
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleState();
            // SetAnimatorState();
        }
    }

    public void Interact()
    {
        print("PowerLever Interact");
    }
    
    private void ToggleState()
    {
        // leverState = LeverState.freeze;
        // SetAnimatorState();

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