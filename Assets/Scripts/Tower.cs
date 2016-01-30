using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Tower Class
// One in every tower. Stores all information needed for detecting enemies. Stores range, speed, damage it inflicts
public class Tower : MonoBehaviour
{

    // bullet type this tower uses
	public Rigidbody bulletPrefab;
    // spawn point of the bullet
	public Transform bulletSpawn;

    // list of potential targets
	public List<Transform> targets;
    // current target (based on sort settings)
	public Transform currentTarget;
    
    // shots per second
    public float rateOfFire = 1.0f;
    // speed of the bullet
    public float bulletSpeed;
    
    // time since last shot
    private float elapsedTime = 0.0f;
    // time between shots, in seconds
    private float shotInterval = 0.0f;

    // List of towers in use (STATIC: list is the same for all towers)
    static List<Tower> towerList = new List<Tower>(128);
    // Count of towers (STATIC)
    static int towerCount = 0;

    // Tower ID in list;
    int id = -1;

    //public int range;
    //public float fireSpeed;
   // public float damage;

    // Returns a tower from towerList
    public static Tower GetTowerByIndex(int i)
    {
        return towerList[i];
    }

    // Returns the number of towers currently alive
    public static int NumTowers()
    {
        return towerCount;
    }

    // Returns the current tower's ID
    public int Id()
    {
        return id;
    }

    // Use this for initialization
    void Start()
    {
        // initialize targets
		targets = new List<Transform>();
		currentTarget = null;

        shotInterval = 1 / rateOfFire;
        
        // add this tower to the tower list and set id
        towerList.Add(this);
        id = towerList.Count-1;
        gameObject.name = "Tower " + Id() + " (" + gameObject.name + ")";
    }

    void OnDestroy()
    {
        // remove from the tower list and reset id
        towerList.Remove(this);
        towerCount -= 1;
        id = -1;
    }

    /// <summary>
    /// Called when an enemy enters tower's range volume
    /// </summary>
    /// <param name="other"></param>
	public void OnRangeEnter(Collider other)
    {
		if (other.CompareTag("Enemy")) // only objects with tag 'Enemy' are added to the target list!
        { 
            // add enemy to targets
			targets.Add(other.transform);
		}
	}

    /// <summary>
    /// Called when an enemy leaves tower's range volume
    /// </summary>
    /// <param name="other"></param>
    public void OnRangeExit(Collider other)
    {
		if (other.CompareTag("Enemy"))
        {
            // remove enemy from targets
			targets.Remove(other.transform);
            currentTarget = null;
		}

        // target new enemy
        TargetEnemy();
	}

    /// <summary>
    /// Tells the tower to target the next enemy
    /// </summary>
	public void TargetEnemy()
    {

		if (currentTarget == null) { // if target destroyed or not selected yet...
			SortTargetsByDistance ();  // select the closest one
			if (targets.Count > 0)
				currentTarget = targets [0];    
		} 
	}

    /// <summary>
    /// Sorts targets by distance
    /// </summary>
	public void SortTargetsByDistance()
    {

        // First, make sure the array is purged of any empty transforms
        for (int e = 0; e < targets.Count; e++)
        {
            // if the current transform is null
            if (targets[e] == null)
            {
                // remove current transform
                targets.RemoveAt(e);
                e -= 1;
            }
        }

        // Second, sort the array according to distance
        targets.Sort
        (
            delegate(Transform t1, Transform t2)
		    {
		    	return Vector3.Distance(t1.position, transform.position).CompareTo(Vector3.Distance(t2.position, transform.position));
		    }
        );

    }

    void Update()
    {
		TargetEnemy(); // update the selected target and look at it

        // if there's any target in the range...
        if (currentTarget)
        {
            //print(selectedTarget);

            // if it's time to shoot...
            if (elapsedTime >= shotInterval)
            { 
				GameObject bulletObj = Instantiate(Resources.Load("Bullet") as GameObject); //instantiates the bullet to shoot
				bulletObj.transform.position = bulletSpawn.position;
				bulletObj.transform.rotation = bulletSpawn.rotation;
				bulletObj.GetComponent<Bullet>().towerTarget = currentTarget.transform;
				bulletObj.GetComponent<Bullet>().bulletSpeed = bulletSpeed;

                // reset time
                elapsedTime = 0.0f;
			}

            elapsedTime += Time.deltaTime;
		}
	}

    void LateUpdate()
    {
        if (currentTarget)
        {
            // rotate towards selected object 
            Quaternion newRotation = Quaternion.LookRotation(currentTarget.position - transform.position, Vector3.forward);
            newRotation.x = 0.0f;
            newRotation.z = 0.0f;
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * 8);
        }
    }

}
