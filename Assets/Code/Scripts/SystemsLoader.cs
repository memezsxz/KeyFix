using UnityEngine;

/// <summary>
/// Automatically loads a prefab named "Managers" from the Resources folder
/// and instantiates it before the first scene is loaded. Ensures it's not destroyed on scene change.
/// </summary>
public static class SystemsLoader
{
    /// <summary>
    /// This method is called by Unity before any scene is loaded.
    /// It loads and instantiates the "Managers" prefab from Resources.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Execute()
    {
        var systems = Resources.Load("Managers");

        if (!systems)
            return;

        // Instantiate and preserve the manager object across scene loads
        Object.DontDestroyOnLoad(Object.Instantiate(systems));

        // Debug.Log("Systems Loaded");
    }
}