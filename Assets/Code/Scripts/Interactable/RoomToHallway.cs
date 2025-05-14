
public class RoomToHallway : InteractableBase
{
    public override void Interact()
    {
        GameManager.Instance.ChangeState(GameManager.GameState.Victory);
    }
}