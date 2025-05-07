using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Scripts.Obstacles
{
    public class WindBlower : MonoBehaviour
    {
        [SerializeField] private Vector3 windDirection = Vector3.forward;
        [SerializeField] private float baseWindStrength = 10f;
        [SerializeField] private float detectionRange = 10f;
        [SerializeField] private float minimumWindResistance = 0.3f; // How much resistance even if huge

        private PlayerMovement playerMovement;
        private Transform playerTransform;

        private void Start()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerMovement = player.GetComponent<PlayerMovement>();
                playerTransform = player.transform;
            }
        }

        private void Update()
        {
            if (playerMovement == null || playerTransform == null) return;
            Vector3 center = transform.position;
            Vector3 halfExtents = transform.localScale * 0.5f;
            Quaternion rotation = transform.rotation;

            RaycastHit hit;
            if (Physics.BoxCast(center, halfExtents, windDirection, out hit, rotation, detectionRange))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    float scaleFactor = playerTransform.localScale.magnitude / Mathf.Sqrt(3f);

                    float effectiveWindStrength = baseWindStrength;

                    if (scaleFactor > 1f)
                    {
                        effectiveWindStrength /= (scaleFactor * 2f);
                        effectiveWindStrength =
                            Mathf.Max(effectiveWindStrength, baseWindStrength * minimumWindResistance);
                    }

                    var inputActions = playerTransform.GetComponent<PlayerInput>().actions;
                    Vector2 moveValue = inputActions.FindAction("Move").ReadValue<Vector2>();

                    if (moveValue != Vector2.zero && scaleFactor > 1f)
                    {
                        Vector3 moveInput = new Vector3(moveValue.x, 0f, moveValue.y).normalized;
                        float againstWind = Vector3.Dot(moveInput, windDirection.normalized);

                        if (againstWind < 0f)
                        {
                            effectiveWindStrength *= 0.5f;
                        }
                    }

                    Vector3 force = windDirection.normalized * effectiveWindStrength;
                    playerMovement.ApplyExternalForce(force);
                }
            }
        }
    }
}