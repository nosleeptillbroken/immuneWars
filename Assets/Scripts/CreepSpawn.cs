using UnityEngine;
using System.Collections;

public class CreepSpawn : MonoBehaviour {
	public GameObject creep;

	public int creepCount;
	public float spawnWait;
	public float startWait;
	public float waveWait;

	void Start ()
	{
		StartCoroutine (SpawnWaves ());
	}

	IEnumerator SpawnWaves ()
	{
		yield return new WaitForSeconds (startWait);
		while (true)
		{
			for (int i = 0;i < creepCount; i++) 
			{
				Vector3 spawnPosition = transform.position;
				Quaternion spawnRotation = Quaternion.identity;
				Instantiate (creep, spawnPosition, spawnRotation);
				yield return new WaitForSeconds (spawnWait);
			}
			yield return new WaitForSeconds (waveWait);
		}
	}
}
