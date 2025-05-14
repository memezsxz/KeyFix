/// <summary>
/// Tracks global state related to scene transitions and player progress.
/// Useful for persisting flags like whether the player is returning from gameplay.
/// </summary>
public static class GameStateTracker
{
    /// <summary>
    /// Indicates whether the player is returning to the main menu from gameplay.
    /// Can be used to trigger animations, UI changes, or skip intro sequences.
    /// </summary>
    public static bool returningFromGame = false;
}