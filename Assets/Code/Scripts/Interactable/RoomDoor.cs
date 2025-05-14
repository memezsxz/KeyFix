/// <summary>
/// An interactable door used inside rooms to return the player to the Hallways scene.
/// Prevents multiple interactions from triggering multiple loads.
/// </summary>
public class RoomDoor : InteractableBase
{
    #region Private Fields

    /// <summary>
    /// Ensures the interaction logic only executes once.
    /// </summary>
    private bool didInteract;

    #endregion

    #region Interaction Logic

    /// <summary>
    /// Base interaction method (not used here).
    /// </summary>
    public override void Interact()
    {
        // Intentionally left empty – only InteractH() is used for this door
    }

    /// <summary>
    /// Handles a special/hint interaction to return the player to the hallway scene.
    /// </summary>
    public override void InteractH()
    {
        if (didInteract) return; // Prevent repeated interaction

        didInteract = true;

        // Load the Hallways scene and set the game state to Playing
        GameManager.Instance.HandleSceneLoad(GameManager.Scenes.HALLWAYS, GameManager.GameState.Playing);
    }

    #endregion
}