// CreepSpawn.cs
// Defines behaviour for spawning creeps, how to spawn creeps, and where to spawn creeps.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public class WaveCreep // provides a pair of GameObject to select creep type and number of that type to spawn
{
    public GameObject creep;
    public int number = 1; // should initialize to 1, but it doesnt seem to
}

[System.Serializable]
public class CreepWave // allows for the creation of a 2 dimensional array of CreepPairs 
{
    public List<WaveCreep> creepList;
}

public class CreepSpawner : CreepTarget
{

    public List<CreepWave> waveList; // an array of waves will create the level

    /// <summary>
    /// Time to wait between creep spawns.
    /// </summary>
    public float spawnWait;

    /// <summary>
    /// Time to wait at the beginning of the level.
    /// </summary>
    //public float startWait;

    /// <summary>
    /// Time to wait between waves.
    /// </summary>
    //public float waveWait;

    /// <summary>
    /// If the next wave has been set to start.
    /// </summary>
    public bool startNextWave { get { return _startNextWave; } }
    private bool _startNextWave = false;

    /// <summary>
    /// If the current wave is complete.
    /// </summary>
    public bool waveComplete { get { return _waveComplete; } }
    private bool _waveComplete = false;

    /// <summary>
    /// If the current level is complete.
    /// </summary>
    public bool levelComplete { get { return _levelComplete; } }
    private bool _levelComplete = false;

    /// <summary>
    /// The number of waves to be created by the creep sapwner.
    /// </summary>
    public int waves { get { return _waves; } }
    private int _waves = 0;

    /// <summary>
    /// The number of creeps currently alive in the wave.
    /// </summary>
    public int livingCreepsInWave { get { return _livingCreepsInWave; } }
    private int _livingCreepsInWave = 0;
    
    /// <summary>
    /// The current wave the spawner is on.
    /// </summary>
    public int currentWave { get { return _currentWave; } }
    private int _currentWave = 0;

    /// <summary>
    /// Button that starts next wave.
    /// </summary>
    private Button startNextWaveButton = null;

    /// <summary>
    /// Toggle that auto starts next wave.
    /// </summary>
    private Toggle autoNextWaveToggle = null;

    void Start()
    {
        // get the next wave button object (if it exists)
        GameObject startNextWaveObject = GameObject.Find("Start Next Wave");
        if (startNextWaveObject)
        {
            startNextWaveButton = startNextWaveObject.GetComponent<Button>();
            startNextWaveButton.onClick.AddListener(() => OnStartNextWave());
        }


        GameObject autoWaveObject = GameObject.Find("Auto Next Wave");
        if(autoWaveObject) autoNextWaveToggle = autoWaveObject.GetComponent<Toggle>();

        Player.instance.SendMessage("OnNewCreepSpawner", null, SendMessageOptions.DontRequireReceiver);

        _waves = waveList.Count;

        StartCoroutine(SpawnWaves());
    }

    private static int _totalLevelCreeps = 0;

    /// <summary>
    /// Gets the number of creeps to be spawned by all spawners in this level.
    /// </summary>
    /// <returns></returns>
    public static int GetLevelTotalCreeps()
    {
        int result = 0;
        
        // if the creep's spawner is not a CreepSpawner, it will not be correctly counted in the level's total creeps
        foreach(Creep creep in FindObjectsOfType<Creep>())
        {
            // add 1 to make it counted so we don't have an early win from killing creeps spawned through other means
            if(creep.spawner && !creep.spawner.GetComponent<CreepSpawner>())
            {
                result += 1;
            }
        }

        // for the rest of the level creeps, count the number of creeps in each spawner's waves.
        foreach(CreepSpawner spawner in FindObjectsOfType<CreepSpawner>())
        {
            result += spawner.GetTotalCreeps();
        }

        _totalLevelCreeps = Mathf.Max(result, _totalLevelCreeps);

        return _totalLevelCreeps;
    }

