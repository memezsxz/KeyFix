using System;
using Cinemachine;
using UnityEngine;

/// <summary>
/// Overrides the camera's aiming behavior by locking the orientation
/// to the Y-axis rotation of a target transform (e.g., the player).
/// Useful for maintaining consistent horizontal aim, such as in lock-on modes.
/// </summary>
public class CinemachinePOVExtension : CinemachineExtension
{
    /// <summary>
    /// The target Transform (typically the player) whose Y rotation will be used to control the camera aim.
    /// </summary>
    [SerializeField] private Transform target;

    /// <summary>
    /// Called by Cinemachine at different stages of the camera pipeline.
    /// In this extension, we override the Aim stage to orient the camera based on the target's Y-axis rotation.
    /// 
    /// Pipeline stages (for reference):
    /// - Aim: Rotate the camera to point at the target.
    /// - Body: Position the camera in world space.
    /// - Noise: Apply camera shake or other procedural effects.
    /// - Finalize: Internal adjustment stage before Noise (not overrideable).
    /// </summary>
    /// <param name="vcam">The virtual camera being processed.</param>
    /// <param name="stage">The pipeline stage currently being executed.</param>
    /// <param name="state">The mutable state of the camera for this frame.</param>
    /// <param name="deltaTime">Time since the last frame.</param>
    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage,
        ref CameraState state, float deltaTime)
    {
        // If no target is assigned, do nothing
        if (target == null) return;

        // Override the Aim stage to set horizontal rotation based on the target
        if (stage == CinemachineCore.Stage.Aim)
        {
            // Create a rotation with only the Y-axis from the target
            var playerRotation = Quaternion.Euler(0f, target.eulerAngles.y, 0f);

            // Apply the rotation to the camera's raw orientation
            state.RawOrientation = playerRotation;
        }
    }
}
