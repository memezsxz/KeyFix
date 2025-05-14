using UnityEngine;
using UnityEngine.InputSystem;


    public class WindBlower : MonoBehaviour
    {
        [SerializeField] private Vector3 windDirection = Vector3.forward;
        [SerializeField] private float baseWindStrength = 10f;
        [SerializeField] private float detectionRange = 10f;
        [SerializeField] private float minimumWindResistance = 0.3f; // How much resistance even if huge
        [SerializeField] private AudioClip sound;
        private PlayerMovement playerMovement;
        private Transform playerTransform;

        private void Start()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerMovement = player.GetComponent<PlayerMovement>();
                playerTransform = player.transform;
            }
        }

        private void Update()
        {
            if (playerMovement == null || playerTransform == null) return;
            var center = transform.position;
            var halfExtents = transform.localScale * 0.5f;
            var rotation = transform.rotation;

            RaycastHit hit;
            if (Physics.BoxCast(center, halfExtents, windDirection, out hit, rotation, detectionRange))
                if (hit.collider.CompareTag("Player"))
                {
                    var scaleFactor = playerTransform.localScale.magnitude / Mathf.Sqrt(3f);

                    var effectiveWindStrength = baseWindStrength;

                    if (scaleFactor > 1f)
                    {
                        effectiveWindStrength /= scaleFactor * 2f;
                        effectiveWindStrength =
                            Mathf.Max(effectiveWindStrength, baseWindStrength * minimumWindResistance);
                    }

                    var inputActions = playerTransform.GetComponent<PlayerInput>().actions;
                    var moveValue = inputActions.FindAction("Move").ReadValue<Vector2>();

                    if (moveValue != Vector2.zero && scaleFactor > 1f)
                    {
                        var moveInput = new Vector3(moveValue.x, 0f, moveValue.y).normalized;
                        var againstWind = Vector3.Dot(moveInput, windDirection.normalized);

                        if (againstWind < 0f) effectiveWindStrength *= 0.5f;
                    }

                    var force = windDirection.normalized * effectiveWindStrength;
                    playerMovement.ApplyExternalForce(force);
                    SoundManager.Instance.PlaySound(sound);
                }
        }
    }
