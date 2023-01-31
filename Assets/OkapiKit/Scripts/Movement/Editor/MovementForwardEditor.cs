using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MovementForward))]
public class MovementForwardEditor : MovementEditor
{
    SerializedProperty propSpeed;
    SerializedProperty propAxis;

    protected override void OnEnable()
    {
        base.OnEnable();

        propSpeed = serializedObject.FindProperty("speed");
        propAxis = serializedObject.FindProperty("axis");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (WriteTitle())
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(propSpeed, new GUIContent("Speed"));
            EditorGUILayout.PropertyField(propAxis, new GUIContent("Axis"));

            EditorGUILayout.PropertyField(propDescription, new GUIContent("Description"));

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }

            StdEditor(false);
        }
    }

}
