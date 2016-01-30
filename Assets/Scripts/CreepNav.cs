using UnityEngine;
using System.Collections;

public class CreepNav : MonoBehaviour
{
	private Transform target;
	NavMeshAgent agent;

	void Start () 
	{
		// The creep is referenced with the NavMesh so it can interact.
		agent = GetComponent<NavMeshAgent> ();
		// Creeps are instantiated each wave, and references are made to the despawn point for pathing.
		GameObject despawnGameObject = GameObject.FindWithTag ("Despawn");
		if (despawnGameObject != null)
			target = despawnGameObject.transform;
		if (despawnGameObject == null)
			Debug.Log ("Cannot find despawn object");
	}

	void Update () 
	{
		agent.SetDestination (target.position);
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Despawn") 
		{
			Destroy (gameObject);
			/*	
			 * {
			 * 	Code for Losing Lives.
			 * }
			*/
		}
	}
}
