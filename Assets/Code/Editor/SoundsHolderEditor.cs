using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(SoundsHolder))]
public class SoundsHolderEditor : Editor
{
    private SerializedProperty soundDataProp;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw the reference to the SoundsList ScriptableObject
        soundDataProp = serializedObject.FindProperty("soundData");
        EditorGUILayout.PropertyField(soundDataProp);

        var holder = (SoundsHolder)target;
        var soundData = holder.soundData;

        if (soundData == null)
        {
            EditorGUILayout.HelpBox("No SoundsList assigned.", MessageType.Warning);
            serializedObject.ApplyModifiedProperties();
            return;
        }

        // Create a SerializedObject for the referenced SoundsList asset
        var soundSO = new SerializedObject(soundData);
        var soundsProp = soundSO.FindProperty("sounds");

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("🎵 Sounds Editor", EditorStyles.boldLabel);

        // Display list header and buttons to add/remove sounds
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Sound Entries", GUILayout.MaxWidth(100));

        if (GUILayout.Button("+", GUILayout.Width(25)))
            soundsProp.InsertArrayElementAtIndex(soundsProp.arraySize);

        if (GUILayout.Button("-", GUILayout.Width(25)) && soundsProp.arraySize > 0)
            soundsProp.DeleteArrayElementAtIndex(soundsProp.arraySize - 1);

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        // Loop through sound entries and draw editable fields
        for (int i = 0; i < soundsProp.arraySize; i++)
        {
            var soundEntry = soundsProp.GetArrayElementAtIndex(i);
            var nameProp = soundEntry.FindPropertyRelative("name");
            var clipProp = soundEntry.FindPropertyRelative("sound");
            var volProp = soundEntry.FindPropertyRelative("volume");

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
