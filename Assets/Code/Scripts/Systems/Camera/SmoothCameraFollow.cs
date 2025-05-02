using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    private Vector3 _offset;
    [SerializeField] private float smoothTime;
    [SerializeField] private Transform target;
    private Vector3 _velocity;

    private void Awake()
    {
        _offset = transform.position - target.position;
    }

    private void LateUpdate()
    {
        Vector3 targetPosition = target.position + _offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, smoothTime);
    }
}