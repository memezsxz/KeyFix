using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Code.Scripts.Managers
{
    public class InteractionChallengeManager : Singleton<InteractionChallengeManager>, IDataPersistence
    {
        [SerializeField] private TextMeshProUGUI scoreText;

        [SerializeField] private List<GameObject> interactables;
        private int currentScore;

        private void Start()
        {
            UpdateUI();
            EnableNextItem();
        }

        private void OnEnable()
        {
            UpdateUI();
            EnableNextItem();
        }

        public void SaveData(ref SaveData data)
        {
            data.Progress.CollectablesCount = currentScore;
        }

        public void LoadData(ref SaveData data)
        {
            print("Loading data in w room");
            currentScore = data.Progress.CollectablesCount;
            UpdateUI();
            EnableNextItem();
        }

        private void EnableNextItem()
        {
            var defaultLayer = LayerMask.NameToLayer("Default");
            var interactableLayer = LayerMask.NameToLayer("Interactable");

            interactables.ForEach(i => SetLayerRecursively(i, defaultLayer));
            SetLayerRecursively(interactables[currentScore], interactableLayer);
        }

        private void SetLayerRecursively(GameObject obj, int newLayer)
        {
            if (obj == null) return;

            obj.layer = newLayer;

            foreach (Transform child in obj.transform) SetLayerRecursively(child.gameObject, newLayer);
        }

        public void IncrementScore()
        {
            currentScore++;
            UpdateUI();

            if (currentScore >= interactables.Count)
                GameManager.Instance.ChangeState(GameManager.GameState.Victory);
            else
                EnableNextItem();
        }

        private void UpdateUI()
        {
            scoreText.text = $"{currentScore} / {interactables.Count}";
        }
    }
}