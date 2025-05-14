using UnityEngine;

/// <summary>
/// Handles interaction logic for hallway doors that either teleport the player into a scene or move them back out.
/// Inherits from <see cref="InteractableBase"/>.
/// </summary>
public class HallwayRoomDoor : InteractableBase
{
    #region Enums

    /// <summary>
    /// Specifies whether the door leads into a scene or exits the scene back to the hallway.
    /// </summary>
    public enum DoorType
    {
        Enter,
        Exit
    }

    #endregion

    #region Serialized Fields

    /// <summary>
    /// The scene this door connects to (only used for entering).
    /// </summary>
    [SerializeField] private GameManager.Scenes _scene;

    /// <summary>
    /// Determines whether this door is for entering or exiting.
    /// </summary>
    [SerializeField] private DoorType _doorType;

    /// <summary>
    /// The local forward offset direction for pushing the player on exit.
    /// </summary>
    [SerializeField] private Vector3 forwardPosition;

    #endregion

    #region Private Fields

    // The distance to push the player when exiting a room.
    private readonly float playerPush = 5;

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the target scene linked to this door.
    /// </summary>
    public GameManager.Scenes Scene => _scene;

    /// <summary>
    /// Gets the door type (Enter or Exit).
    /// </summary>
    public DoorType type => _doorType;

    #endregion

    #region Unity Methods

    private void Start()
    {
        // Exit doors should be on the default layer to not allow interaction
        if (_doorType == DoorType.Exit)
            gameObject.layer = LayerMask.NameToLayer("Default");
    }

    #endregion

    #region Interaction Logic

    /// <summary>
    /// Called when the player interacts with the door.
    /// </summary>
    public override void Interact()
    {
        if (_doorType == DoorType.Enter)
            HandleEnterDoor();
        else
            HandleExitDoor();
    }

    /// <summary>
    /// Handles entering the door: saves hallway position and loads the target scene.
    /// </summary>
    private void HandleEnterDoor()
    {
        SaveManager.Instance.SaveHallwayPosition();
        GameManager.Instance.HandleSceneLoad(_scene);
    }

    /// <summary>
    /// Handles exiting the door: moves the player to a position just outside the door.
    /// </summary>
    public void HandleExitDoor()
    {
        var pos = transform.position + forwardPosition * playerPush;
        GameManager.Instance.MovePlayerTo(pos, transform.rotation);
    }

    #endregion
}