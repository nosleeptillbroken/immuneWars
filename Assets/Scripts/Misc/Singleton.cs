using UnityEngine;
using System.Collections.Generic;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    private static object _lock = new object();

    protected Singleton() { }

    public static T instance
    {
        get
        {
            if (_applicationIsQuitting)
            {
                Debug.LogWarning("'" + typeof(T) + "' singleton already destroyed on quit. Returning null.");
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        Debug.LogError("More than one '" + typeof(T) + "' singleton created.");
                        return _instance;
                    }

                    if (_instance == null)
                    {
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = "(Singleton) " + typeof(T).ToString();

                        DontDestroyOnLoad(singleton);

                        Debug.Log("'" + typeof(T) + "' singleton is needed, so " + singleton +  " was created with DontDestroyOnLoad.");
                    }
                    else
                    {
                        Debug.Log("Using singleton " +
                            _instance.gameObject.name);
                    }

                }

                return _instance;
            }
        }
    }

    private static bool _applicationIsQuitting = false;

    public void OnDestroy()
    {
        _applicationIsQuitting = true;
    }
}
