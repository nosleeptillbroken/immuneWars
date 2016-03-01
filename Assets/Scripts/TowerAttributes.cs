using UnityEngine;

[System.Serializable]
public class TowerAttributes : MonoBehaviour
{
    [Header("General")]

    /// <summary>
    /// The name of the tower / upgrade in-game.
    /// </summary>
    public string displayName = "Tower Attribute";

    /// <summary>
    /// The cost to purchase or upgrade this tower.
    /// </summary>
    public int cost = 0;

    /// <summary>
    /// The tower's targeting range.
    /// </summary>
    public float range = 1.0f;

    /// <summary>
    /// The damage each shot inflicts.
    /// </summary>
    public int damage = 1;

    /// <summary>
    /// Number of shots the tower fires per second.
    /// </summary>
    public float rateOfFire = 1.0f;

    /// <summary>
    /// Speed of a missile shot from this tower.
    /// </summary>
    public float missileSpeed = 1.0f;
    
    [Header("Creep Slowing")]

    /// <summary>
    /// Whether or not the tower's shots apply a slowing effect.
    /// </summary>
    public bool applySlow = false;

    /// <summary>
    /// How many seconds to slow the target for.
    /// </summary>
    public float slowTime = 1.0f;

    /// <summary>
    /// How much to slow the target's speed by.
    /// </summary>
    public float slowFactor = 0.5f;

    [Header("Creep Burn")]

    /// <summary>
    /// Whether or not the tower's shots apply residual damage over time.
    /// </summary>
    public bool applyBurn = false;

    /// <summary>
    /// Time between target burns, in seconds.
    /// </summary>
    public float burnTime = 1.0f;

    /// <summary>
    /// Number of burns to inflict on the target.
    /// </summary>
    public int burnCount = 1;

    /// <summary>
    /// Damage to inflict on the target each burn.
    /// </summary>
    public int burnDamage = 1;
    
}
