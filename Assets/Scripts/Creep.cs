// Creep.cs
// General-purpose enemy movement and stats script. Also defines enemy behaviour.

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Creep : MonoBehaviour
{
    public GameObject GoldDrop;
    [SerializeField]
    private float goldValue;
    private static float currentGold = 0;
    public Text CurrencyText;

    // Target for despawning
    private Transform target;

    // The navmesh agent for this 
    NavMeshAgent agent;

    /// <summary>
    /// How much health the unit currently has.
    /// </summary>
	public int Health = 50;

    /// <summary>
    /// How much damage the unit inflicts when it successfully leaks through.
    /// </summary>
    public int LeakDamage = 1;
        
	// Use this for initialization
	void Start ()
    {
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
        CurrencyText = GameObject.FindGameObjectWithTag("MoneyText").GetComponent<Text>(); //
        CurrencyText.text = "";

    }

    /// <summary>
    /// Called whenever enemy takes damage
    /// </summary>
    /// <param name="damage">How much damage the enemy will take</param>
	void TakeDamage(int damage)
    {
		Health -= damage;
		if (Health <= 0)
        {
			Destroy(gameObject);
            Instantiate(GoldDrop,transform.position,Camera.main.transform.rotation);
            GoldDrop.SetActive(true);
            GoldDrop.GetComponent<TextMesh>().text = "+" + goldValue + " Gold";
            currentGold += goldValue;
            //Debug.Log(currentGold);

		}
	}

    // Update
	void Update ()
    {
        GoldDrop.transform.position = transform.position;
        if (agent && target)
        {
            agent.SetDestination(target.position);
        }
        CurrencyText.text = "Gold: " + currentGold;
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
