using Code.Scripts.Managers;
using TMPro;
using UnityEngine;

public class LevelTimer : MonoBehaviour
{
    [Header("Timer Reference")] public float countdownTime = 20f;
    public TextMeshProUGUI timerText;
    public GameObject zapEffect; // assign your zap GameObject
    public AudioClip zapSound;

    [Header("Tick Sound Reference")] public AudioClip tickSound;

    [Header("Scene Reference")] public GameObject scene;

    [Header("Zaps Reference")] private float currentTime;
    private bool gameOverTriggered;
    private float nextTickTime;
    private float nextZapTime;
    private readonly float zapInterval = 4f;

    public void Start()
    {
        Room_A_Start();
    }

    private void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;

            var minutes = Mathf.FloorToInt(currentTime / 60);
            var seconds = Mathf.FloorToInt(currentTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            if (currentTime <= nextZapTime)
            {
                ShowZap();
                nextZapTime -= zapInterval;
            }

            if (currentTime <= nextTickTime && currentTime > 0f)
            {
                if (!SoundManager.Instance.IsMusicPlaying) SoundManager.Instance.PlayMusic(tickSound);
                nextTickTime = Mathf.Floor(currentTime) - 1f;
            }
        }
        else if (!gameOverTriggered)
        {
            timerText.text = "00:00";
            gameOverTriggered = true;
            StopAllEffects();

            GameManager.Instance.ChangeState(GameManager.GameState.GameOver);
        }
    }

    public void Room_A_Start()
    {
        scene.SetActive(true);
        currentTime = countdownTime;
        zapEffect.SetActive(false);
        nextZapTime = currentTime - zapInterval;
        nextTickTime = Mathf.Floor(currentTime) - 1f;
    }

    private void ShowZap()
    {
        zapEffect.SetActive(true);
        if (zapSound != null) SoundManager.Instance.PlaySound(zapSound);

        Invoke("HideZap", 1f); // show zap briefly
    }

    private void HideZap()
    {
        zapEffect.SetActive(false);
    }

    private void StopAllEffects()
    {
        if (SoundManager.Instance.IsSoundPlaying)
            SoundManager.Instance.StopSound();
        if (SoundManager.Instance.IsMusicPlaying)
            SoundManager.Instance.StopMusic();

        if (zapEffect.activeSelf)
            zapEffect.SetActive(false);
    }
}