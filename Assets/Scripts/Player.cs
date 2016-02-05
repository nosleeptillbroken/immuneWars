// Player.cs
// Defines behaviour for all player-related tasks.

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player : MonoBehaviour {

    public int maxHealth = 50;
    public int currentHealth;
    
    public Slider healthBar;
    public GameObject gameOver;
    public TowerSpawner towerSpawner; // tower spawner childed to this gameobject
    public TowerSelector towerSelector; // tower selector childed to this gameobject

    // Use this for initialization
    void Start ()
    {
        if(!healthBar) healthBar = GameObject.Find("Health Bar").GetComponent<Slider>();
        if (!gameOver) gameOver = GameObject.Find("Game Over");

        towerSpawner = GetComponentInChildren<TowerSpawner>();
        towerSelector = GetComponentInChildren<TowerSelector>();

        towerSpawner.gameObject.SetActive(false);
        towerSelector.gameObject.SetActive(true);

        // set health to max at beginning
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.minValue = 0;
	}
	
	// Update is called once per frame
	void Update ()
    {
        healthBar.value = currentHealth;

        // if player health is zero
        if(currentHealth <= 0)
        {
            // disable health bar
            healthBar.transform.parent.gameObject.SetActive(false);

            // display game over menu
            gameOver.SetActive(true);

            // hide the tower ghost
            GameObject towerGhost = GameObject.Find("Tower Ghost");
            if (towerGhost)
            {
                towerGhost.SetActive(false);
            }

            // disable this player (will disable children)
            gameObject.SetActive(false);
        }
	}
}
