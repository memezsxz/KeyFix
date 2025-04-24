using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelTimer : MonoBehaviour
{
    //timmer refrence
    public float countdownTime = 20f;
    public TextMeshProUGUI timerText;
    public GameObject zapEffect; // assign your zap GameObject
    public AudioSource zapSound;

    //zaps reference
    private float currentTime;
    private float zapInterval = 4f;
    private float nextZapTime = 0f;

    //tick sound referenc
    public AudioSource tickSound; 
    private float nextTickTime = 0f;

    //scene reference
    public GameObject scene;
    public GameObject gameOverScene;
    private bool gameOverTriggered = false;

    public void Room_A_Start() {
        scene.SetActive(true);
        currentTime = countdownTime;
        zapEffect.SetActive(false);
        nextZapTime = currentTime - zapInterval;
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

            if (currentTime <= nextTickTime)
            {
                tickSound.Play();
                nextTickTime = Mathf.Floor(currentTime) - 1;
            }
        }
        else if(!gameOverTriggered)
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
        if (zapSound != null) zapSound.Play();

        Invoke("HideZap", 1f); // show zap briefly
    }

    void HideZap()
    {
        zapEffect.SetActive(false);
    }


    void StopAllEffects()
    {
        if (tickSound.isPlaying)
            tickSound.Stop();

        if (zapEffect.activeSelf)
            zapEffect.SetActive(false);
    }


    IEnumerator HandleGameOver()
    {
      
        yield return new WaitForSeconds(1f);
        gameOverScene.SetActive(true);
    }
}
