using Code.Scripts.Managers;

public class DoorInteract : InteractableBase
{
    public override void Interact()
    {
        GameManager.Instance.ChangeState(GameManager.GameState.Victory);
    }
}