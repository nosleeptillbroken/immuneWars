// Player.cs
// Defines behaviour for all player-related tasks.

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player : MonoBehaviour {

	// Set the default player max health to 50 and create a variable for current health
	// This will be used to display the health bar for player and provide a loss condition
    public int maxHealth = 50;
    public int currentHealth;
    
	// Create the slider for display of the health bar
	// This will notify the player of their current progress towards a lose condition
    public Slider healthBar;
    public GameObject gameOver;
    public TowerSpawner towerSpawner; // tower spawner childed to this gameobject
    
	// Use this for initialization
	void Start ()
    {
		// If they were not linked in editor find the Health Bar and Game Over objects for the scene
        if(!healthBar) healthBar = GameObject.Find("Health Bar").GetComponent<Slider>();
        if (!gameOver) gameOver = GameObject.Find("Game Over");
		
		// Get a reference to the tower spawner
        towerSpawner = GetComponentInChildren<TowerSpawner>();

        // set health to max at beginning
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.minValue = 0;
	}
	
	// Update is called once per frame
	void Update ()
    {
		// Health bar displayed reflects current health
        healthBar.value = currentHealth;

        // If player health is zero
        if(currentHealth <= 0)
        {
            // Disable health bar
            healthBar.transform.parent.gameObject.SetActive(false);

            // Display game over menu
            gameOver.SetActive(true);
            
            // Hide the tower ghost
            GameObject.Find("Tower Ghost").SetActive(false);

            // Disable this player (will disable children)
            gameObject.SetActive(false);
        }
	}
}
