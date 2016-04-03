using UnityEngine;
using System.Collections.Generic;

public class LevelData : MonoBehaviour {

    [Header("General")]

    public string level = null;
    public string nameKey = null;

    public enum LevelDifficulty { easyDifficulty, mediumDifficulty, hardDifficulty };
    public LevelDifficulty difficulty;

    [Header("Completion")]

    public bool completed = false;
    public LevelData[] completeRequired;

    /// <summary>
    /// Whether or not this level's start requirements are met.
    /// </summary>
    public bool canStart
    {
        get
        {
            bool res = true;
            foreach(LevelData level in completeRequired)
            {
                res = res && level.completed;
            }
            return res;
        }
    }

    public static int count
    {
        get
        {
            return FindObjectsOfType<LevelData>().Length;
        }
    }

    public static int completedCount
    {
        get
        {
            LevelData[] levels = FindObjectsOfType<LevelData>();
            int numCompleted = 0;
            foreach(LevelData level in levels)
            {
                numCompleted += level.completed ? 1 : 0;
            }
            return numCompleted;
        }
    }

    /// <summary>
    /// The levels still required to complete in order to play this level.
    /// </summary>
    public LevelData[] incomplete
    {
        get
        {
            List<LevelData> res = new List<LevelData>();
            foreach(LevelData level in completeRequired)
            {
                if(level.completed == false)
                {
                    res.Add(level);
                }
            }
            return res.ToArray();
        }
    }

    void OnLoadState()
    {
        Debug.Log("Load LevelData in SubState");

        completed = StateManager.instance.GetBool(StringUtils.KeyFriendlyString(level + " complete"));
        Debug.Log(level + " complete : " + completed);

        StateManager.instance.SetInt(StringUtils.KeyFriendlyString(level + " difficulty"), (int)difficulty);
    }

    void OnUnloadState()
    {
        Debug.Log("Unload LevelData in SubState");
    }
}
