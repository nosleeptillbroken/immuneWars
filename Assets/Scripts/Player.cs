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
        if(!healthBar) healthBar = GameObject.Find("Health Bar").GetComponent<Slider>();
        if (!gameOver) gameOver = GameObject.Find("Game Over");

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
