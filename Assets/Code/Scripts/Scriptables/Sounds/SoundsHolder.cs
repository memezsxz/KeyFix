using UnityEditor;
using UnityEngine;
using GUIContent = UnityEngine.GUIContent;

/// <summary>
/// Component used to play sounds via an assigned <see cref="SoundsList"/>.
/// </summary>
public class SoundsHolder : MonoBehaviour
{
    /// <summary>
    /// Reference to the sound data asset containing the list of sound entries.
    /// </summary>
    public SoundsList soundData;

    /// <summary>
    /// Plays a sound by index from the assigned sound list.
    /// </summary>
    /// <param name="index">The index of the sound entry.</param>
    public void PlaySound(int index)
    {
        if (index >= 0 && index < soundData.sounds.Length)
        {
            var entry = soundData.sounds[index];
            SoundManager.Instance.PlaySound(entry.sound, entry.volume);
        }
        else
        {
            Debug.LogWarning($"Sound index {index} is out of range.");
        }
    }

    /// <summary>
    /// Plays a sound by its name from the assigned sound list.
    /// </summary>
    /// <param name="soundName">The name of the sound to play.</param>
    public void PlaySound(string soundName)
    {
        var entry = soundData.GetSoundByName(soundName);
        if (entry != null)
            SoundManager.Instance.PlaySound(entry.sound, entry.volume);
    }
}