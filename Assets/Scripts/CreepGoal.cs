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
        GameObject playerObject = GameObject.Find("Player"); // find Player object
		// if player object exists and colliding object is enemy
        if (playerObject && other.CompareTag("Enemy"))
        {
            Player.current.RemoveHealth(other.gameObject.GetComponent<Creep>().leakDamage);
            Destroy(other.gameObject);
        }
    }
}
