using System;
using System.Collections.Generic;
using Code.Scripts.Managers;
using UnityEngine;


public class MazeController : Singleton<MazeController>
{
    [SerializeField] protected List<Light> _lights;
    [SerializeField] protected List<WallEntry> entries;

    public List<Light> Lights => _lights;
    private int currentIndex = 0;

    private void Start()
    {
        entries.ForEach(e => { SetEntryActive(e, false); });

        SetEntryActive(entries[currentIndex], true);
    }


    public void BoardIsDone()
    {
        SetEntryActive(entries[currentIndex], false);

        currentIndex++;

        if (currentIndex >= entries.Count)
        {
            GameManager.Instance.TogglePlayerMovement(true);
            GameManager.Instance.ChangeState(GameManager.GameState.Victory);
            return;
        }

        SetEntryActive(entries[currentIndex], true);
        GameManager.Instance.TogglePlayerMovement(true);
    }

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
}


[Serializable]
public struct WallEntry
{
    public LightBoard board;
    public GameObject walls;
}