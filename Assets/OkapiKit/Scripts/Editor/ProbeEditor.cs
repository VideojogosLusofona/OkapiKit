using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Probe))]
public class ProbeEditor : OkapiBaseEditor
{
    SerializedProperty propType;
    SerializedProperty propDirection;
    SerializedProperty propRadius;
    SerializedProperty propMinDistance;
    SerializedProperty propMaxDistance;
    SerializedProperty propTargetObject;
    SerializedProperty propTargetTag;
    SerializedProperty propTags;
    SerializedProperty propTargetTransform;

    protected override void OnEnable()
    {
        base.OnEnable();

        propType = serializedObject.FindProperty("type");
        propDirection = serializedObject.FindProperty("direction");
        propRadius = serializedObject.FindProperty("radius");
        propMinDistance = serializedObject.FindProperty("minDistance");
        propMaxDistance = serializedObject.FindProperty("maxDistance");
        propTargetObject = serializedObject.FindProperty("dirTransform");
        propTargetTag = serializedObject.FindProperty("dirTag");
        propTags = serializedObject.FindProperty("tags");
        propTargetTransform = serializedObject.FindProperty("targetTransform");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (WriteTitle())
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(propType, new GUIContent("Type"));
            if (propType.enumValueIndex == (int)Probe.Type.Circlecast)
            {
                EditorGUILayout.PropertyField(propRadius, new GUIContent("Radius"));
            }
            EditorGUILayout.PropertyField(propDirection, new GUIContent("Direction"));
            Probe.Direction direction = (Probe.Direction)propDirection.enumValueIndex;

            switch (direction)
            {
                case Probe.Direction.Up:
                case Probe.Direction.Down:
                case Probe.Direction.Right:
                case Probe.Direction.Left:
                    EditorGUILayout.PropertyField(propMinDistance, new GUIContent("Min Distance"));
                    EditorGUILayout.PropertyField(propMaxDistance, new GUIContent("Max Distance"));
                    break;
                case Probe.Direction.TargetObject:
                    EditorGUILayout.PropertyField(propTargetObject, new GUIContent("Target Object"));
                    EditorGUILayout.PropertyField(propMinDistance, new GUIContent("Min Distance"));
                    break;
                case Probe.Direction.TargetTag:
                    EditorGUILayout.PropertyField(propTargetTag, new GUIContent("Target Tag"));
                    EditorGUILayout.PropertyField(propMinDistance, new GUIContent("Min Distance"));
                    break;
                default:
                    break;
            }
            EditorGUILayout.PropertyField(propTags, new GUIContent("Tags"));
            EditorGUILayout.PropertyField(propTargetTransform, new GUIContent("Target Transform"));

            EditorGUILayout.PropertyField(propDescription, new GUIContent("Description"));

            EditorGUI.EndChangeCheck();

            serializedObject.ApplyModifiedProperties();
            (target as OkapiElement).UpdateExplanation();
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
        return "Probe";
    }

    protected override Texture2D GetIcon()
    {
        return GUIUtils.GetTexture("Probe");
    }

    protected override (Color, Color, Color) GetColors() => (GUIUtils.ColorFromHex("#d1d283"), GUIUtils.ColorFromHex("#2f4858"), GUIUtils.ColorFromHex("#969721"));
}
