using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(Path))]
    public class PathEditor : OkapiBaseEditor
    {
        SerializedProperty propType;
        SerializedProperty propClosed;
        SerializedProperty propSides;
        SerializedProperty propPoints;
        SerializedProperty propWorldSpace;
        SerializedProperty propEditMode;
        SerializedProperty propOnlyDisplayWhenSelected;
        SerializedProperty propDisplayColor;

        int editPoint = -1;
        Tool lastTool = Tool.None;

        protected override void OnEnable()
        {
            base.OnEnable();

            propType = serializedObject.FindProperty("type");
            propClosed = serializedObject.FindProperty("closed");
            propSides = serializedObject.FindProperty("nSides");
            propPoints = serializedObject.FindProperty("points");
            propWorldSpace = serializedObject.FindProperty("worldSpace");
            propEditMode = serializedObject.FindProperty("editMode");
            propOnlyDisplayWhenSelected = serializedObject.FindProperty("onlyDisplayWhenSelected");
            propDisplayColor = serializedObject.FindProperty("displayColor");

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
                var t = (target as Path);

                EditorGUI.BeginChangeCheck();

                var type = (Path.Type)propType.intValue;

                EditorGUILayout.PropertyField(propType, new GUIContent("Type", "Type of path.\nLinear: Straight lines between points\nSmooth: Curved line that passes through some points and is influenced by the others.\nCircle: The first point defines the center, the second the radius of the circle. If there is a third point, it defines the radius in that approximate direction.\nArc: First point defines the center, the second and third define the beginning and end of an arc centered on the first point.\nPolygon: First point define the center, the second and third point define the radius in different directions, while the 'Sides' property defines the number of sides of the polygon."));
                if ((type != Path.Type.Circle) && (type != Path.Type.Arc))
                {
                    EditorGUILayout.PropertyField(propClosed, new GUIContent("Closed", "If the path should end where it starts."));
                }
                if (type == Path.Type.Polygon)
                {
                    EditorGUILayout.PropertyField(propSides, new GUIContent("Sides", "Number of sides in the polygon."));
                }
                EditorGUILayout.PropertyField(propPoints, new GUIContent("Points", "Waypoints"));
                EditorGUILayout.PropertyField(propWorldSpace, new GUIContent("World Space", "Are the positions in world space, or relative to this object."));
                EditorGUILayout.PropertyField(propEditMode, new GUIContent("Edit Mode", "If edit mode is on, you can edit the points the scene view.\nClick on a point to select it, use the gizmo to move them around."));
                EditorGUILayout.PropertyField(propOnlyDisplayWhenSelected, new GUIContent("Only display when selected", "If on, it will only display the path when the object is selected, otherwise it will show the object with the selected color."));
                EditorGUILayout.PropertyField(propDisplayColor, new GUIContent("Display Color", "What color should the path be rendered when not being edited"));

                EditorGUILayout.PropertyField(propDescription, new GUIContent("Description", "This is for you to leave a comment for yourself or others."));

                bool prevEnabled = GUI.enabled;
                GUI.enabled = (propPoints.arraySize < 3) || ((type != Path.Type.Circle) && (type != Path.Type.Arc) && (type != Path.Type.Polygon));

                if (type == Path.Type.Smooth)
                {
                    if (GUILayout.Button("Add Segment"))
                    {
                        t.AddPoint();
                        // Change to edit mode
                        propEditMode.boolValue = true;

                        EditorUtility.SetDirty(target);
                    }
                }
                else
                {
                    if (GUILayout.Button("Add Point"))
                    {
                        t.AddPoint();
                        // Change to edit mode
                        propEditMode.boolValue = true;

                        EditorUtility.SetDirty(target);
                    }
                }
                GUI.enabled = prevEnabled;

                if ((type == Path.Type.Linear) || (type == Path.Type.Smooth) || (type == Path.Type.Arc))
                {
                    if (GUILayout.Button("Invert Path"))
                    {
                        Undo.RecordObject(target, "Invert path");
                        t.InvertPath();

                        SceneView.RepaintAll();
                    }
                }
                if (GUILayout.Button("Center Path"))
                {
                    Undo.RecordObject(target, "Center path");
                    t.CenterPath();

                    SceneView.RepaintAll();
                }

                EditorGUI.EndChangeCheck();

                serializedObject.ApplyModifiedProperties();
                (target as OkapiElement).UpdateExplanation();
            }
        }

        public void OnSceneGUI()
        {
            var t = (target as Path);

            bool    localSpace = !t.isWorldSpace;
            var     type = (Path.Type)propType.intValue;
            
            if (t.isEditMode)
            {
                List<Vector3> newPoints = t.GetEditPoints();

                bool manualDirty = false;

                EditorGUI.BeginChangeCheck();

                Event e = Event.current;
                if (e.type == EventType.KeyDown)
                {
                    if (e.keyCode == KeyCode.Delete)
                    {
                        // Del pressed, check if a point is selected
                        if (editPoint >= 0)
                        {
                            // Remove this point
                            newPoints.RemoveAt(editPoint);
                            editPoint = -1;
                            manualDirty = true;

                            e.Use();
                        }
                    }
                }

                // Disable transform gizmo tool
                if (Tools.current != Tool.None) lastTool = Tools.current;
                Tools.current = Tool.None;

                if (localSpace)
                {
                    var worldMatrix = t.transform.localToWorldMatrix;
                    for (int i = 0; i < newPoints.Count; i++) newPoints[i] = worldMatrix * new Vector4(newPoints[i].x, newPoints[i].y, newPoints[i].z, 1);
                }

                for (int i = 0; i < newPoints.Count; i++)
                {
                    float s = (editPoint == i) ? (10.0f) : (5.0f);
                    bool selectable = true;

                    // Render text
                    string text = "";
                    if ((type == Path.Type.Linear) || (type == Path.Type.Smooth))
                    {
                        text = $"{i}";
                    }
                    else if ((type == Path.Type.Circle) || (type == Path.Type.Polygon))
                    {
                        if (i == 0) text = "Center";
                        else if (i == 1) text = "Primary Axis";
                        else if (i == 2) text = "Secondary Axis";
                        else selectable = false;
                    }
                    else if (type == Path.Type.Arc) 
                    {
                        if (i == 0) text = "Center";
                        else if (i == 1) text = "Start";
                        else if (i == 2) text = "End";
                        else selectable = false;
                    }

                    if (selectable)
                    {
                        if (Handles.Button(newPoints[i], Quaternion.identity, s, s, Handles.CircleHandleCap))
                        {
                            editPoint = i;
                        }
                    }
                    if (text != "")
                    {
                        Handles.Label(newPoints[i] + Vector3.right * s * 1.25f, text);
                    }
                }

                if ((editPoint >= 0) && (editPoint < newPoints.Count))
                {
                    newPoints[editPoint] = Handles.PositionHandle(newPoints[editPoint], Quaternion.identity);

                    if ((type == Path.Type.Circle) || (type == Path.Type.Polygon))
                    {
                        if (newPoints.Count >= 2)
                        {
                            // Project this point into the perpendicular and use that instead
                            Vector3 delta = (newPoints[1] - newPoints[0]).normalized;
                            (delta.x, delta.y) = (delta.y, -delta.x);

                            delta.Normalize();

                            if (newPoints.Count >= 3)
                            {
                                float r = Vector3.Dot(delta, (newPoints[2] - newPoints[1]));
                                newPoints[2] = newPoints[0] + r * delta;
                            }
                        }
                    }
                }

                if (localSpace)
                {
                    var worldMatrix = t.transform.worldToLocalMatrix;
                    for (int i = 0; i < newPoints.Count; i++) newPoints[i] = worldMatrix * new Vector4(newPoints[i].x, newPoints[i].y, newPoints[i].z, 1.0f);
                }

                if ((EditorGUI.EndChangeCheck()) || (manualDirty))
                {
                    Undo.RecordObject(target, "Move point");
                    t.SetEditPoints(newPoints);
                }

                DrawPath(type, t.GetPoints(), Color.white);

                var prevMatrix = Handles.matrix;
                if (!t.isWorldSpace)
                {
                    Handles.matrix = t.transform.localToWorldMatrix;
                }

                var editPoints = t.GetEditPoints();
                if ((type == Path.Type.Smooth) && (!propClosed.boolValue))
                {
                    if (editPoints.Count > 2)
                    {
                        // Draw last segment
                        Vector3 p1 = editPoints[editPoints.Count - 1];
                        Vector3 p2 = editPoints[editPoints.Count - 2];
                        Vector3 d = p2 - p1;

                        float len = 0.1f;
                        Handles.color = Color.grey;
                        for (int i = 0; i < 1.0f / len; i++)
                        {
                            Handles.DrawLine(p1 + d * i * len, p1 + d * (i + 0.5f) * len, 0.5f);
                        }
                    }
                }

                if (((type == Path.Type.Circle) || (type == Path.Type.Polygon) || (type == Path.Type.Polygon)) && (editPoints.Count > 0))
                {
                    Vector3 center = editPoints[0];
                    Vector3 upBound = t.upAxis * t.upExtent;
                    Vector3 rightBound = t.rightAxis * t.rightExtent;

                    Handles.color = Color.yellow;
                    Handles.DrawPolyLine(new Vector3[] { center - rightBound - upBound, center + rightBound - upBound, center + rightBound + upBound, center - rightBound + upBound, center - rightBound - upBound });
                }

                Handles.matrix = prevMatrix;
            }
            else
            {
                if (lastTool != Tool.None) Tools.current = lastTool;
                lastTool = Tool.None;

                DrawPath(type, t.GetPoints(), propDisplayColor.colorValue);
            }
        }

        void DrawPath(Path.Type type, List<Vector3> points, Color color)
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
            if (propType.intValue == (int)Path.Type.Linear)
                return GUIUtils.GetTexture("PathStraight");
            else
                return GUIUtils.GetTexture("PathCurved");
        }

        protected override (Color, Color, Color) GetColors() => (GUIUtils.ColorFromHex("#ffcaca"), GUIUtils.ColorFromHex("#2f4858"), GUIUtils.ColorFromHex("#ff6060"));
    }
}