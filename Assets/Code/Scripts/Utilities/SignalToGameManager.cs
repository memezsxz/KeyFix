using UnityEngine;

/// <summary>
/// Provides a trigger mechanism for advancing the game state to a cutscene.
/// Typically called from animation events or interactables.
/// </summary>
public class SignalToGameManager : MonoBehaviour
{
    /// <summary>
    /// Instructs the GameManager to transition the game into the CutScene state.
    /// </summary>
    public void PlayNextCutScene()
    {
        GameManager.Instance.ChangeState(GameManager.GameState.CutScene);
    }
}