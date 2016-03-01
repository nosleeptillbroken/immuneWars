// CreepGoal.cs
// Defines any behaviour to be perfomed when an enemy successfully enters the creep goal.

using UnityEngine;
using System.Collections;

public class CreepGoal : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        GameObject playerObject = GameObject.Find("Player");
        if (playerObject && other.CompareTag("Enemy"))
        {
            playerObject.GetComponent<Player>().currentHealth -= other.gameObject.GetComponent<Creep>().leakDamage;
        }
    }
}
