using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player : MonoBehaviour {

    public int maxHealth = 50;
    public int currentHealth;
    
    public Slider healthBar;
    public GameObject gameOver;
    
	// Use this for initialization
	void Start ()
    {
        if(!healthBar) healthBar = GameObject.Find("Health Bar").GetComponent<Slider>();
        if (!gameOver) gameOver = GameObject.Find("Game Over");
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.minValue = 0;
	}
	
	// Update is called once per frame
	void Update ()
    {
        healthBar.value = currentHealth;
        if(currentHealth <= 0)
        {
            healthBar.transform.parent.gameObject.SetActive(false);
            gameOver.SetActive(true);

            GameObject.Find("Tower Spawner").SetActive(false);
            GameObject.Find("Tower Ghost").SetActive(false);

            gameObject.SetActive(false);
        }
	}
}
