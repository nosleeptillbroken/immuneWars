using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEditor;

[@CustomEditor(typeof(CreepSpawner))]
public class CreepSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {        
        base.OnInspectorGUI();
    }

    public void OnSceneGUI()
    {
        Handles.color = Color.blue;

        CreepTarget creepTarget = (CreepTarget)target;

        DrawAllParents(creepTarget);

        DrawAllChildren(creepTarget);

    }

    void DrawAllParents(CreepTarget creepTarget)
    {
        CreepTarget current = creepTarget;
        CreepTarget parent = creepTarget.parent;
        while(current != null && parent != null)
        {
            Handles.DrawLine(current.transform.position, parent.transform.position);
            current = parent;
            parent = parent.parent;
        }
    }

    void DrawAllChildren(CreepTarget creepTarget)
    {
        if (creepTarget.children.Length > 0)
        {
            foreach (CreepTarget child in creepTarget.children)
            {
                Handles.DrawLine(creepTarget.transform.position, child.transform.position);
                DrawAllChildren(child);
            }
        }
    }

}

[CustomEditor(typeof(CreepTarget))]
public class CreepTargetEditor : CreepSpawnerEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        // Additional code for the derived class...
    }

    public void OnSceneGUI()
    {
        base.OnSceneGUI();
    }
}