    /// <summary>
    /// Gets the number of creeps to be spawned by this spawner.
    /// </summary>
    /// <returns></returns>
    public int GetTotalCreeps()
    {
        int result = 0;
        foreach (CreepWave wl in waveList)
        {
            foreach (WaveCreep cp in wl.creepList)
            {
                result += cp.number;
            }
        }
        return result;
    }

    /// Waits for startWait seconds
    /// Iterates through every waveList in levelList
    /// Iterates through every CreepPair in every waveList
    /// Instantiates number creeps of type creep as defined by every CreepPair
    /// Waits after Instantiating every creep spawnWait seconds
    /// Waits waveWait seconds after every waveList finishes iteration
    IEnumerator SpawnWaves()
    {

        yield return new WaitUntil(()=> _startNextWave || (autoNextWaveToggle && autoNextWaveToggle.isOn));

        OnLevelStart();

        for (int i = 0; i < waveList.Count; i++) // for every wave in the level
        {
            _waveComplete = false;

            _livingCreepsInWave = 0;

            foreach (WaveCreep cw in waveList[i].creepList)
            {
                _livingCreepsInWave += cw.number;
            }

            _currentWave += 1;
            OnWaveStart();

            for (int j = 0; j < waveList[i].creepList.Count; j++) // for every creep type listed in the wave
            {
                for (int k = 0; k < waveList[i].creepList[j].number; k++) // for every creep of that type in the wave
                {
                    Vector3 spawnPosition = transform.position;
                    Quaternion spawnRotation = Quaternion.identity;

                    // spawn wave for this wave, of this type
                    GameObject newCreepObject = Instantiate(waveList[i].creepList[j].creep, spawnPosition, spawnRotation) as GameObject;
                    Creep newCreep = newCreepObject.GetComponent<Creep>();
                    // set the creep's spawner so it can notify when it has died or despawned
                    newCreep.spawner = this.gameObject;
                    DirectToNextNode(newCreep);

                    yield return new WaitForSeconds(spawnWait); // wait before spawning next creep
                }
            }

            // wait until last creep in wave is killed before ending the wave
            yield return new WaitUntil(() => _waveComplete);

            // wait until the next wave is set to start (or auto wave toggle is on)
            OnWaveEnd();
            yield return new WaitUntil(() => _startNextWave || (autoNextWaveToggle && autoNextWaveToggle.isOn));

        }

        // call level end callback
        OnLevelEnd();

        // fall out the bottom
        Debug.Log("Spawning Finished");
        // Debug.Break();
    }

    /// <summary>
    /// Called when the start next wave button is pressed.
    /// </summary>
    void OnStartNextWave()
    {
        _startNextWave = true;
    }

    /// <summary>
    /// Called when the level begins, after the start wait period.
    /// </summary>
    void OnLevelStart()
    {
        _totalLevelCreeps = 0;
    }

    /// <summary>
    /// Called when the level ends.
    /// </summary>
    void OnLevelEnd()
    {

    }

    /// <summary>
    /// Called when the wave begins, after the wave wait period.
    /// </summary>
    void OnWaveStart()
    {
        _startNextWave = false;
        if (startNextWaveButton) startNextWaveButton.interactable = false;
    }

    /// <summary>
    /// Called when the wave ends.
    /// </summary>
    void OnWaveEnd()
    {
        if (startNextWaveButton) startNextWaveButton.interactable = true;
    }

    /// <summary>
    /// Called when a creep spawned by this spawner is killed.
    /// </summary>
    void OnCreepDeath()
    {
        UpdateWaveCreepFlags();
    }

    /// <summary>
    /// Called when a creep spawned by this spawner despawns (successfully traverses the level).
    /// </summary>
    void OnCreepDespawn()
    {
        UpdateWaveCreepFlags();
    }

    /// <summary>
    /// Updates t
    /// </summary>
    void UpdateWaveCreepFlags()
    {
        _livingCreepsInWave -= 1;
        if (_livingCreepsInWave <= 0)
        {
            _waveComplete = true;
            _waves -= 1;
        }
        if(_waves <= 0)
        {
            _levelComplete = true;
            Player.instance.SendMessage("CheckStatus", null, SendMessageOptions.DontRequireReceiver);
        }
    }

}
