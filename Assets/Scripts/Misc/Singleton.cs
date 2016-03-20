using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A MonoBehaviour Singleton; this allows only one GameObject with this script to exist and persist. Use Singleton<T> for the general-purpose implementation.
/// </summary>
/// <typeparam name="T"></typeparam>
public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance = null;

    /// <summary>
    /// The singleton instance. Can be null.
    /// </summary>
    public static T instance { get { return _instance; } }

    void Awake()
    {
        if (!HasInstance())
        {
            _instance = this as T;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Returns whether or not this singleton has an instance, without initializing it.
    /// </summary>
    /// <returns></returns>
    public static bool HasInstance()
    {
        return (_instance != null);
    }
}

/// <summary>
/// General-purpose singleton. This does not reference a MonoBehaviour-derived object; use MonoSingleton<T> for that implementation.
/// </summary>
/// <typeparam name="T">The class being turned into a singleton.</typeparam>
public class Singleton<T> where T : class, new()
{
    private static T _instance = null;

    /// <summary>
    /// The singleton instance. Creates a new instance if one does not exist.
    /// </summary>
    public static T instance
    {
        get
        {
            if (!HasInstance())
            {
                _instance = new T();
            }

            return _instance;
        }
    }

    /// <summary>
    /// Returns whether or not this singleton has an instance, without initializing it.
    /// </summary>
    /// <returns></returns>
    public static bool HasInstance()
    {
        return (_instance != null);
    }
}