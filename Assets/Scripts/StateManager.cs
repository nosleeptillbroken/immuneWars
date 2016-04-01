using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class StateManager : MonoSingleton<StateManager>
{

    public enum GameState { None, MainMenu, Overworld, InGame };

    //

    public GameState initialState = GameState.None;

    //

    private GameState _currentState = GameState.None;
    private GameState _previousState = GameState.None;

    public GameState currentState { get { return _currentState; } }
    public GameState previousState { get { return _previousState; } }

    //

    private string _currentSubState = null;
    private string _previousSubState = null;

    public string currentSubState { get { return _currentSubState; } }
    public string previousSubState { get { return _previousSubState; } }

    //

    [SerializeField] private Dictionary<string, string> _persistentStrings = new Dictionary<string, string>(127);
    [SerializeField] private Dictionary<string, int> _persistentInts = new Dictionary<string, int>(127);
    [SerializeField] private Dictionary<string, bool> _persistentBools = new Dictionary<string, bool>(127);
    [SerializeField] private Dictionary<string, float> _persistentFloats = new Dictionary<string, float>(127);

    //

    [SerializeField] private List<GameObject> statePersistents = new List<GameObject>();

    //

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        if (initialState != GameState.None)
        {
            SetState(initialState);
        }
    }

    //

    #region Meta States

    /// <summary>
    /// Sets the current GameState, unloading the previous GameState and loading the new GameState in the process.
    /// </summary>
    /// <param name="newState">The new GameState to load.</param>
    public void SetState(GameState newState, string newSubState = null)
    {
        // Unload the old StateScene
        UnloadState();

        // Set the previous state
        _previousState = _currentState;

        // Set the current state
        _currentState = newState;

        // Load the new StateScene
        if (newState != GameState.None)
        {
            StartCoroutine("LoadState", new object[] { newState, newSubState });
        }

    }

    #endregion

    //

    #region Sub States
    
    /// <summary>
    /// Sets the current SubState, unloading the previous SubState and loading the current SubState in the process.
    /// </summary>
    /// <param name="newSubState"></param>
    public void SetSubState(string newSubState)
    {
        if (_currentSubState != null)
        {
            UnloadSubState();
        }

        _previousSubState = _currentSubState;
        _currentSubState = newSubState;

        if (_currentSubState != null)
        {
            StartCoroutine("LoadSubState", newSubState);
        }
    }

    // Load a SubState
    IEnumerator LoadSubState(object newSubState)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync((string)newSubState);
        yield return async;

        foreach (Transform trans in FindObjectsOfType<Transform>())
        {
            trans.SendMessage("OnLoadSubState", null, SendMessageOptions.DontRequireReceiver);
        }
    }
    
    void UnloadSubState()
    {
        foreach (Transform trans in FindObjectsOfType<Transform>())
        {
            trans.SendMessage("OnUnloadSubState", null, SendMessageOptions.DontRequireReceiver);
        }
        SceneManager.UnloadScene(SceneManager.GetActiveScene().buildIndex);
    }

    #endregion

    //

    #region State Loading

    IEnumerator LoadState(object[] args)
    {
        GameState newState = (GameState)args[0];
        string newSubState = (string)args[1];

        Debug.Log("Loading " + newState.ToString());

        AsyncOperation async = SceneManager.LoadSceneAsync(newState.ToString());
        yield return async;

        // Make the new StateScene objects persistent
        // Call OnLoadState() on all objects in the persistent state
        foreach (Transform trans in FindObjectsOfType<Transform>())
        {
            if (trans.parent == null && trans.gameObject != this.gameObject)
            {
                DontDestroyOnLoad(trans.gameObject);
                statePersistents.Add(trans.gameObject);
            }
            trans.SendMessage("OnLoadState", null, SendMessageOptions.DontRequireReceiver);
        }

        // If a SubState was specified, load the SubState
        if (newSubState != null)
        {
            SetSubState(newSubState);
        }

    }

    void UnloadState()
    {
        Debug.Log("Unloading " + currentState.ToString());

        // Clear the persistent state data from the dictionary

        if (currentSubState != null)
        {
            // Unload the currently loaded SubState
            UnloadSubState();
        }

        // If scene is loaded, call OnUnloadState() on all of its GameObjects and unload it 
        // Destroy the current StateScene Persistent Objects
        foreach (Transform trans in FindObjectsOfType<Transform>())
        {
            trans.SendMessage("OnUnloadState", null, SendMessageOptions.DontRequireReceiver);
            
        }
        foreach(GameObject obj in statePersistents)
        {
            if (obj != this.gameObject)
            {
                //Debug.Log("StateManager destroyed " + trans.name);
                Destroy(obj);
            }
        }

        statePersistents.Clear();
    }

    #endregion

    //

    #region Persistent Data

    /// <summary>
    /// Sets a string flag that persists between state changes.
    /// </summary>
    /// <param name="key">The access key</param>
    /// <param name="val">The string value</param>
    public void SetString(string key, string val)
    {
        _persistentStrings[key] = val;
    }

    /// <summary>
    /// Gets a persistent string flag.
    /// </summary>
    /// <param name="key">The access key</param>
    /// <param name="defaultVal">The default value if the key-value pair does not exist.</param>
    /// <returns>The string value</returns>
    public string GetString(string key, string defaultVal = null)
    {
        return HasString(key) ? _persistentStrings[key] : defaultVal;
    }

    /// <summary>
    /// Returns whether or not there is a string value with this key.
    /// </summary>
    /// <param name="key">The access key.</param>
    /// <returns></returns>
    public bool HasString(string key)
    {
        return _persistentStrings.ContainsKey(key);
    }

    /// <summary>
    /// Sets a integer flag that persists between state changes.
    /// </summary>
    /// <param name="key">The access key</param>
    /// <param name="val">The integer value</param>
    public void SetInt(string key, int val)
    {
        _persistentInts[key] = val;
    }

    /// <summary>
    /// Gets a persistent integer flag.
    /// </summary>
    /// <param name="key">The access key</param>
    /// <param name="defaultVal">The default value if the key-value pair does not exist.</param>
    /// <returns>The integer value</returns>
    public int GetInt(string key, int defaultVal = 0)
    {
        return HasInt(key) ? _persistentInts[key] : defaultVal;
    }

    /// <summary>
    /// Returns whether or not there is a integer value with this key.
    /// </summary>
    /// <param name="key">The access key.</param>
    /// <returns></returns>
    public bool HasInt(string key)
    {
        return _persistentInts.ContainsKey(key);
    }

    /// <summary>
    /// Sets a boolean flag that persists between state changes.
    /// </summary>
    /// <param name="key">The access key</param>
    /// <param name="val">The boolean value</param>
    public void SetBool(string key, bool val)
    {
        _persistentBools[key] = val;
    }

    /// <summary>
    /// Gets a persistent boolean flag.
    /// </summary>
    /// <param name="key">The access key</param>
    /// <param name="defaultVal">The default value if the key-value pair does not exist.</param>
    /// <returns>The boolean value</returns>
    public bool GetBool(string key, bool defaultVal = false)
    {
        return HasBool(key) ? _persistentBools[key] : defaultVal;
    }

    /// <summary>
    /// Returns whether or not there is a bool value with this key.
    /// </summary>
    /// <param name="key">The access key.</param>
    /// <returns></returns>
    public bool HasBool(string key)
    {
        return _persistentBools.ContainsKey(key);
    }

    /// <summary>
    /// Sets a float flag that persists between state changes.
    /// </summary>
    /// <param name="key">The access key</param>
    /// <param name="val">The float value</param>
    public void SetFloat(string key, float val)
    {
        _persistentFloats[key] = val;
    }

    /// <summary>
    /// Gets a persistent float flag.
    /// </summary>
    /// <param name="key">The access key</param>
    /// <param name="defaultVal">The default value if the key-value pair does not exist.</param>
    /// <returns>The float value</returns>
    public float GetFloat(string key, float defaultVal = 0.0f)
    {
        return HasFloat(key) ? _persistentFloats[key] : defaultVal;
    }

    /// <summary>
    /// Returns whether or not there is a float value with this key.
    /// </summary>
    /// <param name="key">The access key.</param>
    /// <returns></returns>
    public bool HasFloat(string key)
    {
        return _persistentFloats.ContainsKey(key);
    }

    ///// <summary>
    ///// Sets a persistent object. This operation is potentially unsafe.
    ///// </summary>
    ///// <param name="key">The access key.</param>
    ///// <param name="val">The object value.</param>
    //public void SetObject(string key, object val)
    //{
    //    _persistentObjects[key] = val;
    //}

    ///// <summary>
    ///// Get a persistent object. This operation is potentially unsafe.
    ///// </summary>
    ///// <param name="key">The access key.</param>
    ///// <returns>The object value.</returns>
    //public object GetObject(string key)
    //{
    //    return _persistentObjects[key];
    //}

    ///// <summary>
    ///// Sets a persistent object array. This operation is potentially unsafe.
    ///// </summary>
    ///// <param name="key">The access key.</param>
    ///// <param name="val">The array value.</param>
    //public void SetObjectArray(string key, object[] val)
    //{
    //    SetValHelper("arr", key, val);
    //}

    ///// <summary>
    ///// Get a persistent object array. This operation is potentially unsafe.
    ///// </summary>
    ///// <param name="key">The access key.</param>
    ///// <returns>The array value.</returns>
    //public object GetObjectArray(string key)
    //{
    //    return GetValHelper("arr", key);
    //}

    /// <summary>
    /// Saves persistent data to a file.
    /// </summary>
    /// <param name="file">The path to the file. Creates the file if it doesn't exist.</param>
    /// <returns>Whether or not saving was successful.</returns>
    public bool SaveDataToFile(string file)
    {
        System.IO.BinaryWriter writer = new System.IO.BinaryWriter(System.IO.File.Open(file, System.IO.FileMode.OpenOrCreate));

        writer.Write(_persistentStrings.Count);
        foreach(KeyValuePair<string, string> entry in _persistentStrings)
        {
            writer.Write(entry.Key);
            writer.Write(entry.Value);
        }

        writer.Write(_persistentInts.Count);
        foreach (KeyValuePair<string, int> entry in _persistentInts)
        {
            writer.Write(entry.Key);
            writer.Write(entry.Value);
        }

        writer.Write(_persistentBools.Count);
        foreach (KeyValuePair<string, bool> entry in _persistentBools)
        {
            writer.Write(entry.Key);
            writer.Write(entry.Value);
        }

        writer.Write(_persistentFloats.Count);
        foreach (KeyValuePair<string, float> entry in _persistentFloats)
        {
            writer.Write(entry.Key);
            writer.Write(entry.Value);
        }

        writer.Close();

        return true;
    }

    /// <summary>
    /// Loads persistent data from a file.
    /// </summary>
    /// <param name="file">The path to the file.</param>
    /// <param name="additive">Whether or not the file exists AND loading was successful.</param>
    public bool LoadDataFromFile(string file, bool additive = false)
    {
        System.IO.FileInfo readFile = new System.IO.FileInfo(file);

        System.IO.BinaryReader reader = new System.IO.BinaryReader(System.IO.File.Open(file, System.IO.FileMode.Open));

        if (readFile.Length <= 0)
        {
            Debug.LogWarning("Data file '" + file + "' is empty; No action taken.");
            return false;
        }
        else
        {
            try
            {
                if (additive == false)
                {
                    _persistentStrings.Clear();
                    _persistentInts.Clear();
                    _persistentBools.Clear();
                    _persistentFloats.Clear();
                }

                int numPairs = 0;

                // strings
                numPairs = reader.ReadInt32();
                for (int i = 0; i < numPairs; ++i)
                {
                    string tkey = reader.ReadString();
                    string tval = reader.ReadString();
                    _persistentStrings[tkey] = tval;
                    Debug.Log(tkey + ": " + tval);
                }

                // ints
                numPairs = reader.ReadInt32();
                for (int i = 0; i < numPairs; ++i)
                {
                    string tkey = reader.ReadString();
                    int tval = reader.ReadInt32();
                    _persistentInts[tkey] = tval;
                    Debug.Log(tkey + ": " + tval);
                }

                // bools
                numPairs = reader.ReadInt32();
                for (int i = 0; i < numPairs; ++i)
                {
                    string tkey = reader.ReadString();
                    bool tval = reader.ReadBoolean();
                    _persistentBools[tkey] = tval;
                    Debug.Log(tkey + ": " + tval);
                }

                // floats
                numPairs = reader.ReadInt32();
                for (int i = 0; i < numPairs; ++i)
                {
                    string tkey = reader.ReadString();
                    float tval = reader.ReadSingle();
                    _persistentFloats[tkey] = tval;
                    Debug.Log(tkey + ": " + tval);
                }
            }
            catch(System.IO.IOException e)
            {
                Debug.LogWarning(e.Message);
                reader.Close();
                return false;
            }
        }

        reader.Close();
        return true;
    }

    #endregion
}
