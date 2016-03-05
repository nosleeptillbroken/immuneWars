// Player.cs
// Defines behaviour for all player-related tasks.

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{

    private static Player _current = null;
    public static Player current { get { return _current; } }

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

    // current gold the player has
    [SerializeField]
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
    private bool lossFlag = false;

    [Header("Score Keeping")]

    /// <summary>
    /// Number of enemies that must be killed to win the level.
    /// </summary>
    public int killsToWin = 0;    
    
    private int score = 0;
    private int totalKills = 0;     //Number of enemies killed by the player.
    private int totalMissed = 0;    //Number of enemies leaked, added with total kills for win condition.
    
    private bool towersMode = false;
    public bool placeTowers { get { return towersMode; } }
    public bool selectTowers { get { return !towersMode; } }

    #region Tower Modes

    //
    public void ToggleTowersMode()
    {
        towersMode = !towersMode;
        TowerSelector.current.gameObject.SetActive(selectTowers);
        TowerSpawner.current.gameObject.SetActive(placeTowers);
    }

    //
    public void PlaceTowersMode()
    {
        TowerSelector.current.gameObject.SetActive(false);
        TowerSpawner.current.gameObject.SetActive(true);
    }

    //
    public void SelectTowersMode()
    {
        TowerSpawner.current.gameObject.SetActive(false);
        TowerSelector.current.gameObject.SetActive(true);
    }

    #endregion

    #region MonoBehaviour

    //
    void Awake ()
    {
        _current = this;
    }

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
	}
	
	// Update is called once per frame
	void Update ()
    {
        // Health bar displayed reflects current health
        healthBar.value = currentHealth;
        scoreText.text = "Score: " + score.ToString();        //Adjust the player's score killing enemies.
        killsText.text = "Kills: " + totalKills.ToString();   //Adjust the player's total kills.
        
        // Update text on the gold display
        goldText.GetComponentInChildren<Text>().text = (infiniteGold ? "∞" : currentGold.ToString());
        

        if ((totalKills) >= killsToWin && winFlag == false) //Game Win State.
        {
            OnWin();
            winFlag = true;
        }
    }

    #endregion

    #region Messages

    /// <summary>
    /// Function handles what is activated/deactivated on game win.
    /// </summary>
    void OnWin()
    {
        // set one-time flag
        winFlag = true;

        // Enable game win screen
        Debug.Log("Game Won");
        gameWin.SetActive(true);

        DisableInteractables();
    }

    /// <summary>
    /// Called when the player's health reaches 0
    /// </summary>
    void OnLose()
    {
        // set one-time flag
        lossFlag = true;

        // Display game over menu
        Debug.Log("Game Lost");
        gameOver.SetActive(true);

        DisableInteractables();
    }

    /// <summary>
    /// Function adds to score when creep is killed.
    /// </summary>
    void OnKillCreep()
    {
        score += 10;
        totalKills += 1;
        if (totalKills > killsToWin)
        {
            OnWin();
        }
    }

    /// <summary>
    /// Function to add to missed kills when creep reaches despawn point.
    /// </summary>
    void OnMissCreep()
    {
        totalMissed += 1;
        killsToWin -= 1;
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

        // If player health is zero
        if (currentHealth <= 0)
        {
            OnLose();
        }
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
    /// Function reloads current level, selected from gameover menu.
    /// </summary>
    public void ReloadCurrentLevel()
    {
        //Reactivate camera controls once game is over.
        Camera.main.GetComponent<CameraController>().enableEdgePanning = true;
        Camera.main.GetComponent<CameraController>().enableRotation = true;
        Camera.main.GetComponent<CameraController>().enableZoom = true;

        TowerSpawner.current.gameObject.SetActive(true);
        lossFlag = false;
        winFlag = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Transition to a new level
    /// </summary>
    /// <param name="sceneName"></param>
    public void TransitionToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Function displays score screen, selected from game win menu.
    /// </summary>
    public void DisplayScoreScreen()
    {
        //Activate score screen.
        resultsScreen.SetActive(true);      

        //More values can be added in the future for different creep types.
        int CKV = 10;   //Creep Kill Value
        int CKVtotal = CKV * totalKills;
        int CMV = -50;  //Creep Miss Value
        int CMVtotal = CMV * totalMissed;

        //Text for total creeps killed, and associated score.
        Text SSkills = GameObject.Find("KilledAmount").GetComponent<Text>();
        SSkills.text = totalKills + " x " + CKV + " = " + CKVtotal;
        //Text for total creeps killed, and associated deduction.
        Text SSmissed = GameObject.Find("MissedAmount").GetComponent<Text>();
        SSmissed.text = totalMissed + " x (" + CMV + ") = " + CMVtotal;
        //Total Score.
        Text SStotal = GameObject.Find("TotalAmount").GetComponent<Text>();
        SStotal.text = (CKVtotal + CMVtotal).ToString();
    }

    /// <summary>
    /// Disables all player-interactable objects
    /// </summary>
    public void DisableInteractables()
    {
        // Disable camera controls
        Camera.main.GetComponent<CameraController>().enabled = false;

        // Disable health bar
        healthBar.gameObject.SetActive(false);

        // Disable gold display
        goldText.transform.parent.gameObject.SetActive(false);
        
        // Disable tower spawner and tower selector
        TowerSpawner.current.gameObject.SetActive(false);
        TowerSelector.current.gameObject.SetActive(false);

        // Hide the tower ghost
        GameObject towerGhost = GameObject.Find("Tower Ghost");
        if (towerGhost)
        {
            towerGhost.SetActive(false);
        }
    }
}
