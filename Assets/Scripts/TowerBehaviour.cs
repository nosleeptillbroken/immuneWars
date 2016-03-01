// Tower.cs
// Contains behaviour for spawning projectiles (Missiles) and aiming them at a creep based on targeting parameters.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Tower Class
// One in every tower. Stores all information needed for detecting enemies. Stores range, speed, damage it inflicts
[RequireComponent(typeof(TowerAttributes))]
[RequireComponent(typeof(CapsuleCollider))]
public class TowerBehaviour : MonoBehaviour
{

    // the missile this tower fires
    public Missile missilePrefab;
    // the range volume childed to this object
    public GameObject rangeVolume = null;
    // the missile spawn point transform childed to this object
    public Transform missileSpawn = null;

    // the tower attributes for this tower
    public TowerAttributes attributes = null;

    // list of potential targets
    public List<Creep> targets = new List<Creep>();
    // current target (based on sort settings)
    public Transform currentTarget = null;

    /// <summary>
    /// Enum containing the different targeting modes available.
    /// </summary>
    public enum TargetingMode
    {
        Distance,
        Health,
        Damage
    }
    
    // current targeting mode used by the tower
    public TargetingMode targetingMode = TargetingMode.Distance;

    // time since last shot
    private float elapsedTime = 0.0f;

    #region TowerAttributes

    void ApplyAttributes()
    {
        rangeVolume.GetComponent<SphereCollider>().radius = attributes.range;
    }

    #endregion

    #region TowerList

    /// <summary>
    /// List of towers in use (STATIC: list is the same for all towers)
    /// </summary>
    static List<TowerBehaviour> towerList = new List<TowerBehaviour>(128);
    // Count of towers (STATIC)
    static int towerCount = 0;

    /// <summary>
    /// Tower ID in list
    /// </summary>
    int id = -1;

    /// <summary>
    /// The number of towers currently alive
    /// </summary>
    /// <returns></returns>
    public static int NumTowers()
    {
        return towerCount;
    }

    /// <summary>
    /// Returns the current tower's ID
    /// </summary>
    public int Id()
    {
        return id;
    }

    /// <summary>
    /// Returns a tower from towerList
    /// </summary>
    public static TowerBehaviour GetTowerByIndex(int i)
    {
        return towerList[i];
    }

    #endregion

    #region Range Messages

    /// <summary>
    /// Called when an enemy enters tower's range volume
    /// </summary>
    /// <param name="other"></param>
    public void OnRangeEnter(Collider other)
    {
        if (other.CompareTag("Enemy")) // only objects with tag 'Enemy' are added to the target list!
        {
            // add enemy to targets
            targets.Add(other.GetComponent<Creep>());
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
            targets.Remove(other.GetComponent<Creep>());
            currentTarget = null;
        }

        // target new enemy
        TargetEnemy();
    }

    #endregion

    #region Targeting

    public void ClearEmptyTransforms()
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
    }

    /// <summary>
    /// Tells the tower to target the next enemy
    /// </summary>
    public void TargetEnemy()
    {
        // if target destroyed or not selected yet...
        if (currentTarget == null)
        {
            ClearEmptyTransforms();
            // select the closest one
            if (targetingMode == TargetingMode.Distance)
            {
                SortTargetsByDistance();
            }
            else if (targetingMode == TargetingMode.Health)
            {
                SortTargetsByHealth();
            }
            else if (targetingMode == TargetingMode.Damage)
            {
                SortTargetsByDamage();
            }

            if (targets.Count > 0)
            {
                currentTarget = targets[0].transform;
            }
        }
    }

    /// <summary>
    /// Sorts targets by distance
    /// </summary>
	public void SortTargetsByDistance()
    {
        // sort the array according to distance
        targets.Sort
        (
            (l, r) => Vector3.Distance(l.transform.position, transform.position).CompareTo(Vector3.Distance(r.transform.position, transform.position))
        );
    }

    /// <summary>
    /// Sorts targets by health
    /// </summary>
	public void SortTargetsByHealth()
    {
        // sort the array according to health
        targets.Sort
        (
            (l, r) => (r.health - l .health)
        );
    }

    /// <summary>
    /// Sorts targets by health
    /// </summary>
    public void SortTargetsByDamage()
    {
        // sort the array according to distance
        targets.Sort
        (
            (l, r) => (r.leakDamage - l.leakDamage)
        );
    }

    #endregion

    #region MonoBehaviour

    void Start()
    {
        if (rangeVolume == null)
        {
            rangeVolume = transform.FindChild("RangeVolume").gameObject;
        }

        if (missileSpawn == null)
        {
            missileSpawn = transform.FindChild("MissileSpawn");
        }

        if (attributes == null)
        {
            attributes = GetComponent<TowerAttributes>();
        }
        ApplyAttributes();        

        // add this tower to the tower list and set id
        towerList.Add(this);
        id = towerList.Count - 1;
        gameObject.name = "Tower " + Id() + " (" + gameObject.name + ")";
    }

    void OnDestroy()
    {
        // remove from the tower list and reset id
        towerList.Remove(this);
        towerCount -= 1;
        id = -1;
    }

    void Update()
    {
        ApplyAttributes(); // update the attributes in case something has changed

        TargetEnemy(); // update the selected target and look at it

        // if there's any target in the range...
        if (currentTarget)
        {
            //print(selectedTarget);

            elapsedTime += Time.deltaTime;

            // if it's time to shoot...
            if (elapsedTime > 1.0f / attributes.rateOfFire)
            {
                GameObject missileObj = Instantiate(missilePrefab.gameObject); //instantiates the bullet to shoot
                missileObj.transform.position = missileSpawn.position;
                missileObj.transform.rotation = missileSpawn.rotation;

                Missile missile = missileObj.GetComponent<Missile>();
                missile.tower = this;
                missile.target = currentTarget.transform;
                missile.attributes = attributes;

                // reset time
                elapsedTime = 0.0f;
            }
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

    void OnDrawGizmos()
    {
        if (attributes != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, attributes.range);
        }
        if (missileSpawn != null && currentTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(missileSpawn.position, currentTarget.position);
        }
    }

    #endregion

}
