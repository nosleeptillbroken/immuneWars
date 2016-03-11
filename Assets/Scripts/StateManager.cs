using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class StateManager : Singleton<StateManager>
{

    public enum GameState { None, MainMenu, Overworld, InGame };

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

    [SerializeField] private List<GameObject> statePersistents = new List<GameObject>();

    //

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        SetState(GameState.MainMenu);
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
        UnloadSubState();
        _previousSubState = _currentSubState;
        _currentSubState = newSubState;
        LoadSubState(newSubState);
    }

    // Load a SubState
    void LoadSubState(string newSubState)
    {
        SceneManager.LoadScene(newSubState);
        foreach (Transform trans in FindObjectsOfType<Transform>())
        {
            trans.gameObject.SendMessage("OnLoadSubState", null, SendMessageOptions.DontRequireReceiver);
        }
    }
    
    void UnloadSubState()
    {
        foreach (Transform trans in FindObjectsOfType<Transform>())
        {
            trans.gameObject.SendMessage("OnUnloadSubState", null, SendMessageOptions.DontRequireReceiver);
        }
        SceneManager.UnloadScene(SceneManager.GetActiveScene().buildIndex);
    }

    #endregion

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
            trans.gameObject.SendMessage("OnLoadState", null, SendMessageOptions.DontRequireReceiver);
        }

        // If a SubState was specified, load the SubState
        if (newSubState != null)
        {
            LoadSubState(newSubState);
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

}
