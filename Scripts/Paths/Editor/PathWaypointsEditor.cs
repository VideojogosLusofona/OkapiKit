using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Path))]
public class PathWaypointsEditor : Editor
{
    int                 editPoint = -1;
    Tool                lastTool = Tool.None;

    void OnEnable()
    {
        var t = (target as Path);

        lastTool = Tools.current;
    }
    void OnDisable()
    {
        Tools.current = lastTool;
    }

    public override void OnInspectorGUI()
    {
        //serializedObject.Update();

        //bool b = propEditMode.boolValue;
        //propEditMode.boolValue = EditorGUILayout.Toggle("EDIT MODE", b, "Button");

        base.OnInspectorGUI();

        if (GUILayout.Button("Add Point"))
        {
            var t = (target as Path);
            t.AddPoint();

            EditorUtility.SetDirty(target);
        }

        //EditorGUILayout.PropertyField(lookAtPoint);

        //serializedObject.ApplyModifiedProperties();

        //Debug.Log(propEditMode.boolValue);
    }

    public void OnSceneGUI()
    {
        var t = (target as Path);

        bool localSpace = !t.isWorldSpace;

        if (t.isEditMode)
        {
            // Disable transform gizmo tool
            if (Tools.current != Tool.None) lastTool = Tools.current;
            Tools.current = Tool.None;

            List<Vector3> newPoints = t.GetEditPoints();
            if (localSpace)
            {
                var worldMatrix = t.transform.localToWorldMatrix;
                for (int i = 0; i < newPoints.Count; i++) newPoints[i] = worldMatrix * new Vector4(newPoints[i].x, newPoints[i].y, newPoints[i].z, 1);
            }

            EditorGUI.BeginChangeCheck();
            for (int i = 0; i < newPoints.Count; i++)
            {
                float s = (editPoint == i) ? (10.0f) : (5.0f);

                if (Handles.Button(newPoints[i], Quaternion.identity, s, s, Handles.CircleHandleCap))
                {
                    editPoint = i;
                }
            }

            if ((editPoint >= 0) && (editPoint < newPoints.Count))
            {
                newPoints[editPoint] = Handles.PositionHandle(newPoints[editPoint], Quaternion.identity);
            }

            if (localSpace)
            {
                var worldMatrix = t.transform.worldToLocalMatrix;
                for (int i = 0; i < newPoints.Count; i++) newPoints[i] = worldMatrix * new Vector4(newPoints[i].x, newPoints[i].y, newPoints[i].z, 1.0f);
            }

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Move point");
                t.SetEditPoints(newPoints);
            }

            var prevMatrix = Handles.matrix;
            DrawPath(t.GetPoints(), Color.white);
        }
        else
        {
            if (lastTool != Tool.None) Tools.current = lastTool;
            lastTool = Tool.None;

            var prevMatrix = Handles.matrix;
            DrawPath(t.GetPoints(), Color.yellow);
        }
    }

    void DrawPath(List<Vector3> points, Color color)
    {
        if (points == null) return;

        Handles.color = color;       
        
        for (int i = 1; i < points.Count; i++) 
        {
            Handles.DrawLine(points[i - 1], points[i], 1.0f);
        }
    }
}
