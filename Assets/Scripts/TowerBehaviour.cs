// Tower.cs
// Contains behaviour for spawning projectiles (Missiles) and aiming them at a creep based on targeting parameters.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Tower Class
// One in every tower. Stores all information needed for detecting enemies. Stores range, speed, damage it inflicts
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
    public TowerAttributes upgradeAttributes = new TowerAttributes();

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
        Damage,
        Speed
    }
    
    // current targeting mode used by the tower
    public TargetingMode targetingMode = TargetingMode.Distance;
    public int sortOrder = 1;

    // time since last shot
    private float elapsedTime = 0.0f;

    #region Tower Upgrades

    [System.Serializable]
    public class UpgradePath
    {
        public List<TowerAttributes> list;
    }

    public List<UpgradePath> upgradeTree;

    public List<int> upgradeLevels;

    public void Upgrade(int path)
    {
        if(path < upgradeLevels.Count)
        {
            int currentLevel = upgradeLevels[path];
            if(currentLevel < upgradeTree[path].list.Count - 1)
            {
                upgradeLevels[path] += 1;
                ApplyAttributes(path);
            }
        }
    }

    public TowerAttributes GetCurrentUpgrade(int path)
    {
        return upgradeTree[path].list[upgradeLevels[path]];
    }

    public TowerAttributes GetNextUpgrade(int path)
    {
        return upgradeTree[path].list[upgradeLevels[path]+1];
    }

    public bool CanUpgrade(int path)
    {
        return upgradeLevels[path] + 1 < upgradeTree[path].list.Count;
    }

    #endregion

    #region Tower Attributes

    void ApplyAttributes(int path)
    {
        Mesh upgradeMesh = GetCurrentUpgrade(path).mesh;
        Material upgradeMaterial = GetCurrentUpgrade(path).material;

        if (upgradeMesh)
        {
            GetComponent<MeshFilter>().mesh = upgradeMesh;
        }

        if(upgradeMaterial)
        {
            GetComponent<MeshRenderer>().material = upgradeMaterial;
        }

        upgradeAttributes = new TowerAttributes();
        for (int i = 0; i < upgradeTree.Count; i++)
        {
            if (upgradeLevels[i] >= 0)
            {
                upgradeAttributes += GetCurrentUpgrade(i);
            }
        }

        ApplyAttributes();
    }

    void ApplyAttributes()
    {
        TowerAttributes compositeAttributes = attributes + upgradeAttributes;
        rangeVolume.GetComponent<SphereCollider>().radius = compositeAttributes.range;
    }

    #endregion

    #region Tower List

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

            // target enemy if none targeted
            TargetNewEnemy();
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

            // target enemy if none targeted
            TargetNewEnemy();
        }
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
    /// Tells the tower to target the next enemy, if an enemy isn't targeted
    /// </summary>
    public void TargetNewEnemy()
    {
        currentTarget = null;

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
        else if (targetingMode == TargetingMode.Speed)
        {
            SortTargetsBySpeed();
        }

        if (targets.Count > 0)
        {
            currentTarget = targets[0].transform;
        }
    }

    #endregion

    #region Target Sorting

    /// <summary>
    /// Sorts targets by distance
    /// </summary>
    public void SortTargetsByDistance()
    {
        // sort the array according to distance
        targets.Sort
        (
            (l, r) => Vector3.Distance(l.transform.position, transform.position).CompareTo(Vector3.Distance(r.transform.position, transform.position)) * sortOrder
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
            (l, r) => (l.health - r.health) * sortOrder
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
            (l, r) => (l.leakDamage - r.leakDamage) * sortOrder
        );
    }

    /// <summary>
    /// Sorts targets by speed
    /// </summary>
    public void SortTargetsBySpeed()
    {
        // sort the array according to distance
        targets.Sort
        (
            (l, r) => ((int)(l.speed*10) - (int)(r.speed*10)) * sortOrder
        );
    }

    #endregion

    #region MonoBehaviour

    void Start()
    {
        if(upgradeLevels.Count == 0)
        {
            for(int i = 0; i < upgradeTree.Count; i++)
            {
                upgradeLevels.Add(-1);
            }
        }    
        
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
        
        // if there's any target in the range...
        if (currentTarget)
        {
            //print(selectedTarget);

            elapsedTime += Time.deltaTime;

            TowerAttributes compositeAttributes = attributes + upgradeAttributes;

            // if it's time to shoot...
            if (elapsedTime > 1.0f / compositeAttributes.rateOfFire)
            {
                TargetNewEnemy();

                GameObject missileObj = Instantiate(missilePrefab.gameObject); //instantiates the bullet to shoot
                missileObj.transform.position = missileSpawn.position;
                missileObj.transform.rotation = missileSpawn.rotation;

                Missile missile = missileObj.GetComponent<Missile>();
                missile.tower = this;
                missile.target = currentTarget.transform;
                missile.attributes = compositeAttributes;


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

        TowerAttributes compositeAttributes = attributes + upgradeAttributes;
        if (attributes != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, compositeAttributes.range);
        }
        if (missileSpawn != null && currentTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(missileSpawn.position, currentTarget.position);
        }
    }

    #endregion

}
