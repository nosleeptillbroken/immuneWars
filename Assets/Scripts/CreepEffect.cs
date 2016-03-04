using UnityEngine;
using System.Collections;

public class CreepEffect : MonoBehaviour {

    [Header("Creep Slowing")]

    public bool applySlow = false;

    /// <summary>
    /// How many seconds to slow the target for.
    /// </summary>
    public float slowTime = 0.0f;

    /// <summary>
    /// How much to slow the target's speed by.
    /// </summary>
    public float slowFactor = 0.0f;
    private float originalSpeed = 0.0f;

    [Header("Creep Burn")]

    public bool applyBurn = false;

    /// <summary>
    /// Time between target burns, in seconds.
    /// </summary>
    public float burnTime = 0.0f;

    private float elapsedBurnTime = 0.0f;

    /// <summary>
    /// Number of burns to inflict on the target.
    /// </summary>
    public int burnCount = 0;

    /// <summary>
    /// Damage to inflict on the target each burn.
    /// </summary>
    public int burnDamage = 0;

    //
    private Creep creep = null;

    //
    void Start ()
    {
        creep = GetComponent<Creep>();

        originalSpeed = creep.speed;
	}
	
	//
	void Update ()
    {
        if (burnCount > 0)
        {
            elapsedBurnTime += Time.deltaTime;

            if (elapsedBurnTime > burnTime)
            {
                creep.health -= burnDamage;
                burnCount -= 1;
                elapsedBurnTime = 0;
            }
        }

        if(slowTime > 0.0f)
        {
            creep.speed = originalSpeed * slowFactor;
            slowTime -= Time.deltaTime;
        }
        else
        {
            creep.speed = originalSpeed;
        }
        
        if(burnCount <= 0 && slowTime <= 0.0f)
        {
            Destroy(this);
        }
	}
}
