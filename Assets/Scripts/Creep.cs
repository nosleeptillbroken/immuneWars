// Creep.cs
// General-purpose enemy movement and stats script. Also defines enemy behaviour.

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Creep : MonoBehaviour
{
    public GameObject GoldDrop;
    [SerializeField]
    private float goldValue; //This is the Creep's worth. Se in inspector.
    private static float currentGold = 0; //This is the player's total money. This is what they'll use to puchase towers/upgrades, etc.
    public Text CurrencyText; //The gui text element

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

        /*
         * As far as I know, you canot edit text element in a prefab during Runtime, so I've broken the current prefab instance of 'InGame Screen' so that I can
         * edit the gold metric live. You may, in fact be able to edit text elements of a prefab during runtime but I had some issues so I did it this way.
         * As a note to Dan, if you can think of a quick way to fix this, by all means go ahead during the glue code phase. 
         */
        CurrencyText = GameObject.FindGameObjectWithTag("MoneyText").GetComponent<Text>(); //Find the UI Text element that is the player's gold.
        CurrencyText.text = ""; //resets the players bank.

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
            /*
             * This is where I do the logic for the player's gold. Because of the way OnDestroy works, if you were to, let's say, exit the game mid level and there were still
             * the text elements on the screen, Unity wouldn't be able to clean the mup properly and they would stick around the next iteration of the game.
             * This way, things happen properly, and only after the creep has been destroyed.
            */
            GoldDrop.SetActive(true); //By default, the 'GoldDrop' gameObject is set to false so it's animation doesn't play. this activates the object, and also the DestroyByTime script on the go.
            GoldDrop.GetComponent<TextMesh>().text = "+" + goldValue + " Gold";
            currentGold += goldValue; //increments the player's gold by the set gold value associated with the creep
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
        CurrencyText.text = "Gold: " + currentGold; //updates the player's bank.
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
