using UnityEngine;

public class SignalToGameManager : MonoBehaviour
{
    public void PlayNextCutScene()
    {
        GameManager.Instance.ChangeState(GameManager.GameState.CutScene);
    }
}