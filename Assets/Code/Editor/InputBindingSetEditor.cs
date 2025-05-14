using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom editor for <see cref="InputBindingSet"/> that allows you to apply bindings
/// to all <see cref="PlayerBindingManage"/> components in the scene from the asset inspector.
/// </summary>
[CustomEditor(typeof(InputBindingSet))]
public class InputBindingSetEditor : Editor
{
    /// <summary>
    /// Draws the default inspector plus a button for applying bindings to all players in the scene.
    /// </summary>
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);

        if (GUILayout.Button("Apply Bindings In Scene"))
            ApplyBindingsInScene();
    }

    /// <summary>
    /// Applies the current <see cref="InputBindingSet"/> to all active <see cref="PlayerBindingManage"/> components in the scene.
    /// </summary>
    private void ApplyBindingsInScene()
    {
        var bindingSetAsset = (InputBindingSet)target;

        var managers = FindObjectsOfType<PlayerBindingManage>();
        int updated = 0;

        foreach (var manager in managers)
        {
            if (manager != null)
            {
                manager.ApplyFromScriptableObject(bindingSetAsset);
                updated++;
            }
        }

        Debug.Log($"Applied bindings to {updated} manager(s) in scene.");
    }
}