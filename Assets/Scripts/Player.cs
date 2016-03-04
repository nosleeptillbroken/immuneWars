// Player.cs
// Defines behaviour for all player-related tasks.

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player : MonoBehaviour
{

    private static Player _current = null;
    public static Player current { get { return _current; } }

    [Header("Health")]
    // Create the slider for display of the health bar
    // This will notify the player of their current progress towards a lose condition
    public Slider healthBar;
    // Set the default player max health to 50 and create a variable for current health
    // This will be used to display the health bar for player and provide a loss condition
    public int maxHealth = 50;
    private int _currentHealth = 50;
    public int currentHealth { get { return _currentHealth; } }
    
    [Header("Gold")]
    // This is the text element for the gold
    public Text goldDisplay;

    public int maxGold = int.MaxValue;
    private int _currentGold = 0;
    public int currentGold { get { return _currentGold; } }

    //
    public GameObject gameOver;

    private bool towersMode = false;

    public bool placeTowers { get { return towersMode; } }
    public bool selectTowers { get { return !towersMode; } }

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

    // Use this for initialization
    void Start ()
    {
        _current = this;

		// If they were not linked in editor find the Health Bar and Game Over objects for the scene
        if(!healthBar) healthBar = GameObject.Find("Health Bar").GetComponent<Slider>();
        if (!goldDisplay) goldDisplay = GameObject.Find("Gold Display").GetComponent<Text>();
        if (!gameOver) gameOver = GameObject.Find("Game Over");
		
		// Get a reference to the tower spawner
        // set health to max at beginning
        _currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.minValue = 0;
	}
	
	// Update is called once per frame
	void Update ()
    {
        // Health bar displayed reflects current health
        healthBar.value = currentHealth;

        // Update text on the gold display
        goldDisplay.text = "Gold: " + currentGold;
	}

    /// <summary>
    /// Called when the player's health reaches 0
    /// </summary>
    void OnDie()
    {
        // Disable health bar
        healthBar.transform.parent.gameObject.SetActive(false);

        // Disable gold display
        goldDisplay.gameObject.SetActive(false);

        // Disable this player
        gameObject.SetActive(false);

        // Disable tower spawner and tower selector
        TowerSpawner.current.gameObject.SetActive(false);
        TowerSelector.current.gameObject.SetActive(false);

        // Display game over menu
        gameOver.SetActive(true);

        // Hide the tower ghost
        GameObject towerGhost = GameObject.Find("Tower Ghost");
        if (towerGhost)
        {
            towerGhost.SetActive(false);
        }

    }

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
            OnDie();
        }
    }

    /// <summary>
    /// Add gold to the player. If the resulting gold does not exceed max gold, add gold and return true. Else, add up to max gold and return false.
    /// </summary>
    /// <param name="amount">The amount to add</param>
    /// <returns>If the gold does not exceed max gold, return true and add gold. Else, add up to max gold and return false.</returns>
    public bool AddGold(int amount)
    {
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

}
