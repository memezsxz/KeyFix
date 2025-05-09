using UnityEngine;

public abstract class InteractableBase : MonoBehaviour
{
    [SerializeField] protected HintUI hintUI;

    public string InteractMessage
    {
        get => hintUI == null ? "" : hintUI.message;
        protected set => hintUI.message = value;
    }

    public abstract void Interact();

    public virtual void InteractH()
    {
        
    }
    public virtual void ShowHint(bool show)
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