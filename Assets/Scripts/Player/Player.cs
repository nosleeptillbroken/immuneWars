// Player.cs
// Defines behaviour for all player-related tasks.

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Player : MonoSingleton<Player>
{

    [Header("Health")] ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Set the default player max health to 50 and create a variable for current health
    /// This will be used to display the health bar for player and provide a loss condition
    /// </summary>
    public int maxHealth = 50;

    [SerializeField] private int _currentHealth = 0;
    public int currentHealth { get { return _currentHealth; } }

    [Header("Gold")] ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // maximum amount of gold the player can have
    public int maxGold = int.MaxValue;

    // amount of gold the player starts with
    public int startingGold = 30;

    // current gold the player has
    private int _currentGold = 0;
    public int currentGold { get { return (infiniteGold) ? maxGold : _currentGold; } }

    /// <summary>
    /// If true, player can buy and upgrade for free, and does not collect gold.
    /// </summary>
    public bool infiniteGold = false;

    [Header("Interface")] ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    /// <summary>
    /// The GameObject that contains the game over UI dialog.
    /// </summary>
    public GameObject gameOver;

    /// <summary>
    /// The GameObject that contains the game win UI dialog.
    /// </summary>
    public GameObject gameWin;

    /// <summary>
    /// The GameObject that contains the score screen.
    /// </summary>
    public GameObject resultsScreen;

    /// <summary>
    /// The Slider component that displays the player's current health.
    /// </summary>
    public Slider healthBar;

    /// <summary>
    /// The Text component that displays gold on screen.
    /// </summary>
    public Text goldText;

    /// <summary>
    /// The Text component that displays kills on screen.
    /// </summary>
    public Text killsText;

    /// <summary>
    /// The Text component that displays score on screen.
    /// </summary>
    public Text scoreText;

    // One-time flags for win and loss

    private bool winFlag = false;
    private bool loseFlag = false;

    [Header("Score Keeping")]

    /// <summary>
    /// Number of enemies that must be killed to win the level.
    /// </summary> 

    //Number of enemies killed by the player.
    private int totalKills = 0;
    //Number of enemies leaked, added with total kills for win condition.   
    private int totalMisses = 0;    

    private int killScoreMult = 10;
    private int missScoreMult = 10;

    [Header("Level Loading")]
    public string currentLevel;

    #region MonoBehaviour

    // Use this for initialization
    void Start()
    {
        // If they were not linked in editor find the Health Bar and Game Over objects for the scene
        if (!healthBar && GameObject.Find("Health Display")) healthBar = GameObject.Find("Health Display").GetComponent<Slider>();
        if (!goldText && GameObject.Find("Gold Display")) goldText = GameObject.Find("Gold Display").GetComponent<Text>();

        if (!gameOver) gameOver = GameObject.Find("Game Over");
        if (!gameWin) gameWin = GameObject.Find("Game Win");
        if (!resultsScreen) resultsScreen = GameObject.Find("Results Screen");

        if (gameOver) gameOver.SetActive(false);
        if (gameWin) gameWin.SetActive(false);
        if (resultsScreen) resultsScreen.SetActive(false);

        // Get a reference to the tower spawner
        // set health to max at beginning
        if (_currentHealth == 0) _currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.minValue = 0;

    }

    // Update is called once per frame
    void Update()
    {
        // Health bar displayed reflects current health
        if(healthBar) healthBar.value = currentHealth;
        if(scoreText) scoreText.text = "Score: " + (totalKills * killScoreMult).ToString(); //Adjust the player's score killing enemies.
        if(killsText) killsText.text = "Kills: " + totalKills.ToString(); //Adjust the player's total kills.

        // Update text on the gold display
        if (goldText)
        {
            goldText.GetComponentInChildren<Text>().text = (infiniteGold ? "∞" : currentGold.ToString());
        }

        if (Debug.isDebugBuild && Input.GetButtonDown("Debug Win Level"))
        {
            OnWin();
        }
        else if (Debug.isDebugBuild && Input.GetButtonDown("Debug Lose Level"))
        {
            OnLose();
        }

        if(StateManager.HasInstance() && StateManager.instance.currentState == StateManager.GameState.Overworld)
        {
            CheckGlobalStatus();
        }
    }

    #endregion

    /// <summary>
    /// Checks if the player has lost (player has no health) or won (all creeps killed/despawned and player still has health)
    /// </summary>
    public void CheckStatus()
    {
        // If player health is zero
        if (currentHealth <= 0 && loseFlag == false)
        {
            OnLose();
        }
        // If player health is nonzero and total kills and misses is greater than / equal to level creeps
        else if ((totalKills + totalMisses) >= CreepSpawner.GetLevelTotalCreeps() && loseFlag == false && winFlag == false )
        {
            OnWin();
        }
    }

    /// <summary>
    /// Checks if the player has lost (player has no health) or won (all creeps killed/despawned and player still has health)
    /// </summary>
    public void CheckGlobalStatus()
    {
        // If player health is zero
        if (StateManager.instance.GetInt("player_global_health", maxHealth) <= 0 && loseFlag == false)
        {
            OnLose();
        }
        // If player health is nonzero and total kills and misses is greater than / equal to level creeps
        else if (LevelData.completedCount >= LevelData.count && StateManager.instance.GetBool("overworld_complete") == false)
        {
            OnWin();
        }
    }

    #region Messages

    /// <summary>
    /// Function handles what is activated/deactivated on game win.
    /// </summary>
    void OnWin()
    {
        winFlag = true;

        // Enable game win screen
        Debug.Log("Player Won");

        gameWin.SetActive(true);

        if (StateManager.instance.currentState == StateManager.GameState.InGame)
        {
            StateManager.instance.SetBool(StringUtils.KeyFriendlyString(currentLevel + " complete"), true);
        }
        else if (StateManager.instance.currentState == StateManager.GameState.Overworld)
        {
            StateManager.instance.SetBool("overworld_complete", true);
        }
    }

    /// <summary>
    /// Called when the player's health reaches 0
    /// </summary>
    void OnLose()
    {
        loseFlag = true;

        // Display game over menu
        Debug.Log("Player Lost");

        gameOver.SetActive(true);

        if (StateManager.instance.currentState == StateManager.GameState.InGame)
        {

            int levelDifficulty = StateManager.instance.GetInt(StringUtils.KeyFriendlyString(currentLevel + " difficulty"))+1;

            int currentGlobalHealth = StateManager.instance.GetInt("player_global_health");
            int newGlobalHealth = currentGlobalHealth - levelDifficulty;

            Debug.Log("player_global_health = " + currentGlobalHealth + " - " + levelDifficulty + " = " + newGlobalHealth);

            StateManager.instance.SetInt("player_global_health", newGlobalHealth);

            bool isLevelAlreadyCompleted = StateManager.instance.GetBool(StringUtils.KeyFriendlyString(currentLevel + " complete"));
            StateManager.instance.SetBool(StringUtils.KeyFriendlyString(currentLevel + " complete"), isLevelAlreadyCompleted);
        }
        else if (StateManager.instance.currentState == StateManager.GameState.Overworld)
        {
            StateManager.instance.WipeData();
        }

        DisableInteractables();
    }

    /// <summary>
    /// Function adds to score when creep is killed.
    /// </summary>
    void OnKillCreep()
    {
        totalKills += 1;

        CheckStatus();
    }

    /// <summary>
    /// Function to add to missed kills when creep reaches despawn point.
    /// </summary>
    void OnMissCreep()
    {
        totalMisses += 1;

        CheckStatus();
    }

    /// <summary>
    /// Called when the state this GameObject is in is loaded.
    /// </summary>
    void OnLoadState()
    {
        if(StateManager.instance.currentState == StateManager.GameState.Overworld)
        {
            if (!StateManager.instance.HasInt("player_global_health"))
            {
                StateManager.instance.SetInt("player_global_health", maxHealth);
                Debug.Log("player_global_health = " + maxHealth + "(default)");
            }
            else
            {
                int newGlobalHealth = StateManager.instance.GetInt("player_global_health", maxHealth);
                _currentHealth = newGlobalHealth;
                Debug.Log("player_global_health = " + newGlobalHealth);
            }
        }
    }

    /// <summary>
    /// Called when the state this GameObject in is about to be unloaded.
    /// </summary>
    void OnUnloadState()
    {
    }

    /// <summary>
    /// Called when the substate this GameObject is in is loaded.
    /// </summary>
    void OnLoadSubState()
    {
        currentLevel = StateManager.instance.currentSubState;
        ResetState();
    }

    /// <summary>
    /// Called when the substate this GameObject is in is about to be unloaded.
    /// </summary>
    void OnUnloadSubState()
    {
        ResetInteractables();
        ResetState();
    }

    /// <summary>
    /// Called if a new creep spawner is created.
    /// </summary>
    void OnNewCreepSpawner()
    {
    }

    #endregion

    /// <summary>
    /// Add health to the player. If the resulting health does not exceed max health, add health and return true. Else, add up to max health and return false.
    /// </summary>
    /// <param name="amount">The amount to add</param>
    /// <returns>If the gold does not exceed max gold, return true and add gold. Else, add up to max gold and return false.</returns>
    public bool AddHealth(int amount)
    {
        if (currentHealth + amount <= maxHealth)
        {
            _currentHealth += amount;
            return true;
        }
        else
        {
            _currentHealth = maxHealth;
            return false;
        }
    }

    /// <summary>
    /// Remove health from the player. If the player would die from removing health, call the death function.
    /// </summary>
    /// <param name="amount">The damage to recieve</param>
    public void RemoveHealth(int amount)
    {
        _currentHealth -= amount;

        CheckStatus();
    }

    /// <summary>
    /// Add gold to the player. If the resulting gold does not exceed max gold, add gold and return true. Else, add up to max gold and return false.
    /// </summary>
    /// <param name="amount">The amount to add</param>
    /// <returns>If the gold does not exceed max gold, return true and add gold. Else, add up to max gold and return false.</returns>
    public bool AddGold(int amount)
    {
        if (infiniteGold)
        {
            return true;
        }

        if (currentGold + amount <= maxGold)
        {
            _currentGold += amount;
            return true;
        }
        else
        {
            _currentGold = maxGold;
            return false;
        }
    }

    /// <summary>
    /// Remove gold from the player. If the resulting gold is 0 or greater, subtract gold and return true. Else, do nothing and return false.
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    public bool RemoveGold(int amount)
    {
        if (infiniteGold)
        {
            return true;
        }

        if (currentGold - amount >= 0)
        {
            _currentGold -= amount;
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Resets player state to initial values
    /// </summary>
    public void ResetState()
    {
        winFlag = false;
        loseFlag = false;

        _currentHealth = maxHealth;
        _currentGold = startingGold;

        totalKills = 0;
        totalMisses = 0;
    }

    /// <summary>
    /// Transition to the next level
    /// </summary>
    /// <param name="sceneName"></param>
    public void ReturnToOverworld()
    {
        StateManager.instance.SetState(StateManager.GameState.Overworld);
    }

    /// <summary>
    /// Reloads current level, selected from the InGame pause menu.
    /// </summary>
    public void ReloadCurrentLevel()
    {
        StateManager.instance.SetState(StateManager.GameState.InGame, StateManager.instance.currentSubState);
    }

    /// <summary>
    /// Returns to the MainMenu, selected from the InGame pause menu.
    /// </summary>
    public void ReturnToMainMenu()
    {
        System.IO.Directory.CreateDirectory("./Saves/");
        StateManager.instance.SaveDataToFile("./Saves/save.iws");
        StateManager.instance.SetState(StateManager.GameState.MainMenu);
    }

    /// <summary>
    /// Function displays score screen, selected from game win menu.
    /// </summary>
    public void DisplayScoreScreen()
    {
        //Activate score screen.
        resultsScreen.SetActive(true);

        //More values can be added in the future for different creep types.
        int killScore = killScoreMult * totalKills;
        int missScore = missScoreMult * totalMisses;

        //Text for total creeps killed, and associated score.
        Text SSkills = GameObject.Find("KilledAmount").GetComponent<Text>();
        SSkills.text = totalKills + " x " + killScoreMult + " = " + killScore;
        //Text for total creeps killed, and associated deduction.
        Text SSmissed = GameObject.Find("MissedAmount").GetComponent<Text>();
        SSmissed.text = totalMisses + " x (" + missScoreMult + ") = " + missScore;
        //Total Score.
        Text SStotal = GameObject.Find("TotalAmount").GetComponent<Text>();
        SStotal.text = (killScore + missScore).ToString();
    }

    /// <summary>
    /// Resets all player-interactable objects to their initial state
    /// </summary>
    public void ResetInteractables()
    {
        if (Camera.main)
        {
            Camera.main.GetComponent<CameraController>().enabled = true;
        }

        if (healthBar)
        {
            healthBar.gameObject.SetActive(true);
        }

        if (goldText)
        {
            goldText.transform.parent.gameObject.SetActive(true);
        }

        if (TowerManager.HasInstance())
        {
            TowerManager.instance.gameObject.SetActive(true);
            TowerManager.instance.SetSelectTowersMode();
        }

        // reset results menus
        if (gameOver) gameOver.SetActive(false);
        if (gameWin) gameWin.SetActive(false);
        if (resultsScreen) resultsScreen.SetActive(false);
    }

    /// <summary>
    /// Disables all player-interactable objects
    /// </summary>
    public void DisableInteractables()
    {
        if (Camera.main)
        {
            // Disable camera controls
            Camera.main.GetComponent<CameraController>().enabled = false;
        }

        // Disable health bar
        if(healthBar) healthBar.gameObject.SetActive(false);

        // Disable gold display
        if (goldText) goldText.transform.parent.gameObject.SetActive(false);

        // Disable tower spawner and tower selector
        if (TowerManager.HasInstance()) TowerManager.instance.gameObject.SetActive(false);
    }
}