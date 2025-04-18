using UnityEngine;
using GUIContent = UnityEngine.GUIContent;
using UnityEditor;

public class SoundsHolder : MonoBehaviour
{
    public SoundsList soundData;

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

    public void PlaySound(string soundName)
    {
        var entry = soundData.GetSoundByName(soundName);
        if (entry != null)
        {
            SoundManager.Instance.PlaySound(entry.sound, entry.volume);
        }
    }
}

#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(SoundsHolder))]
public class SoundsHolderEditor : Editor
{
    private SerializedProperty soundDataProp;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Reference to SoundsSO
        soundDataProp = serializedObject.FindProperty("soundData");
        EditorGUILayout.PropertyField(soundDataProp);

        SoundsHolder holder = (SoundsHolder)target;
        SoundsList soundData = holder.soundData;

        if (soundData == null)
        {
            EditorGUILayout.HelpBox("No SoundsList assigned.", MessageType.Warning);
            serializedObject.ApplyModifiedProperties();
            return;
        }

        // SerializedObject for SoundsSO
        SerializedObject soundSO = new SerializedObject(soundData);
        SerializedProperty soundsProp = soundSO.FindProperty("sounds");

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("🎵 SoundsSO Editor", EditorStyles.boldLabel);

        // Display list size with Add/Remove buttons
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Sound Entries", GUILayout.MaxWidth(100));
        if (GUILayout.Button("+", GUILayout.Width(25)))
        {
            soundsProp.InsertArrayElementAtIndex(soundsProp.arraySize);
        }

        if (GUILayout.Button("-", GUILayout.Width(25)) && soundsProp.arraySize > 0)
        {
            soundsProp.DeleteArrayElementAtIndex(soundsProp.arraySize - 1);
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // Display and allow editing of each sound entry
        for (int i = 0; i < soundsProp.arraySize; i++)
        {
            SerializedProperty soundEntry = soundsProp.GetArrayElementAtIndex(i);
            SerializedProperty nameProp = soundEntry.FindPropertyRelative("name");
            SerializedProperty clipProp = soundEntry.FindPropertyRelative("sound");
            SerializedProperty volProp = soundEntry.FindPropertyRelative("volume");

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField($"Sound {i}", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(nameProp);
            EditorGUILayout.PropertyField(clipProp);
            EditorGUILayout.Slider(volProp, -0.001f, 1f, new GUIContent("Volume"));
            EditorGUILayout.EndVertical();
        }

        soundSO.ApplyModifiedProperties();
        serializedObject.ApplyModifiedProperties();
    }
}
#endif