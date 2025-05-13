using UnityEngine;

public static class SystemsLoader
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Execute()
    {
        var systems = Resources.Load("Managers");

        if (!systems)
            return;

        Object.DontDestroyOnLoad(Object.Instantiate(systems));
        // Debug.Log("Systems Loaded");
    }
}