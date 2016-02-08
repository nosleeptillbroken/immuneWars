// Creep.cs
// General-purpose enemy movement and stats script. Also defines enemy behaviour.

using UnityEngine;
using System.Collections;

public class Creep : MonoBehaviour
{
    //Vars for the health bar
    [SerializeField] private GameObject healthBar; //link to the health bar game object
    private Quaternion healthBarRot; //var for storing the initial rotation of the health bar
    private MeshRenderer healthBarRenderer; //for enabling and disabling renderer
    private Vector3 healthBarScale; //to scale properly

    // Target for despawning
    private Transform target;

    // The navmesh agent for this 
    NavMeshAgent agent;

    /// <summary>
    /// How much health the unit currently has.
    /// </summary>
	public int Health = 50;
    private float startingHealth;

    /// <summary>
    /// How much damage the unit inflicts when it successfully leaks through.
    /// </summary>
    public int LeakDamage = 1;
        
	// Use this for initialization
	void Start ()
    {
        // The creep is referenced with the NavMesh so it can interact.
        agent = GetComponent<NavMeshAgent>();

        //Store the initial rotation of the health bar
        healthBarRot = healthBar.transform.rotation;
        healthBarScale = healthBar.transform.localScale;
        healthBarRenderer = GetComponentsInChildren<MeshRenderer>()[1]; //associate renderer for healthbar
        healthBarRenderer.enabled = false; //make invisible from start
        startingHealth = Health; //store full health value

        // Creeps are instantiated each wave, and references are made to the despawn point for pathing.
        GameObject despawnGameObject = GameObject.FindWithTag("Despawn");

        if (despawnGameObject != null)
        {
            target = despawnGameObject.transform;
        }
        if (despawnGameObject == null)
        {
            Debug.Log("Cannot find despawn object");
        }

    }

    /// <summary>
    /// Called whenever enemy takes damage
    /// </summary>
    /// <param name="damage">How much damage the enemy will take</param>
	void TakeDamage(int damage)
    {
        if (startingHealth == Health)
        {
            healthBarRenderer.enabled = true; //begin rendering health bar once creep takes damage for first time
        }
       
        // lower health
		Health -= damage;
		if (Health <= 0)
        {
			Destroy(gameObject);
		}

       //rescale healthBar
        Vector3 newScale = new Vector3(((healthBarScale.x * Health) / startingHealth), healthBarScale.y, healthBarScale.z);
        Vector4 newColor = new Vector4((1 - (Health / startingHealth)), (Health / startingHealth), 0, 1);
        healthBar.transform.localScale = newScale;
        healthBarRenderer.material.color = newColor;
	}

    // Update
	void Update ()
    {
        if (agent && target)
        {
            agent.SetDestination(target.position);
        }
    }

    // LateUpdate
    void LateUpdate ()
    {
        healthBar.transform.rotation = healthBarRot; //rotation correction for health bar
    }

    // OnTriggerEnter
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Despawn"))
        {
            Destroy(gameObject);
            /*	
			 * {
			 * 	Code for Losing Lives.
			 * }
			*/
        }
    }

}
