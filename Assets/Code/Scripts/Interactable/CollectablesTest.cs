using UnityEngine;

namespace Code.Scripts.Interactable
{
    public class CollectablesTest : MonoBehaviour
    {
        [SerializeField] private AudioClip _collectSound;
        private Vector3 _rotationSpeed; // degrees per second

        private bool didTriggerCollect;

        private void Start()
        {
            var remaining = 165f;
            var x = Random.Range(0f, remaining);
            remaining -= x;
            var y = Random.Range(0f, remaining);
            var z = remaining - y;

            // Shuffle the values randomly
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
            transform.Rotate(_rotationSpeed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (didTriggerCollect) return;
            // print("here");
            if (!other.CompareTag("Player")) return;
            didTriggerCollect = true;
            SoundManager.Instance.PlaySound(_collectSound);
            GameManager.Instance.IncrementCollectables();
            Destroy(gameObject);
        }
    }
}