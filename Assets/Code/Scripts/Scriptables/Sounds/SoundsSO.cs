using System;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu(menuName = "Audio/SoundsSO")]
public class SoundsSO : ScriptableObject
{
    public SoundList[] sounds;
}

[Serializable]
public struct SoundList
{
    [HideInInspector] public string name;
    [Range(0, 1)] public float volume;
    public AudioClip[] sounds;
}


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SoundList))]
public class SoundListDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var soundsProp = property.FindPropertyRelative("sounds");

        if (soundsProp.arraySize > 0 && soundsProp.GetArrayElementAtIndex(0).objectReferenceValue != null)
        {
            label.text = ((AudioClip)soundsProp.GetArrayElementAtIndex(0).objectReferenceValue).name;
        }

        EditorGUI.PropertyField(position, property, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Automatically calculates the correct height when expanded
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}
#endif

