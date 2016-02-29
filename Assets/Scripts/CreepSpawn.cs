// CreepSpawn.cs
// Defines behaviour for spawning creeps, how to spawn creeps, and where to spawn creeps.

using UnityEngine;
using System.Collections;

[System.Serializable]
public class CreepPair // provides a pair of GameObject to select creep type and number of that type to spawn
{
    public GameObject creep;
    public int number = 1; // should initialize to 1, but it doesnt seem to
}

[System.Serializable]
public class WaveList // allows for the creation of a 2 dimensional array of CreepPairs 
{
    public CreepPair[] waveList;
}

public class CreepSpawn : MonoBehaviour {
    public WaveList[] levelList; // an array of waves will create the level

	// public int creepCount;
	public float spawnWait; // time to wait between spawns
	public float startWait; // time to wait before first spawn of level
	public float waveWait; // time to wait in between waves

	void Start ()
	{
		StartCoroutine (SpawnWaves ());
	}

	IEnumerator SpawnWaves ()
	{
		yield return new WaitForSeconds (startWait);
        for (int i = 0; i < levelList.Length; i++) // for every wave in the level
        {
            for (int j = 0; j < levelList[i].waveList.Length; j++) // for every creep type listed in the wave
            {
                for (int k = 0; k < levelList[i].waveList[j].number; k++) // for every creep of that type in the wave
                {
                    Vector3 spawnPosition = transform.position;
                    Quaternion spawnRotation = Quaternion.identity;
                    Instantiate (levelList[i].waveList[j].creep, spawnPosition, spawnRotation); // spawn wave for this wave, of this type
                    yield return new WaitForSeconds (spawnWait); // wait before spawning next creep
                }
            }
            yield return new WaitForSeconds(waveWait); // wait before starting next wave
        }
        // fall out the bottom
        Debug.Log("Level Ended"); // log that level is over, will need a call for level completed scene to begin
        // Debug.Break();
	}
}
