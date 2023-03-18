using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Path))]
public class PathWaypointsEditor : OkapiBaseEditor
{
    SerializedProperty propType;
    SerializedProperty propTension;
    SerializedProperty propPoints;
    SerializedProperty propWorldSpace;
    SerializedProperty propEditMode;

    int     editPoint = -1;
    Tool    lastTool = Tool.None;

    protected override void OnEnable()
    {
        base.OnEnable();

        propType = serializedObject.FindProperty("type");
        propTension = serializedObject.FindProperty("tension");
        propPoints = serializedObject.FindProperty("points");
        propWorldSpace = serializedObject.FindProperty("worldSpace");
        propEditMode = serializedObject.FindProperty("editMode");

        lastTool = Tools.current;
    }
    void OnDisable()
    {
        Tools.current = lastTool;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (WriteTitle())
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(propType, new GUIContent("Type"));
            if (propType.enumValueIndex == (int)Path.PathType.Smooth)
            {
                EditorGUILayout.PropertyField(propTension, new GUIContent("Tension"));
            }
            EditorGUILayout.PropertyField(propPoints, new GUIContent("Points"));
            EditorGUILayout.PropertyField(propWorldSpace, new GUIContent("World Space"));
            EditorGUILayout.PropertyField(propEditMode, new GUIContent("Edit Mode"));
            EditorGUILayout.PropertyField(propDescription, new GUIContent("Description"));

            if (GUILayout.Button("Add Point"))
            {
                var t = (target as Path);
                t.AddPoint();

                EditorUtility.SetDirty(target);
            }

            EditorGUI.EndChangeCheck();

            serializedObject.ApplyModifiedProperties();
            (target as OkapiElement).UpdateExplanation();
        }
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

            DrawPath(t.GetPoints(), Color.white);
        }
        else
        {
            if (lastTool != Tool.None) Tools.current = lastTool;
            lastTool = Tool.None;

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

    protected override GUIStyle GetTitleSyle()
    {
        return GUIUtils.GetActionTitleStyle();
    }

    protected override GUIStyle GetExplanationStyle()
    {
        return GUIUtils.GetActionExplanationStyle();
    }

    protected override string GetTitle()
    {
        return "Path";
    }

    protected override Texture2D GetIcon()
    {
        if (propType.enumValueIndex == (int)Path.PathType.Linear)
            return GUIUtils.GetTexture("PathStraight");
        else
            return GUIUtils.GetTexture("PathCurved");
    }

    protected override (Color, Color, Color) GetColors() => (GUIUtils.ColorFromHex("#ffcaca"), GUIUtils.ColorFromHex("#2f4858"), GUIUtils.ColorFromHex("#ff6060"));
}
