using UnityEngine;

/// <summary>
/// Example component demonstrating how to use the SoundsHolder system
/// and register a debug command to play indexed sounds.
/// </summary>
[RequireComponent(typeof(SoundsHolder))]
public class ImplementationExample : MonoBehaviour
{
    private SoundsHolder _soundsHolder;

    private void Start()
    {
        _soundsHolder = GetComponent<SoundsHolder>();

        // Register a debug console command to trigger sound playback by index
        DebugController.Instance.AddDebugCommand(new DebugCommand(
            "playSound",
            "Plays the sound with the provided index in the SoundsHolder",
            "playSound <int>",
            args =>
            {
                var index = int.Parse(args[0]);
                _soundsHolder.PlaySound(index);
            }));
    }

    private void OnEnable()
    {
        // Play sound by name when enabled
        _soundsHolder.PlaySound("click");
    }

    private void OnDisable()
    {
        // Play sound by index when disabled
        _soundsHolder.PlaySound(0);
    }
}