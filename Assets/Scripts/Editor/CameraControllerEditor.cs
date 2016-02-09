using UnityEngine;
using UnityEditor;
using System.Collections;

[@CustomEditor(typeof(CameraController))]
public class CameraControllerEditor : Editor
{

    void OnSceneGUI()
    {
        CameraController controller = (CameraController)target;

        if (controller.cameraBoundsRestriction)
        {

            Vector3 min = controller.minimumBounds = Handles.PositionHandle(controller.minimumBounds, Quaternion.identity);
            Vector3 max = controller.maximumBounds = Handles.PositionHandle(controller.maximumBounds, Quaternion.identity);

            Handles.color = Color.red;

            Handles.DrawLine(new Vector3(min.x, min.y, min.z), new Vector3(max.x, min.y, min.z));
            Handles.DrawLine(new Vector3(min.x, min.y, min.z), new Vector3(min.x, min.y, max.z));
            Handles.DrawLine(new Vector3(max.x, min.y, min.z), new Vector3(max.x, min.y, max.z));
            Handles.DrawLine(new Vector3(min.x, min.y, max.z), new Vector3(max.x, min.y, max.z));

            Handles.DrawLine(new Vector3(min.x, min.y, min.z), new Vector3(min.x, max.y, min.z));
            Handles.DrawLine(new Vector3(max.x, min.y, max.z), new Vector3(max.x, max.y, max.z));
            Handles.DrawLine(new Vector3(max.x, min.y, min.z), new Vector3(max.x, max.y, min.z));
            Handles.DrawLine(new Vector3(min.x, min.y, max.z), new Vector3(min.x, max.y, max.z));

            Handles.DrawLine(new Vector3(min.x, max.y, min.z), new Vector3(max.x, max.y, min.z));
            Handles.DrawLine(new Vector3(min.x, max.y, min.z), new Vector3(min.x, max.y, max.z));
            Handles.DrawLine(new Vector3(max.x, max.y, min.z), new Vector3(max.x, max.y, max.z));
            Handles.DrawLine(new Vector3(min.x, max.y, max.z), new Vector3(max.x, max.y, max.z));
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }

    }

}
