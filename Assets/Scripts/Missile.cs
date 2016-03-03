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

	// Use this for initialization
	void Start ()
    {
		
	}

	// Update is called once per frame
	void Update () 
	{
        if (tower == null || attributes == null || target == null)
        {
            Destroy(gameObject);
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
			// call the function TakeDamage(10) in the hit object, if any
			//i need to call the specific bullet, otherwise all enemies take damage. take not for future sessions
			//transform.position = Vector3.Lerp (transform.position, bulletman, _TurretScript.speed * Time.deltaTime);
			other.gameObject.SendMessage("TakeDamage", attributes.damage, SendMessageOptions.DontRequireReceiver);

            CreepEffect effect = other.gameObject.GetComponent<CreepEffect>();
            if(effect == null)
            {
                effect = other.gameObject.AddComponent<CreepEffect>();
                if(attributes.applyBurn)
                {
                    effect.burnCount = attributes.burnCount;
                    effect.burnDamage = attributes.burnDamage;
                    effect.burnTime = attributes.burnTime;
                }
                if(attributes.applySlow)
                {
                    effect.slowFactor = attributes.slowFactor;
                    effect.slowTime = attributes.slowTime;
                }
            }
            else
            {

                if (attributes.applyBurn)
                {
                    effect.burnCount = Mathf.Max(effect.burnCount, attributes.burnCount);
                    effect.burnDamage = Mathf.Max(effect.burnDamage,attributes.burnDamage);
                    effect.burnTime = Mathf.Min(effect.burnTime, attributes.burnTime);
                }
                if (attributes.applySlow)
                {
                    effect.slowFactor = Mathf.Min(effect.slowFactor,attributes.slowFactor);
                    effect.slowTime = Mathf.Max(effect.slowTime, attributes.slowTime);
                }
            }
		}
		Destroy(gameObject); // bullet suicides after hitting anything
	}
	
}
