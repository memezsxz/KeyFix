using System;
using TMPro;
using UnityEngine;

namespace Code.Scripts.Managers
{
    public class InteractionChallengeManager : Singleton<InteractionChallengeManager>
    {
        [SerializeField] TextMeshProUGUI scoreText;
        private int currentScore = 0;
        [SerializeField] private int maxScore = 5;

        private void Start()
        {
            IncrementScore();
        }

        public void IncrementScore()
        {
            scoreText.text = $"{++currentScore} / {maxScore}";

            if (currentScore >= maxScore)
            {
                GameManager.Instance.ChangeState(GameManager.GameState.Victory);
            }
        }
    }
}