using System;
using Code.Scripts.Managers;
using UnityEngine;

namespace Code.Scripts.Interactable
{
    public class CollectablesTest : MonoBehaviour
    {
        [SerializeField] AudioClip _collectSound;
        private Vector3 _rotationSpeed; // degrees per second

        bool didTriggerCollect = false;

        private void Start()
        {
            float remaining = 165f;
            float x = UnityEngine.Random.Range(0f, remaining);
            remaining -= x;
            float y = UnityEngine.Random.Range(0f, remaining);
            float z = remaining - y;

            // Shuffle the values randomly
            float[] values = new[] { x, y, z };
            for (int i = 0; i < 3; i++)
            {
                int j = UnityEngine.Random.Range(i, 3);
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