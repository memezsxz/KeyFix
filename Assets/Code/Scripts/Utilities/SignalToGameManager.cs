using System.Collections;
using System.Collections.Generic;
using Code.Scripts.Managers;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class SignalToGameManager : MonoBehaviour
{
    public void Go()
    {
        GameManager.Instance.ChangeState(GameManager.GameState.Victory);
    }
}