using UnityEngine;

public class ControllLightChanllenge : MonoBehaviour
{
    [SerializeField] private GameObject challengeScene;


    public void showChallenge()
    {
        challengeScene.SetActive(true);
    }
}