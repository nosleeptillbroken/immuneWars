// CreepGoal.cs
// Defines any behaviour to be perfomed when an enemy successfully enters the creep goal.

using UnityEngine;
using System.Collections;

public class CreepGoal : MonoBehaviour
{
	void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            other.SendMessage("OnDespawn", null, SendMessageOptions.DontRequireReceiver);
        }
    }
}
