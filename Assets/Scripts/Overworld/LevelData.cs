using UnityEngine;
using System.Collections;

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

}
