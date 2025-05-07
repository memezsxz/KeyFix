using Code.Scripts.Managers;
using UnityEngine;

namespace Code.Scripts.Interactable
{
    public class LightChallengeObject : InteractableBase
    {
        bool isActive = false;
        [SerializeField] private GameObject challengePrefab;
        GameObject challengeInstance;
        ShapeChecker shapeChecker;

        public override void Interact()
        {
            if (!isActive)
            {
                if (!challengeInstance)
                {
                    challengeInstance = Instantiate(challengePrefab, transform.position, Quaternion.identity);
                    shapeChecker = challengeInstance.GetComponentInChildren<ShapeChecker>();
                    shapeChecker.successCallback += HandleWin;
                }

                challengeInstance.SetActive(true);
                isActive = true;
                GameManager.Instance.TogglePlayerMovement(false);
            }
            else
            {
                isActive = false;
                challengeInstance.SetActive(false);
                GameManager.Instance.TogglePlayerMovement(true);
            }
        }

        private void HandleWin()
        {
            isActive = false;
            challengeInstance.SetActive(false);

            gameObject.layer = LayerMask.NameToLayer("Default");
            GameManager.Instance.TogglePlayerMovement(true);
            InteractionChallengeManager.Instance.IncrementScore();
        }
    }
}