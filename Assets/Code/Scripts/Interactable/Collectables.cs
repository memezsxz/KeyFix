using UnityEngine;

namespace Code.Scripts.Interactable
{
    /// <summary>
    /// Controls behavior for collectible objects.
    /// Spins the object in random directions and handles player collection logic.
    /// </summary>
    public class Collectables : MonoBehaviour
    {
        #region Serialized Fields

        /// <summary>
        /// The audio clip that plays when this collectible is collected.
        /// </summary>
        [SerializeField] private AudioClip _collectSound;

        #endregion

        #region Private Fields

        /// <summary>
        /// Rotation speed in degrees per second for each axis (x, y, z).
        /// Total combined speed is capped at 165.
        /// </summary>
        private Vector3 _rotationSpeed;

        /// <summary>
        /// Flag to ensure collection only happens once.
        /// </summary>
        private bool didTriggerCollect;

        #endregion

        #region Unity Methods

        private void Start()
        {
            // Assign a total rotation speed capped at 165, distributed randomly across axes
            var remaining = 165f;
            var x = Random.Range(0f, remaining);
            remaining -= x;
            var y = Random.Range(0f, remaining);
            var z = remaining - y;

            // Shuffle the axis values to randomize their order
            var values = new[] { x, y, z };
            for (var i = 0; i < 3; i++)
            {
                var j = Random.Range(i, 3);
                (values[i], values[j]) = (values[j], values[i]);
            }

            _rotationSpeed = new Vector3(values[0], values[1], values[2]);
        }

        private void Update()
        {
            // Rotate the collectible smoothly over time
            transform.Rotate(_rotationSpeed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            // Ensure this trigger only activates once and only for the player
            if (didTriggerCollect) return;
            if (!other.CompareTag("Player")) return;

            didTriggerCollect = true;

            // Play collection sound and increment collected item count
            SoundManager.Instance.PlaySound(_collectSound);
            GameManager.Instance.IncrementCollectables();

            // Destroy the collected object
            Destroy(gameObject);
        }

        #endregion
    }
}