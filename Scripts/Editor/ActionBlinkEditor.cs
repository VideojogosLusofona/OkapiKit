using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ActionBlink))]
public class ActionBlinkEditor : ActionEditor
{
    SerializedProperty propTarget;
    SerializedProperty propIncludeChildren;
    SerializedProperty propBlinkTimeOn;
    SerializedProperty propBlinkTimeOff;
    SerializedProperty propDuration;

    protected override void OnEnable()
    {
        base.OnEnable();

        propTarget = serializedObject.FindProperty("target");
        propIncludeChildren = serializedObject.FindProperty("includeChildren");
        propBlinkTimeOn = serializedObject.FindProperty("blinkTimeOn");
        propBlinkTimeOff = serializedObject.FindProperty("blinkTimeOff");
        propDuration = serializedObject.FindProperty("duration");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (WriteTitle())
        {
            StdEditor(false);

            var action = (target as ActionBlink);
            if (action == null) return;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(propTarget, new GUIContent("Target"));
            EditorGUILayout.PropertyField(propIncludeChildren, new GUIContent("Include Children?"));
            EditorGUILayout.PropertyField(propBlinkTimeOn, new GUIContent("On Duration"));
            EditorGUILayout.PropertyField(propBlinkTimeOff, new GUIContent("Off Duration"));
            EditorGUILayout.PropertyField(propDuration, new GUIContent("Duration"));

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                (target as Action).UpdateExplanation();
            }
        }
    }
}
