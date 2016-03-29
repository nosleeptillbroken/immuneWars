using UnityEngine;
using System.Collections;

public class CreepEffect : MonoBehaviour {

    [Header("General")]

    public int damage = 0;
    //public int heal = 5;

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

    [Header("Creep Heal")]

    public bool applyHeal = true;

    /// <summary>
    /// Time between target heals, in seconds.
    /// </summary>
    public float healTime = 1.5f;

    private float elapsedHealTime = 0.0f;

    /// <summary>
    /// health to inflict on the target each burn.
    /// </summary>
    public int healAmount = -5; //uses math logic to add to health. (two negatives make a positive durr)

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
        bool remainingEffects = false;



        if(damage > 0)
        {
            creep.SendMessage("OnApplyDamage", damage);
            damage = 0;
           // Debug.Log("ouch");
        }

        if (burnCount > 0)
        {
            remainingEffects = true;
            elapsedBurnTime += Time.deltaTime;

            if (elapsedBurnTime > burnTime)
            {
                creep.SendMessage("OnApplyDamage", burnDamage);
                burnCount -= 1;
                elapsedBurnTime = 0;
            }
        }

        if (applyHeal == true)
        {
            remainingEffects = true;
            elapsedHealTime += Time.deltaTime;

            if (elapsedHealTime > healTime)
            {
                creep.SendMessage("OnApplyDamage", healAmount); //heal amount gets passed into the 'damage' var inside the Creep.cs script.
                elapsedHealTime = 0;
            }
        }

        if(slowTime > 0.0f)
        {
            remainingEffects = true;
            creep.speed = originalSpeed * slowFactor;
            slowTime -= Time.deltaTime;
        }
        else
        {
            creep.speed = originalSpeed;
        }
        
        if(remainingEffects == false)
        {
            Destroy(this);
        }
	}
}
