using System;
using Cinemachine;
using UnityEngine;

public class CinemachinePOVExtension : CinemachineExtension
{
    [SerializeField] private Transform target;

    private void Start()
    {
    }

    /// <summary>
    ///     Cina machine operates in stages
    ///     Aim -> orien the camera to point at the target
    ///     Body -> position the camera in space
    ///     Noise -> apply noise
    ///     there is also Finalize, called before noise, but it is not part of the pipline
    /// </summary>
    /// <param name="vcam"></param>
    /// <param name="stage"></param>
    /// <param name="state"></param>
    /// <param name="deltaTime"></param>
    /// <exception cref="NotImplementedException"></exception>
    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage,
        ref CameraState state, float deltaTime)
    {
        if (target == null) return;

        if (stage == CinemachineCore.Stage.Aim)
        {
            // Set camera orientation to match the player's Y rotation
            var playerRotation = Quaternion.Euler(0f, target.eulerAngles.y, 0f);
            state.RawOrientation = playerRotation;
        }
    }
}