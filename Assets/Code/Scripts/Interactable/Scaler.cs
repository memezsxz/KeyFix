using System;
using System.Collections;
using Code.Scripts.Managers;
using UnityEngine;

namespace Code.Scripts.Interactable
{
    public class Scaler : MonoBehaviour
    {
        public enum ScaleType
        {
            Shrink,
            Stretch
        }

        [SerializeField] private ScaleType scaleType;
        private bool hasTriggered = false;
        private float timeInSeconds = 2;

        private void OnTriggerEnter(Collider other)
        {
            if (hasTriggered || !other.CompareTag("Player")) return;
            hasTriggered = true;

            var controller = other.GetComponent<CharacterController>();
            var scaler = other.GetComponent<ScalerInteraction>();

            if (controller != null && scaler != null)
            {
                bool alreadyAtTarget = scaleType switch
                {
                    ScaleType.Shrink => scaler.IsAlreadyShrunk(),
                    ScaleType.Stretch => scaler.IsAlreadyStretched(),
                    _ => true
                };

                if (alreadyAtTarget)
                {
                    GameManager.Instance.TogglePlayerMovement(true);
                    return;
                }

                Action<float, Action> performScale = scaleType switch
                {
                    ScaleType.Shrink => scaler.Shrink,
                    ScaleType.Stretch => scaler.Stretch,
                    _ => null
                };

                StartCoroutine(MoveAndThenScale(other.transform, controller, scaler, performScale, 0.5f));
            }
        }

        private IEnumerator MoveAndThenScale(Transform player, CharacterController controller, ScalerInteraction scaler,
            Action<float, Action> performScale, float moveDuration)
        {
            yield return MoveToCenterOverTime(player, controller, moveDuration);

            GameManager.Instance.TogglePlayerMovement(false);
            performScale?.Invoke(timeInSeconds, () => { GameManager.Instance.TogglePlayerMovement(true); });
        }

        private IEnumerator MoveToCenterOverTime(Transform player, CharacterController controller, float duration)

        {
            Vector3 startPos = player.position;
            Vector3 targetPos = transform.position;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                controller.enabled = false;
                player.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
                controller.enabled = true;
                elapsed += Time.deltaTime;
                yield return null;
            }

            controller.enabled = false;
            player.position = targetPos;
            controller.enabled = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                hasTriggered = false;
            }
        }
    }
}