using UnityEngine;

[System.Serializable]
public class TowerAttributes
{
    [Header("General")]

    /// <summary>
    /// The name of the tower / upgrade in-game.
    /// </summary>
    public string displayName = null;

    /// <summary>
    /// Description for the tower in the shop menu
    /// </summary>
    public string description = null;

    /// <summary>
    /// The cost to purchase or upgrade this tower.
    /// </summary>
    public int cost = 1;

    /// <summary>
    /// The tower's targeting range.
    /// </summary>
    public float range = 5.0f;

    /// <summary>
    /// The damage each shot inflicts.
    /// </summary>
    public int damage = 5;

    /// <summary>
    /// Number of shots the tower fires per second.
    /// </summary>
    public float rateOfFire = 1.0f;

    /// <summary>
    /// Speed of a missile shot from this tower.
    /// </summary>
    public float missileSpeed = 20.0f;
    
    [Header("Creep Slowing")]

    /// <summary>
    /// Whether or not the tower's shots apply a slowing effect.
    /// </summary>
    public bool applySlow = false;

    /// <summary>
    /// How many seconds to slow the target for.
    /// </summary>
    public float slowTime = 0.0f;

    /// <summary>
    /// How much to slow the target's speed by.
    /// </summary>
    public float slowFactor = 0.0f;

    [Header("Creep Burn")]

    /// <summary>
    /// Whether or not the tower's shots apply residual damage over time.
    /// </summary>
    public bool applyBurn = false;

    /// <summary>
    /// Time between target burns, in seconds.
    /// </summary>
    public float burnTime = 0.0f;

    /// <summary>
    /// Number of burns to inflict on the target.
    /// </summary>
    public int burnCount = 0;

    /// <summary>
    /// Damage to inflict on the target each burn.
    /// </summary>
    public int burnDamage = 0;

    [Header("AoE Damage")]

    /// <summary>
    /// Whether or not the tower's shots apply area of effect damage.
    /// </summary>
    public bool applyAOE = false;

    /// <summary>
    /// How much area of effect damage is applied.
    /// </summary>
    public int AOEDamage = 0;

    [Header("Display")]

    public Mesh mesh = null;
    public Material material = null;
    
    // add two sets of attributes together
    public static TowerAttributes operator +(TowerAttributes lhs, TowerAttributes rhs)
    {
        TowerAttributes ret = new TowerAttributes();
        
        ret.displayName = rhs.displayName + lhs.displayName;
        ret.cost = lhs.cost + rhs.cost;
        ret.range = lhs.range + rhs.range;
        ret.damage = lhs.damage + rhs.damage;
        ret.rateOfFire = lhs.rateOfFire + rhs.rateOfFire;
        ret.missileSpeed = lhs.missileSpeed + rhs.missileSpeed;
        ret.applySlow = lhs.applySlow || rhs.applySlow;
        ret.slowTime = lhs.slowTime + rhs.slowTime;
        ret.slowFactor = lhs.slowFactor + rhs.slowFactor;
        ret.applyBurn = lhs.applyBurn || rhs.applyBurn;
        ret.burnTime = lhs.burnTime + rhs.burnTime;
        ret.burnCount = lhs.burnCount + rhs.burnCount;
        ret.burnDamage = lhs.burnDamage + rhs.burnDamage;

        return ret;
    }

    static string GetColoredRichTextForOutcome(string text, int outcome, bool addNewline = true)
    {
        string str = "";

        if(outcome < 0)
        {
            str += "<color=red>" + text + " -</color>";
        }
        else if(outcome > 0)
        {
            str += "<color=green>" + text + " +</color>";
        }
        else
        {
            return "";
        }

        if(addNewline)
        {
            str += "\n";
        }

        return str;
    }

    public static string GetUpgradeTooltip(TowerAttributes lhs, TowerAttributes rhs)
    {
        string str = "";

        str += GetColoredRichTextForOutcome("Range",lhs.range.CompareTo(rhs.range));
        str += GetColoredRichTextForOutcome("Damage", lhs.damage.CompareTo(rhs.damage));
        str += GetColoredRichTextForOutcome("Rate of Fire", lhs.rateOfFire.CompareTo(rhs.rateOfFire), false);

        return str;
    }

}