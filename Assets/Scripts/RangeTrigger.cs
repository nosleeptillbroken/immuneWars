// RangeTrigger.cs
// Notifies the tower if an enemy enters the range volume.

using UnityEngine;
using System.Collections;

public class RangeTrigger : MonoBehaviour
{
    [HideInInspector]
    public Tower parent;
	
	// Initializes parent object
	void Start()
    {
        parent = transform.parent.gameObject.GetComponent<Tower>();
    }

	// Calls parent's OnRangeEnter if parent still exists
    void OnTriggerEnter(Collider other)
    {
        if (parent)
        {
            parent.OnRangeEnter(other);
        }
    }

	// Calls parent's OnRangeExit if parent still exists
    void OnTriggerExit(Collider other)
    {
        if (parent)
        {
            parent.OnRangeExit(other);
        }
    }

}
