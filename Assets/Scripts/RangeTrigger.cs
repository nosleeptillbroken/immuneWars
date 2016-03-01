// RangeTrigger.cs
// Notifies the tower if an enemy enters the range volume.

using UnityEngine;
using System.Collections;

public class RangeTrigger : MonoBehaviour
{
    [HideInInspector]
    public TowerBehaviour parent;

	void Start()
    {
        parent = transform.parent.gameObject.GetComponent<TowerBehaviour>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (parent)
        {
            parent.OnRangeEnter(other);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (parent)
        {
            parent.OnRangeExit(other);
        }
    }

}
