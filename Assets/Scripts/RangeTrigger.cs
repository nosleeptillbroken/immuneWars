using UnityEngine;
using System.Collections;

public class RangeTrigger : MonoBehaviour
{
    [HideInInspector]
    public Tower parent;

	void Start()
    {
        parent = transform.parent.gameObject.GetComponent<Tower>();
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
