using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundsList", menuName = "Audio/SoundsList")]
public class SoundsList : ScriptableObject
{
    [System.Serializable]
    public class SoundEntry
    {
        public string name;
        public AudioClip sound;
        [Range(-0.001f, 1f)] public float volume = -0.001f;
    }

    public SoundEntry[] sounds;
    // private Dictionary<string, SoundEntry> soundLookup;

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

//     public void Validate()
//     {
// #if UNITY_EDITOR
//         HashSet<string> seenNames = new HashSet<string>();
//
//         for (int i = 0; i < sounds.Length; i++)
//         {
//             var entry = sounds[i];
//             if (string.IsNullOrWhiteSpace(entry.name))
//             {
//                 Debug.LogError($"[SoundsSO] Sound at index {i} has no name.", this);
//                 EditorGUIUtility.PingObject(this);
//                 Selection.activeObject = this;
//                 return;
//             }
//
//             if (!seenNames.Add(entry.name))
//             {
//                 Debug.LogError($"[SoundsSO] Duplicate sound name '{entry.name}' found at index {i}.", this);
//                 EditorGUIUtility.PingObject(this);
//                 Selection.activeObject = this;
//                 return;
//             }
//         }
// #endif
//     }

    private void OnEnable()
    {
        Dictionary<string, SoundEntry> soundLookup = new Dictionary<string, SoundEntry>();

        HashSet<string> seenNames = new HashSet<string>();

        foreach (var entry in sounds)
        {
            if (string.IsNullOrWhiteSpace(entry.name))
            {
                Debug.LogError($"[SoundsList] Found an unnamed sound entry in '{name}'. Please name all sounds.", this);
                continue;
            }

            if (!seenNames.Add(entry.name))
            {
                Debug.LogError(
                    $"[SoundsList] Duplicate sound name '{entry.name}' found in '{name}'. Names must be unique.", this);
                continue;
            }

            soundLookup[entry.name] = entry;
        }
    }
}