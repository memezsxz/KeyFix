using UnityEngine;

/// <summary>
/// A base class that allows any MonoBehaviour to have a static reference to its instance.
/// Does not enforce persistence or uniqueness across scenes.
/// </summary>
/// <typeparam name="T">The MonoBehaviour type to instantiate as a static instance.</typeparam>
public abstract class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour
{
    /// <summary>
    /// Static reference to the current instance of type T.
    /// </summary>
    public static T Instance { get; private set; }

    /// <summary>
    /// Assigns the instance to this component during Awake.
    /// </summary>
    protected virtual void Awake()
    {
        Instance = this as T;
    }

    /// <summary>
    /// Clears the instance and destroys the object when the application quits.
    /// </summary>
    protected virtual void OnApplicationQuit()
    {
        Instance = null;
        Destroy(gameObject);
    }
}

/// <summary>
/// A singleton base class that ensures only one instance exists at runtime.
/// Destroys duplicates automatically if they are created.
/// </summary>
/// <typeparam name="T">The MonoBehaviour type to enforce singleton behavior on.</typeparam>
public abstract class Singleton<T> : StaticInstance<T> where T : MonoBehaviour
{
    /// <summary>
    /// Ensures that only one instance exists. Destroys this object if another instance is already assigned.
    /// </summary>
    protected override void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        base.Awake();
    }
}

/// <summary>
/// A persistent singleton that survives scene loads using DontDestroyOnLoad.
/// </summary>
/// <typeparam name="T">The MonoBehaviour type to persist between scenes.</typeparam>
public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
{
    /// <summary>
    /// Marks this instance to not be destroyed when loading new scenes.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}
