using System;
using Code.Scripts.Managers;
using UnityEngine;

namespace Code.Scripts.Interactable
{
    public class CollectablesTest : MonoBehaviour
    {
        [SerializeField] AudioClip _collectSound;

        bool didTriggerCollect = false;
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