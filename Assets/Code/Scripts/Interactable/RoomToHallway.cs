/// <summary>
/// Simple interactable used to trigger a victory state when the player exits a room.
/// </summary>
public class RoomToHallway : InteractableBase
{
    #region Interaction Logic

    /// <summary>
    /// Called when the player interacts with the object.
    /// Triggers a transition to the Victory game state.
    /// </summary>
    public override void Interact()
    {
        // Change the game state to Victory when interacted with
        GameManager.Instance.ChangeState(GameManager.GameState.Victory);
    }

    #endregion
}