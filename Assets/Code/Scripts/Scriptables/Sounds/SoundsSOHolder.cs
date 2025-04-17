using UnityEngine;
using GUIContent = UnityEngine.GUIContent;
using UnityEditor;

public class SoundsSOHolder : MonoBehaviour
{
    public SoundsSO soundData ;

    public AudioClip GetRandomSound(int index)
    {
        AudioClip[] audioClips = soundData.sounds[index].sounds;

        Random.Range(0, audioClips.Length);
        return audioClips[Random.Range(0, audioClips.Length)];
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(SoundsSOHolder))]
public class SoundsSOHolderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SoundsSOHolder holder = (SoundsSOHolder)target;

        if (holder.soundData == null)
        {
            EditorGUILayout.HelpBox("No SoundsSO assigned.", MessageType.Warning);
            return;
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Preview of SoundsSO", EditorStyles.boldLabel);

        SerializedObject so = new SerializedObject(holder.soundData);
        SerializedProperty soundsProp = so.FindProperty("sounds");

        for (int i = 0; i < soundsProp.arraySize; i++)
        {
            SerializedProperty element = soundsProp.GetArrayElementAtIndex(i);
            EditorGUILayout.PropertyField(element, new GUIContent($"Sound {i}"), true);
        }

        so.ApplyModifiedProperties();
    }
}
#endif
