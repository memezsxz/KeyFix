using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelTimer : MonoBehaviour
{
    [Header("Timer Reference")] public float countdownTime = 20f;
    public TextMeshProUGUI timerText;
    public GameObject zapEffect; // assign your zap GameObject
    public AudioClip zapSound;

    [Header("Zaps Reference")] private float currentTime;
    private float zapInterval = 4f;
    private float nextZapTime = 0f;

    [Header("Tick Sound Reference")] 
    public AudioClip tickSound;
    private float nextTickTime = 0f;

    [Header("Scene Reference")] public GameObject scene;
    public GameObject gameOverScene;
    private bool gameOverTriggered = false;

    public void Start()
    {
        Room_A_Start();
    }

    public void Room_A_Start()
    {
        scene.SetActive(true);
        currentTime = countdownTime;
        zapEffect.SetActive(false);
        nextZapTime = currentTime - zapInterval;
        nextTickTime = Mathf.Floor(currentTime) - 1f;
    }

    void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;

            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
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

            StartCoroutine(HandleGameOver());
        }
    }

    void ShowZap()
    {
        zapEffect.SetActive(true);
        if (zapSound != null) SoundManager.Instance.PlaySound(zapSound);

        Invoke("HideZap", 1f); // show zap briefly
    }

    void HideZap()
    {
        zapEffect.SetActive(false);
    }

    void StopAllEffects()
    {
        if (SoundManager.Instance.IsSoundPlaying)
            SoundManager.Instance.StopSound();
        if (SoundManager.Instance.IsMusicPlaying)
            SoundManager.Instance.StopMusic();

        if (zapEffect.activeSelf)
            zapEffect.SetActive(false);
    }

    IEnumerator HandleGameOver()
    {
        yield return new WaitForSeconds(0.3f);
        gameOverScene.SetActive(true);
    }
}