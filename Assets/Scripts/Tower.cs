﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Tower Class
// One in every tower. Stores all information needed for detecting enemies. Stores range, speed, damage it inflicts
public class Tower : MonoBehaviour
{

	[Range(0.0f,10.0f)]
	public float shotInterval = 0.5f; // interval between shots
	public Rigidbody bulletPrefab; // drag the bullet prefab here
	public Transform bulletSpawn;
	[Range(0.0f,10000.0f)]
	public float speed = 1000.0f;
	public float shootSpeed = 1.0f;
	private float shootTime = 0.0f;
	public List<Transform> targets;
	public Transform selectedTarget;
	private Transform myTransform;
	public bool hit = false;
	Vector3 distance;
	float dist = 12;
	[SerializeField] private float bulletSpeed;

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
		targets = new List<Transform>();
		selectedTarget = null;
		myTransform = transform;
        towerList.Add(this);
        id = towerList.Count-1;
        gameObject.name = "Tower " + Id() + " (" + gameObject.name + ")";
    }

    void OnDestroy()
    {
        towerList.Remove(this);
        towerCount -= 1;
        id = -1;
    }

	void OnTriggerEnter(Collider other){
		if (other.tag == "Enemy"){ // only objects with tag 'Enemy' are added to the target list!
			targets.Add(other.transform);
			other.gameObject.GetComponent<MovingEnemy> ().listIndex = targets.Count-1;
			print(targets);
		}
	}

	void OnTriggerExit(Collider other){
		if (other.tag == "Enemy"){
			targets.Remove(other.transform);
			other.gameObject.GetComponent<MovingEnemy> ().listIndex = -1;
		}
	}

	public void TargetEnemy(){

		if (selectedTarget == null) { // if target destroyed or not selected yet...
			SortTargetsByDistance ();  // select the closest one
			if (targets.Count > 0)
				selectedTarget = targets [0];    
		} 
	}

	public void SortTargetsByDistance(){
		/*targets.Sort(delegate(Transform t1, Transform t2)
			{ 
			//return Vector3.Distance(t1.position, myTransform.position).CompareTo(Vector3.Distance(t2.position, myTransform.position));
			return Vector3.Distance(t1.position, myTransform.position).CompareTo(Vector3.Distance(t2.position, myTransform.position));
		});*/

		for (int e = 0; e < targets.Count - 1; e++) {

			if (targets [e + 0] == null) {
				targets.RemoveAt (e + 0);
				e -= 1;
				continue;
			}

			if (targets [e + 1] == null) {
				targets.RemoveAt (e + 1);
				e -= 1;
				continue;
			}

			float sqrMag1 = (targets [e + 0].position - myTransform.position).sqrMagnitude;
			float sqrMag2 = (targets [e + 1].position - myTransform.position).sqrMagnitude;

			if (sqrMag2 < sqrMag1) {
				Transform tempStore = targets [e];
				targets [e] = targets [e + 1];
				targets [e + 1] = tempStore;
				e = 0;
			}
		}
	}




	void Update(){
		TargetEnemy(); // update the selected target and look at it
		if (selectedTarget){ // if there's any target in the range...
			transform.LookAt(selectedTarget); // aim at it
			transform.eulerAngles = new Vector3(0,transform.eulerAngles.y,0);
			//print(selectedTarget);
			if (Time.time >= shootTime){ // if it's time to shoot...
				GameObject bulletObj = Instantiate(Resources.Load("Bullet") as GameObject); //instantiates the bullet to shoot
				bulletObj.transform.position = bulletSpawn.position;
				bulletObj.transform.rotation = bulletSpawn.rotation;
				bulletObj.GetComponent<bullet>().towerTarget = selectedTarget.transform;
				bulletObj.GetComponent<bullet>().bulletSpeed = bulletSpeed;
				shootTime = Time.time + shotInterval; // set time for next shot
			}
		}
	}

}