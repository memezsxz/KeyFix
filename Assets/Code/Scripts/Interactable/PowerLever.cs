using Code.Scripts.Managers;
using UnityEngine;

public class PowerLever : InteractableBase
{
    private static readonly int IsPowerOn = Animator.StringToHash("is_power_on");
    private Animator animator;
    private LeverState leverState = LeverState.down;

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

        if (leverState == LeverState.down) Invoke(nameof(TriggerVictory), 2f);
    }


    private void TriggerVictory()
    {
        GameManager.Instance.ChangeState(GameManager.GameState.Victory);
    }

    private void SetAnimatorState()
    {
        animator.SetInteger(IsPowerOn, (int)leverState);
    }

    private enum LeverState
    {
        freeze = -1,
        down = 0,
        up = 1
    }
}