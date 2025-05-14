using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A ScriptableObject that holds a list of named sound entries.
/// Used to centralize and manage audio clips and their volume settings.
/// </summary>
[CreateAssetMenu(fileName = "SoundsList", menuName = "Audio/SoundsList")]
public class SoundsList : ScriptableObject
{
    /// <summary>
    /// Array of all defined sound entries.
    /// </summary>
    public SoundEntry[] sounds;

    private void OnEnable()
    {
        var soundLookup = new Dictionary<string, SoundEntry>();
        var seenNames = new HashSet<string>();

        // Validate each sound entry when the ScriptableObject is loaded
        foreach (var entry in sounds)
        {
            if (string.IsNullOrWhiteSpace(entry.name))
            {
                Debug.LogError($"[SoundsList] Found an unnamed sound entry in '{name}'. Please name all sounds.", this);
                continue;
            }

            if (!seenNames.Add(entry.name))
            {
                Debug.LogError($"[SoundsList] Duplicate sound name '{entry.name}' found in '{name}'. Names must be unique.", this);
                continue;
            }

            soundLookup[entry.name] = entry;
        }
    }

    /// <summary>
    /// Returns a sound entry by name, or logs a warning if not found.
    /// </summary>
    /// <param name="soundName">The name of the sound to retrieve.</param>
    /// <returns>The matching <see cref="SoundEntry"/>, or null if not found.</returns>
    public SoundEntry GetSoundByName(string soundName)
    {
        foreach (var entry in sounds)
        {
            if (entry.name == soundName)
                return entry;
        }

        Debug.LogWarning($"Sound '{soundName}' not found.");
        return null;
    }

    /// <summary>
    /// Represents a single sound definition including its clip and volume.
    /// </summary>
    [Serializable]
    public class SoundEntry
    {
        /// <summary>
        /// Unique name for the sound (used for lookup).
        /// </summary>
        public string name;

        /// <summary>
        /// Audio clip to play for this sound.
        /// </summary>
        public AudioClip sound;

        /// <summary>
        /// Volume at which to play this sound.
        /// </summary>
        [Range(-0.001f, 1f)]
        public float volume = -0.001f;
    }
}
