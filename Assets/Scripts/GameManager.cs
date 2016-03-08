using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : Singleton<GameManager>
{    
    #region Scene/Level Loading

    /// <summary>
    /// Starting scene. Probably the main menu.
    /// </summary>
    public string startingLevel = "Main Menu";

    /// <summary>
    /// List of levels that can be loaded in to play.
    /// </summary>
    public List<string> levels;
    private int _levelIndex = -1;
    public int levelIndex { get { return _levelIndex; } }


    /// <summary>
    /// Loads a single scene to the game.
    /// </summary>
    /// <param name="scene">Name of the scene.</param>
    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

    /// <summary>
    /// Loads a scene additively to the game.
    /// </summary>
    /// <param name="scene">Name of the scene.</param>
    public void LoadSceneAdditive(string scene)
    {
        SceneManager.LoadScene(scene, LoadSceneMode.Additive);
    }

    /// <summary>
    /// Reloads the current scene
    /// </summary>
    public void ReloadScene()
    {
        int bi = SceneManager.GetActiveScene().buildIndex;
        SceneManager.UnloadScene(bi);
        SceneManager.LoadScene(bi, LoadSceneMode.Single);
    }

    /// <summary>
    /// Starts the game by initializing singleton and persisitent gameobjects, then loading the starting level.
    /// </summary>
    public void StartGame()
    {
        _levelIndex = 0;
        StartLevel(levels[levelIndex]);
    }

    /// <summary>
    /// Starts the game by initializing singleton and persisitent gameobjects, then loading the user specified level.
    /// </summary>
    /// <param name="level"></param>
    public void StartLevel(string level)
    {
        LoadPersistents();
        LoadSceneAdditive(level);
    }

    /// <summary>
    /// Loads the next level
    /// </summary>
    public void NextLevel()
    {
        GameObject levelObject = GameObject.Find("Level");
        if(levelObject != null)
        {
            Destroy(levelObject);
        }

        if(levelIndex + 1 >= levels.Count || levelIndex < 0)
        {
            Debug.LogWarning("No next level available, returning to starting scene.");
            LoadSceneAdditive(startingLevel);
        }
        else
        {
            _levelIndex += 1;
            LoadSceneAdditive(levels[levelIndex]);
        }

        Player.instance.SendMessage("OnLevelChanged", null, SendMessageOptions.DontRequireReceiver);
        TowerSelector.instance.SendMessage("OnLevelChanged", null, SendMessageOptions.DontRequireReceiver);
        TowerSpawner.instance.SendMessage("OnLevelChanged", null, SendMessageOptions.DontRequireReceiver);
    }

    /// <summary>
    /// Loads the persistents into the scene.
    /// </summary>
    private void LoadPersistents()
    {
        DontDestroyOnLoad(instance.gameObject);

        LoadScene("Persistents");
    }

    #endregion

    #region Tower Modes

    private bool towersMode = false;
    public bool placeTowers { get { return towersMode; } }
    public bool selectTowers { get { return !towersMode; } }

    //
    public void ToggleTowersMode()
    {
        instance.towersMode = !instance.towersMode;
        TowerSelector.instance.gameObject.SetActive(selectTowers);
        TowerSpawner.instance.gameObject.SetActive(placeTowers);
    }

    //
    public void PlaceTowersMode()
    {
        TowerSelector.instance.gameObject.SetActive(false);
        TowerSpawner.instance.gameObject.SetActive(true);
    }

    //
    public void SelectTowersMode()
    {
        TowerSpawner.instance.gameObject.SetActive(false);
        TowerSelector.instance.gameObject.SetActive(true);
    }

    #endregion

    ////

}
