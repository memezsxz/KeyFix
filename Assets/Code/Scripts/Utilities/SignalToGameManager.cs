using System.Collections;
using System.Collections.Generic;
using Code.Scripts.Managers;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class SignalToGameManager : MonoBehaviour
{
    [SerializeField] private GameManager.Scenes scene = GameManager.Scenes.HALLWAYS;
    [SerializeField] private GameManager.GameState state = GameManager.GameState.Initial;

    public void Go()
    {
        GameManager.Instance.HandleSceneLoad(scene, state);
    }
    
}