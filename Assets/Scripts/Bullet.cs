using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	public Transform towerTarget;
	public Tower _TurretScript;

    [HideInInspector]
	public float bulletSpeed;

    public int damage = 1;
    public int slowFactor = 0;
    public int slowDuration = 0;

	// Use this for initialization
	void Start ()
    {
		
	}

	// Update is called once per frame
	 void Update () 
	{
		if (towerTarget)
        {
			Vector3 targetDir = towerTarget.position - transform.position;
			Vector3 newDir = Vector3.RotateTowards (transform.forward, targetDir, 2.0f, 0.0f);
			transform.rotation = Quaternion.LookRotation (newDir);
			transform.Translate (Vector3.forward * bulletSpeed * Time.deltaTime);
		}
        else
        {
            Destroy(this.gameObject);
        }
	
	}
	void OnCollisionEnter(Collision other)
    {
		if (other.gameObject.CompareTag("Enemy"))
        {
			// call the function TakeDamage(10) in the hit object, if any
			//i need to call the specific bullet, otherwise all enemies take damage. take not for future sessions
			//transform.position = Vector3.Lerp (transform.position, bulletman, _TurretScript.speed * Time.deltaTime);
			other.gameObject.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
		}
		Destroy(gameObject); // bullet suicides after hitting anything
	}
	
}
