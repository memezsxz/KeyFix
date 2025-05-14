using System;
using System.Collections;
using UnityEngine;

    public class Scaler : MonoBehaviour
    {
        public enum ScaleType
        {
            Shrink,
            Stretch
        }

        [SerializeField] private ScaleType scaleType;
        [SerializeField] private AudioClip shrinkerSound;
        [SerializeField] private AudioClip stretcherSound;
        
        private AudioClip sound;
        
        private bool hasTriggered;
        private readonly float timeInSeconds = 2;

        private void Start()
        {
            sound = scaleType == ScaleType.Shrink ? shrinkerSound : stretcherSound;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (hasTriggered || !other.CompareTag("Player")) return;
            hasTriggered = true;

            var controller = other.GetComponent<CharacterController>();
            var scaler = other.GetComponent<ScalerInteraction>();

            if (controller != null && scaler != null)
            {
                var alreadyAtTarget = scaleType switch
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
                SoundManager.Instance.PlaySound(sound);
                StartCoroutine(MoveAndThenScale(other.transform, controller, scaler, performScale, 0.5f));
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player")) hasTriggered = false;
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
            var startPos = player.position;
            var targetPos = transform.position;
            var elapsed = 0f;

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
    }
