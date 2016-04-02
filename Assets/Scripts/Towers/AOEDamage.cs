using UnityEngine;
using System.Collections;

public class AOEDamage : MonoBehaviour {
    public TowerAttributes attributes = null;

    /// <summary>
    /// This script will apply damage to all creeps that are within its collider.
    /// The collider is significantly larger than a normal bullet prefab.
    /// </summary>

    void Start()
    {
        StartCoroutine(Timeout());
    }

    /// <summary>
    /// The AoE damage, taken from the AoE Bullet's attributes, is applied to the creeps within range.
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            CreepEffect effect = other.gameObject.GetComponent<CreepEffect>();
            effect.damage += attributes.AOEDamage;
        }
    }

    /// <summary>
    /// The AoE Damager is destroyed quickly after being created.
    /// </summary>
    IEnumerator Timeout()
    {
        yield return new WaitForSeconds(5.0f);
        Destroy(gameObject);
    }
}
