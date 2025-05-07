using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InputBindingSet))]
public class InputBindingSetEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);
        if (GUILayout.Button("Apply Bindings In Scene"))
        {
            ApplyBindingsInScene();
        }
    }

    private void ApplyBindingsInScene()
    {
        var bindingSetAsset = (InputBindingSet)target;

        var managers = GameObject.FindObjectsOfType<PlayerBindingManage>();
        int updated = 0;

        foreach (var manager in managers)
        {
            if (manager != null)
            {
                manager.ApplyFromScriptableObject(bindingSetAsset); // <- add this method
                updated++;
            }
        }

        Debug.Log($"Applied bindings to {updated} manager(s) in scene.");
    }

}