// CreepGoal.cs
// Defines any behaviour to be perfomed when an enemy successfully enters the creep goal.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Collider))]
public class CreepTarget : MonoBehaviour
{
    /// <summary>
    /// How much of a threat this target poses to creeps.
    /// </summary>
    public int dangerValue { get { return _dangerValue; } }
    [SerializeField] private int _dangerValue = 0;

    /// <summary>
    /// Paths with a danger value within the tolerance value will be selected at random.
    /// </summary>
    public static int dangerTolerance = 1;

    public CreepTarget[] children { get { return _children.ToArray(); } }
    [SerializeField] private List<CreepTarget> _children = new List<CreepTarget>();

    void Awake()
    {
        foreach (Transform child in transform)
        {
            CreepTarget childTarget = child.GetComponent<CreepTarget>();
            if (childTarget != null)
            {
                _children.Add(childTarget);
            }
        }
    }

    public CreepTarget parent
    {
        get
        {
            return (transform.parent != null) ? transform.parent.GetComponent<CreepTarget>() : null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Creep creep = other.GetComponent<Creep>();

            if(creep != null)
            {        
                if (children.Length == 0 )
                {
                    other.SendMessage("OnDespawn", null, SendMessageOptions.DontRequireReceiver);
                }
                else
                {
                    other.SendMessage("OnTargetNode", null, SendMessageOptions.DontRequireReceiver);
                    DirectToNextNode(creep);
                }
            }
        }
        else if(other.CompareTag("Tower"))
        {
            Debug.Log("Danger Value +");
            _dangerValue += 1;
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Tower"))
        {
            Debug.Log("Danger Value -");
            _dangerValue -= 1;
        }
    }

    public void DirectToNextNode(Creep creep)
    {

        int nextTargetIndex = GetNextNodeIndex();

        creep.target = children[nextTargetIndex];
    }
    
    /// <summary>
    /// Sorts the children by danger value, and returns the first index that is less than (index-1).dangerValue - tolerance
    /// </summary>
    /// <returns></returns>
    public int SortChildrenByDanger()
    {
        _children.Sort((l, r) => l.dangerValue.CompareTo(r.dangerValue));
        
        for(int i = 0; i < _children.Count - 1; i++)
        {
            if(_children[i].dangerValue > _children[i+1].dangerValue + dangerTolerance)
            {
                return i + 1;
            }
        }
        return _children.Count;
    }

    public int GetNextNodeIndex()
    {
        int topValuesWithinTolerance = SortChildrenByDanger();

        return Random.Range(0, topValuesWithinTolerance);
    }

}
