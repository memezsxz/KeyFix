using System;
using System.Collections.Generic;
using Code.Scripts.Managers;
using Unity.VisualScripting;
using UnityEngine;


public class MazeController : Singleton<MazeController>
{
    [SerializeField] protected List<Light> _lights;
    [SerializeField] protected List<WallEntry> entries;

    public List<Light> Lights => _lights;
    private int currentIndex = 0;

    private void Start()
    {
        entries.ForEach(e =>
        {
            entries[currentIndex].board.ToggleCanvas(false);
            e.board.gameObject.SetActive(false);
            e.walls.SetActive(false);
        });

        entries[currentIndex].board.gameObject.SetActive(true);
        entries[currentIndex].board.enabled = true;
        entries[currentIndex].board.ToggleCanvas(true);
        entries[currentIndex].walls.SetActive(true);
    }


    public void BoardIsDone()
    {
        entries[currentIndex].board.gameObject.SetActive(false);
        entries[currentIndex].board.ToggleCanvas(false);
        entries[currentIndex].board.enabled = false;
        entries[currentIndex].walls.SetActive(false);

        currentIndex++;

        if (currentIndex >= entries.Count)
        {
            GameManager.Instance.TogglePlayerMovement(true);
            GameManager.Instance.ChangeState(GameManager.GameState.Victory);
            return;
        }

        entries[currentIndex].board.gameObject.SetActive(true);
        entries[currentIndex].board.ToggleCanvas(true);
        entries[currentIndex].board.enabled = true;
        entries[currentIndex].walls.SetActive(true);
        GameManager.Instance.TogglePlayerMovement(true);
    }
}


[Serializable]
public struct WallEntry
{
    public LightBoard board;
    public GameObject walls;
}