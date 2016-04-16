// Creep.cs
// General-purpose enemy movement and stats script. Also defines enemy behaviour.

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]
public class Creep : MonoBehaviour
{
    public GameObject spawner = null;

    private GameObject goldDrop;
    public int goldValue; //This is the Creep's worth. See in inspector.

    public AudioClip goldClip;
    private AudioSource source;
    
    //Vars for the health bar
    private GameObject healthBar; //link to the health bar game object
    private Quaternion healthBarRot; //var for storing the initial rotation of the health bar
    private MeshRenderer healthBarRenderer; //for enabling and disabling renderer
    private Vector3 healthBarScale; //to scale properly

    /// <summary>
    /// The navmesh agent for this creep.
    /// </summary>
    private NavMeshAgent agent;

    /// <summary>
    /// The creep's current target on the path. If the target is the last target in its tree, the creep will despawn when it reaches it.
    /// </summary>
    public CreepTarget target
    {
        get
        {
            return _target;
        }
        set
        {
            _target = value;
            agent.SetDestination(_target.transform.position);
        }
    }
    private CreepTarget _target = null;

    /// <summary>
    /// The previous target on the path. Used for calculating progress towards the goal.
    /// </summary>
    public CreepTarget previous = null;

    /// <summary>
    /// How much health the unit currently has.
    /// </summary>
	public int health = 50;
    private int startingHealth;

    /// <summary>
    /// How much damage the unit inflicts when it successfully leaks through.
    /// </summary>
    public int leakDamage = 1;

    /// <summary>
    /// 
    /// </summary>
    public float progress
    {
        get
        {
            return Vector3.Distance(transform.position, previous.transform.position) / Vector3.Distance(target.transform.position, previous.transform.position) + nodesHit;
        }
    }

    /// <summary>
    /// Number of nodes this creep has hit already.
    /// </summary>
    public int nodesHit = 0;

    [Header("Creep Heal")]
    [SerializeField]
    private bool applyHeal = false;

    /// <summary>
    /// Time between target burns, in seconds.
    /// </summary>
    public float healTime = 0.5f;

    private float elapsedHealTime = 0.0f;

    /// <summary>
    /// Damage to inflict on the target each burn.
    /// </summary>
    ///
    [SerializeField]
    private int heal = 0;

    /// <summary>
    /// The current speed of the creep.
    /// </summary>
    public float speed { get { return GetComponent<NavMeshAgent>().speed; } set { GetComponent<NavMeshAgent>().speed = value; } }

    /// <summary>
    /// The current acceleration of the creep.
    /// </summary>
    public float acceleration { get { return GetComponent<NavMeshAgent>().acceleration; } set { GetComponent<NavMeshAgent>().acceleration = value; } }

    void Awake ()
    {
        agent = GetComponent<NavMeshAgent>();
        source = GetComponent<AudioSource>();
    }

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

    }

    /// <summary>
    /// Called whenever enemy takes damage
    /// </summary>
    /// <param name="amount">How much damage the enemy will take</param>
	void OnApplyDamage (int amount)
    {
		health -= amount;
        if (health <= 0)
        {
            OnDeath();
        }
    }

    /// <summary>
    /// Called whenever enemy recieves healing
    /// /// </summary>
    /// <param name="amount">How much health the enemy will receive</param>
	void OnApplyHeal(int amount)
    {
        health += amount;
        health = Mathf.Min(startingHealth, health);
    }

    // Update
    void Update ()
    {
        if (health > 0 && health < startingHealth)
        {
            healthBarRenderer.enabled = true; //begin rendering health bar once creep takes damage for first time
        }

        //rescale healthBar
        Vector3 newScale = new Vector3(((healthBarScale.x * health) / startingHealth), healthBarScale.y, healthBarScale.z);
        Vector4 newColor = new Vector4((1.0f - (health / startingHealth)), (health / startingHealth), 0, 1);

        healthBar.transform.localScale = newScale;
        healthBarRenderer.material.color = newColor;

        if (applyHeal == true)
        {
            elapsedHealTime += Time.deltaTime;
            if (elapsedHealTime > healTime)
            {
                this.SendMessage("OnApplyHeal", heal);

                elapsedHealTime = 0;
            }
        }

    }

    // LateUpdate
    void LateUpdate ()
    {
        //rotation correction for health bar
        healthBar.transform.rotation = healthBarRot; 
    }

    //
    void OnDeath()
    {
        GameObject goldDropObject = Resources.Load("Effects/Gold Drop") as GameObject;
        goldDrop = Instantiate(goldDropObject);
        goldDrop.transform.position = transform.position;
        goldDrop.transform.rotation = Camera.main.transform.rotation;

        //By default, the 'GoldDrop' gameObject is set to false so it's animation doesn't play. this activates the object, and also the DestroyByTime script on the go.
        goldDrop.SetActive(true); 
        goldDrop.transform.position = transform.position;
        goldDrop.GetComponent<TextMesh>().text = "+" + goldValue + " Gold";

        Player.instance.AddGold(goldValue); //increments the player's gold by the set gold value associated with the creep
        Player.instance.SendMessage("OnKillCreep", null, SendMessageOptions.DontRequireReceiver);
        spawner.SendMessage("OnCreepDeath", null, SendMessageOptions.DontRequireReceiver);

        GameObject ss = Instantiate(Resources.Load("SoundSource") as GameObject, transform.position, Quaternion.identity) as GameObject;
        ss.GetComponent<AudioSource>().PlayOneShot(goldClip);

        Destroy(gameObject);
    }

    // OnTriggerEnter
    void OnDespawn()
    {
        spawner.SendMessage("OnCreepDespawn", null, SendMessageOptions.DontRequireReceiver);
        Player.instance.SendMessage("RemoveHealth", leakDamage, SendMessageOptions.DontRequireReceiver);
        Player.instance.SendMessage("OnMissCreep", null, SendMessageOptions.DontRequireReceiver);
        Destroy(gameObject);
    }

}
