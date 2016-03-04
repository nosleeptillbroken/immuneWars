// Missile.cs
// Defines behaviour and stats for projectiles that shoot from towers to damage enemies.

using UnityEngine;
using System.Collections;

public class Missile : MonoBehaviour
{
	// Create references for towerTarget and _TurretScript
	// To be used in Tower.cs
	public Transform towerTarget;
	public Tower _TurretScript;
	
	// Variable bulletSpeed for manipuation in scripts
    [HideInInspector]
	public float bulletSpeed;

	// Variables for damage, slowing and slowDuration
	/*
		Consider using flags for the effect which can call methods on the creep targetted
		This will make it easier to script the effects
	*/
    public int damage = 1;
    public int slowFactor = 0;
    public int slowDuration = 0;

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
		if (towerTarget)
        {
            /*
             * Instead of emulating MoveTowards consider using:
             * transform.position = Vectro3.MoveTowards(transform.position, towerTarget.position, bulletSpeed * Time.deltaTime)
             * Rotation can still be adjusted as required seperately
             */

            // Move the bullet object toward the target object until collision
			Vector3 targetDir = towerTarget.position - transform.position;
			Vector3 newDir = Vector3.RotateTowards (transform.forward, targetDir, 2.0f, 0.0f);
			transform.rotation = Quaternion.LookRotation (newDir);
			transform.Translate (Vector3.forward * bulletSpeed * Time.deltaTime);
		}
        else
        {
            // If target is destroyed before bullet reaches it, destroy this bullet
            Destroy(this.gameObject);
        }
	
	}

	void OnCollisionEnter(Collision other)
    {
		if (other.gameObject.CompareTag("Enemy"))
        {
			// Call the function TakeDamage(10) in the hit object, if any
			// Call the specific bullet, otherwise all enemies take damage. take not for future sessions
			//transform.position = Vector3.Lerp (transform.position, bulletman, _TurretScript.speed * Time.deltaTime);
			other.gameObject.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
            // Could also send flags for special bullet effects here
		}
		Destroy(gameObject); // Destroy this object after collision
	}
	
}
