using UnityEngine;

namespace Code.Scripts
{
   public static class SystemsLoader
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Execute()
        {
            var systems = Resources.Load("Systems");

            if (!systems)
                return;
        
            Object.DontDestroyOnLoad(Object.Instantiate(systems));
            Debug.Log("Systems Loaded");
        }
    }

}