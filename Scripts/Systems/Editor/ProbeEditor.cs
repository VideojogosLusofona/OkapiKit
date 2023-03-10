using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Probe))]
public class ProbeEditor : OkapiBaseEditor
{
    SerializedProperty propType;
    SerializedProperty propRadius;
    SerializedProperty propMinDistance;
    SerializedProperty propMaxDistance;
    SerializedProperty propTags;
    SerializedProperty propTargetTransform;

    protected override void OnEnable()
    {
        base.OnEnable();

        propType = serializedObject.FindProperty("type");
        propRadius = serializedObject.FindProperty("radius");
        propMinDistance = serializedObject.FindProperty("minDistance");
        propMaxDistance = serializedObject.FindProperty("maxDistance");
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
            EditorGUILayout.PropertyField(propMinDistance, new GUIContent("Min Distance"));
            EditorGUILayout.PropertyField(propMaxDistance, new GUIContent("Max Distance"));
            EditorGUILayout.PropertyField(propTags, new GUIContent("Tags"));
            EditorGUILayout.PropertyField(propTargetTransform, new GUIContent("Target Transform"));

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
