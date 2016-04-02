// Missile.cs
// Defines behaviour and stats for projectiles that shoot from towers to damage enemies.

using UnityEngine;
using System.Collections;

public class Missile : MonoBehaviour
{
    /// <summary>
    /// Tower that fired the missile.
    /// </summary>
	public TowerBehaviour tower = null;
	
    /// <summary>
    /// Attributes for the missile.
    /// </summary>
    public TowerAttributes attributes = null;

    /// <summary>
    /// The target transform.
    /// </summary>
    public Transform target = null;

    /// <summary>
    /// Object instantiated to inflict AoE Damage, if applicable.
    /// </summary>
    public GameObject AOEDamager;

    // Use this for initialization
    /*
	void Start ()
    {
		// nothing to initialize
	}
	*/

    // Update is called once per frame
    void Update () 
	{
        if (tower == null || attributes == null || target == null)
        {
            Destroy(gameObject);
            /*
             * Instead of emulating MoveTowards consider using:
             * transform.position = Vectro3.MoveTowards(transform.position, towerTarget.position, bulletSpeed * Time.deltaTime)
             * Rotation can still be adjusted as required seperately
             */

            // Move the bullet object toward the target object until collision
        }
        else
        {
			Vector3 targetDir = target.position - transform.position;
			Vector3 newDir = Vector3.RotateTowards (transform.forward, targetDir, 2.0f, 0.0f);
			transform.rotation = Quaternion.LookRotation(newDir);
			transform.Translate (Vector3.forward * attributes.missileSpeed * Time.deltaTime);
            if(targetDir.sqrMagnitude < 0.0001f)
            {
                target = null;
            }
		}
	
	}

	void OnTriggerEnter(Collider other)
    {
		if (other.gameObject.CompareTag("Enemy"))
        {
			// Call the function TakeDamage(10) in the hit object, if any
			// Call the specific bullet, otherwise all enemies take damage. take not for future sessions
			//transform.position = Vector3.Lerp (transform.position, bulletman, _TurretScript.speed * Time.deltaTime);
            
            if (attributes.applyAOE)
            {
                Collider[] aoeCreeps = Physics.OverlapSphere(transform.position, attributes.AOERadius, LayerMask.GetMask("Creeps"),QueryTriggerInteraction.Ignore);
                foreach(Collider creep in aoeCreeps)
                {
                    ApplyCreepEffectWithAttributes(creep.gameObject, attributes);
                }
            }
            else
            {
                ApplyCreepEffectWithAttributes(other.gameObject, attributes);
            }

            Destroy(gameObject); // Destroy this object after collision
        }
	}
	
    void ApplyCreepEffectWithAttributes(GameObject creep, TowerAttributes attributes)
    {
        if (creep.GetComponent<Creep>())
        {
            CreepEffect effect = creep.GetComponent<CreepEffect>();
            if (effect == null) effect = creep.gameObject.AddComponent<CreepEffect>();

            effect.damage += attributes.damage;

            //
            if (attributes.applyBurn)
            {
                effect.burnCount = Mathf.Max(effect.burnCount, attributes.burnCount);
                effect.burnDamage = Mathf.Max(effect.burnDamage, attributes.burnDamage);
                effect.burnTime = (effect.burnTime > 0) ? Mathf.Min(effect.burnTime, attributes.burnTime) : attributes.burnTime;
            }

            //
            if (attributes.applySlow)
            {
                effect.slowFactor = (effect.slowFactor > 0) ? Mathf.Min(effect.slowFactor, attributes.slowFactor) : attributes.slowFactor;
                effect.slowTime = Mathf.Max(effect.slowTime, attributes.slowTime);
            }
        }
        else
        {
            Debug.LogWarning("Attempted to apply CreepEffect to non-Creep; doing nothing.");
        }
    }

}
