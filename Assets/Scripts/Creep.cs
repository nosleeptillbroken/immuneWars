// Creep.cs
// General-purpose enemy movement and stats script. Also defines enemy behaviour.

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Creep : MonoBehaviour
{
    private GameObject goldDrop;
    public int goldValue; //This is the Creep's worth. See in inspector.
    
    //Vars for the health bar
    private GameObject healthBar; //link to the health bar game object
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
	public int health = 50;
    private float startingHealth;

    /// <summary>
    /// How much damage the unit inflicts when it successfully leaks through.
    /// </summary>
    public int leakDamage = 1;

    /// <summary>
    /// The current speed of the creep.
    /// </summary>
    public float speed { get { return GetComponent<NavMeshAgent>().speed; } set { GetComponent<NavMeshAgent>().speed = value; } }

    /// <summary>
    /// The current acceleration of the creep.
    /// </summary>
    public float acceleration { get { return GetComponent<NavMeshAgent>().acceleration; } set { GetComponent<NavMeshAgent>().acceleration = value; } }

    // Use this for initialization
    void Start ()
    {
        // Initialize the health bar for the creep
        healthBar = Instantiate(Resources.Load("Effects/Health Bar") as GameObject);
        healthBar.transform.SetParent(transform, false);

        // Store the initial rotation of the health bar
        healthBarRot = healthBar.transform.rotation;
        healthBarScale = healthBar.transform.localScale;
        healthBarRenderer = healthBar.GetComponent<MeshRenderer>(); //associate renderer for healthbar
        healthBarRenderer.enabled = false; //make invisible from start
        startingHealth = health; //store full health value
        
        // The creep is referenced with the NavMesh so it can interact.
        agent = GetComponent<NavMeshAgent>();

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
	void TakeDamage (int damage)
    {
		health -= damage;
    }

    // Update
	void Update ()
    {
        if (agent && target)
        {
            agent.SetDestination(target.position);
        }

        if (health > 0 && health < startingHealth)
        {
            healthBarRenderer.enabled = true; //begin rendering health bar once creep takes damage for first time
        }
        else if (health <= 0)
        {
            /*
             * This is where I do the logic for the player's gold. Because of the way OnDestroy works, if you were to, let's say, exit the game mid level and there were still
             * the text elements on the screen, Unity wouldn't be able to clean the mup properly and they would stick around the next iteration of the game.
             * This way, things happen properly, and only after the creep has been destroyed.
            */

            GameObject goldDropObject = Resources.Load("Effects/Gold Drop") as GameObject;
            goldDrop = Instantiate(goldDropObject);
            goldDrop.transform.position = transform.position;
            goldDrop.transform.rotation = Camera.main.transform.rotation;

            goldDrop.SetActive(true); //By default, the 'GoldDrop' gameObject is set to false so it's animation doesn't play. this activates the object, and also the DestroyByTime script on the go.
            goldDrop.transform.position = transform.position;
            goldDrop.GetComponent<TextMesh>().text = "+" + goldValue + " Gold";
            Player.current.AddGold(goldValue); //increments the player's gold by the set gold value associated with the creep

            Destroy(gameObject);
        }

        //rescale healthBar
        Vector3 newScale = new Vector3(((healthBarScale.x * health) / startingHealth), healthBarScale.y, healthBarScale.z);
        Vector4 newColor = new Vector4((1.0f - (health / startingHealth)), (health / startingHealth), 0, 1);

        healthBar.transform.localScale = newScale;
        healthBarRenderer.material.color = newColor;

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
