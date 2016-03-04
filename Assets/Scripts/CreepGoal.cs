// CreepGoal.cs
// Defines any behaviour to be perfomed when an enemy successfully enters the creep goal.

using UnityEngine;
using System.Collections;

public class CreepGoal : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        //References the ScoreKeeper object.
        GameObject scoreKeeperObject = GameObject.Find("ScoreKeeper");

        //If an enemy reaches the despawn point, subtract health from the player's total.
        if (scoreKeeperObject && other.CompareTag("Enemy"))
        {
            scoreKeeperObject.GetComponent<ScoreKeeper>().currentHealth -= other.gameObject.GetComponent<Creep>().LeakDamage;
        }
    }
}
