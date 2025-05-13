using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    [SerializeField] private float smoothTime;
    [SerializeField] private Transform target;
    private Vector3 _offset;
    private Vector3 _velocity;

    private void Awake()
    {
        _offset = transform.position - target.position;
    }

    private void LateUpdate()
    {
        var targetPosition = target.position + _offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, smoothTime);
    }
}