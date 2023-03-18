using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ActionChangeActionState))]
public class ActionChangeActionStateEditor : ActionEditor
{
    SerializedProperty propTarget;
    SerializedProperty propState;

    protected override void OnEnable()
    {
        base.OnEnable();

        propTarget = serializedObject.FindProperty("target");
        propState = serializedObject.FindProperty("state");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (WriteTitle())
        {
            StdEditor(false);

            var action = (target as ActionChangeActionState);
            if (action == null) return;

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(propTarget, new GUIContent("Target"));
            EditorGUILayout.PropertyField(propState, new GUIContent("State"));

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                (target as Action).UpdateExplanation();
            }
        }
    }
}
