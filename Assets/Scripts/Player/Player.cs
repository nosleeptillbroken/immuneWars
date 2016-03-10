// Player.cs
// Defines behaviour for all player-related tasks.

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Player : Singleton<Player>
{

    [Header("Health")] ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Set the default player max health to 50 and create a variable for current health
    /// This will be used to display the health bar for player and provide a loss condition
    /// </summary>
    public int maxHealth = 50;

    [SerializeField]
    private int _currentHealth = 0;
    public int currentHealth { get { return _currentHealth; } }
    
    [Header("Gold")] ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // maximum amount of gold the player can have
    public int maxGold = int.MaxValue;

    // amount of gold the player starts with
    public int startingGold = 30;

    // current gold the player has
    private int _currentGold = 0;
    public int currentGold { get { return _currentGold; } }

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
    public int killsToWin = 0;    
    
    private int killScoreMult = 10;      
    private int totalKills = 0;     //Number of enemies killed by the player.
    private int missScoreMult = 10;
    private int totalMisses = 0;    //Number of enemies leaked, added with total kills for win condition.
    
    #region MonoBehaviour

    // Use this for initialization
    void Start ()
    {
		// If they were not linked in editor find the Health Bar and Game Over objects for the scene
        if(!healthBar) healthBar = GameObject.Find("Health Display").GetComponent<Slider>();
        if (!goldText) goldText = GameObject.Find("Gold Display").GetComponent<Text>();

        if (!gameOver) gameOver = GameObject.Find("Game Over");
        if (!gameWin) gameWin = GameObject.Find("Game Win");
        if (!resultsScreen) resultsScreen = GameObject.Find("Results Screen");

        gameOver.SetActive(false);
        gameWin.SetActive(false);
        resultsScreen.SetActive(false);

        // Get a reference to the tower spawner
        // set health to max at beginning
        if(_currentHealth == 0) _currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.minValue = 0;

        ResetState();

    }
	
	// Update is called once per frame
	void Update ()
    {
        // Health bar displayed reflects current health
        healthBar.value = currentHealth;
        scoreText.text = "Score: " + (totalKills * killScoreMult).ToString(); //Adjust the player's score killing enemies.
        killsText.text = "Kills: " + totalKills.ToString(); //Adjust the player's total kills.
        
        // Update text on the gold display
        goldText.GetComponentInChildren<Text>().text = (infiniteGold ? "∞" : currentGold.ToString());    
        
        if(Debug.isDebugBuild && Input.GetButtonDown("Debug Next Level"))
        {
            NextLevel();
        }    
    }

    #endregion
    
    public void CheckStatus()
    {
        if (currentHealth <= 0 && loseFlag == false) // If player health is zero
        {
            loseFlag = true;
            OnLose();
        }
        else if ((totalKills + totalMisses) >= killsToWin && winFlag == false) //Game Win State.
        {
            winFlag = true;
            OnWin();
        }
    }

    #region Messages

    /// <summary>
    /// Function handles what is activated/deactivated on game win.
    /// </summary>
    void OnWin()
    {

        // Enable game win screen
        Debug.Log("Player Won");
        gameWin.SetActive(true);

        DisableInteractables();
    }

    /// <summary>
    /// Called when the player's health reaches 0
    /// </summary>
    void OnLose()
    {

        // Display game over menu
        Debug.Log("Player Lost");
        gameOver.SetActive(true);
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
    /// Called when the GameManager loads a new level
    /// </summary>
    void OnLevelChanged()
    {
        ResetInteractables();
        ResetState();
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
        if(infiniteGold)
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
        if(infiniteGold)
        {
            return true;
        }

        if(currentGold - amount >= 0)
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

        killsToWin = 0;

        totalKills = 0;
        totalMisses = 0;
    }

    /// <summary>
    /// Transition to the next level
    /// </summary>
    /// <param name="sceneName"></param>
    public void NextLevel()
    {
    }

    /// <summary>
    /// Function reloads current level, selected from gameover menu.
    /// </summary>
    public void ReloadCurrentLevel()
    {
    }

    public void ReturnToMainMenu()
    {
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
        
        healthBar.gameObject.SetActive(true);
    
        goldText.transform.parent.gameObject.SetActive(true);

        TowerManager.instance.SetSelectTowersMode();

        // reset results menus
        gameOver.SetActive(false);
        gameWin.SetActive(false);
        resultsScreen.SetActive(false);

        TowerManager.instance.SetSelectTowersMode();
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
        healthBar.gameObject.SetActive(false);

        // Disable gold display
        goldText.transform.parent.gameObject.SetActive(false);
        
        // Disable tower spawner and tower selector
        TowerManager.instance.gameObject.SetActive(false);

        // Hide the tower ghost
        GameObject towerGhost = GameObject.Find("Tower Ghost");
        if (towerGhost)
        {
            towerGhost.SetActive(false);
        }
    }
}
