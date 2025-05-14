using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the sequence of color-matching boards and wall gates in a puzzle maze.
/// Progresses the player through entries and triggers victory upon completion.
/// </summary>
public class MazeController : Singleton<MazeController>
{
    #region Serialized Fields

    /// <summary>
    /// List of lights that can be manipulated during the challenge.
    /// These are affected by color matching logic.
    /// </summary>
    [SerializeField] protected List<Light> _lights;

    /// <summary>
    /// Ordered list of wall and board combinations that define puzzle steps.
    /// </summary>
    [SerializeField] protected List<WallEntry> entries;

    #endregion

    #region Private Fields

    /// <summary>
    /// Index of the currently active puzzle board in the sequence.
    /// </summary>
    private int currentIndex;

    #endregion

    #region Properties

    /// <summary>
    /// Public access to the lights used in the puzzle.
    /// </summary>
    public List<Light> Lights => _lights;

    #endregion

    #region Unity Methods

    private void Start()
    {
        // Disable all entries first
        entries.ForEach(e => { SetEntryActive(e, false); });

        // Enable the first board
        SetEntryActive(entries[currentIndex], true);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Called when the player completes the current board.
    /// Advances to the next entry or ends the puzzle with victory.
    /// </summary>
    public void BoardIsDone()
    {
        // Deactivate the current entry
        SetEntryActive(entries[currentIndex], false);

        currentIndex++;

        // If all boards are done, end puzzle with victory
        if (currentIndex >= entries.Count)
        {
            GameManager.Instance.TogglePlayerMovement(true);
            GameManager.Instance.ChangeState(GameManager.GameState.Victory);
            return;
        }

        // Activate the next puzzle entry
        SetEntryActive(entries[currentIndex], true);
        GameManager.Instance.TogglePlayerMovement(true);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Enables or disables both the puzzle board and wall associated with the entry.
    /// </summary>
    private void SetEntryActive(WallEntry entry, bool isActive)
    {
        if (entry.board != null)
        {
            entry.board.gameObject.SetActive(isActive);
            entry.board.enabled = isActive;
            entry.board.ToggleCanvas(isActive);
        }

        if (entry.walls != null)
        {
            entry.walls.SetActive(isActive);
        }
    }

    #endregion
}

/// <summary>
/// Represents a pair of objects involved in a puzzle step: a board and its associated walls.
/// </summary>
[Serializable]
public struct WallEntry
{
    /// <summary>
    /// The interactable board for this step of the puzzle.
    /// </summary>
    public LightBoard board;

    /// <summary>
    /// The GameObject representing walls that surround or reveal the board.
    /// </summary>
    public GameObject walls;
}