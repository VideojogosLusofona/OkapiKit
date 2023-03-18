using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MovementPath))]
public class MovementPathEditor : MovementEditor
{
    SerializedProperty propSpeed;
    SerializedProperty propPath;
    SerializedProperty propTaggedPath;
    SerializedProperty propLoop;
    SerializedProperty propRelativePath;
    SerializedProperty propRotationBehaviour;
    SerializedProperty propUseFlip;
    SerializedProperty propHasMaxRotationSpeed;
    SerializedProperty propMaxRotationSpeed;

    protected override void OnEnable()
    {
        base.OnEnable();

        propSpeed = serializedObject.FindProperty("speed");
        propPath = serializedObject.FindProperty("path");
        propTaggedPath = serializedObject.FindProperty("taggedPath");
        propLoop = serializedObject.FindProperty("loop");
        propRelativePath = serializedObject.FindProperty("relativePath");
        propRotationBehaviour = serializedObject.FindProperty("rotationBehaviour");
        propUseFlip = serializedObject.FindProperty("useFlip");
        propHasMaxRotationSpeed = serializedObject.FindProperty("hasMaxRotationSpeed");
        propMaxRotationSpeed = serializedObject.FindProperty("maxRotationSpeed");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (WriteTitle())
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(propSpeed, new GUIContent("Speed"));
            if (propPath.objectReferenceValue == null)
            {
                if (propTaggedPath.objectReferenceValue == null)
                {
                    EditorGUILayout.PropertyField(propPath, new GUIContent("Path"));
                    EditorGUILayout.PropertyField(propTaggedPath, new GUIContent("Path Tag"));
                }
                else
                {
                    EditorGUILayout.PropertyField(propTaggedPath, new GUIContent("Path Tag"));
                }
            }
            else
            {
                EditorGUILayout.PropertyField(propPath, new GUIContent("Path"));
            }
            EditorGUILayout.PropertyField(propLoop, new GUIContent("Loop"));
            EditorGUILayout.PropertyField(propRelativePath, new GUIContent("Is Path Relative?"));
            EditorGUILayout.PropertyField(propRotationBehaviour, new GUIContent("Rotation Behaviour"));
            if (propRotationBehaviour.enumValueIndex != (int)MovementPath.RotationBehaviour.None)
            {
                EditorGUILayout.PropertyField(propUseFlip, new GUIContent("Use flip?"));
                EditorGUILayout.PropertyField(propHasMaxRotationSpeed, new GUIContent("Has maximum rotation speed?"));
                if (propHasMaxRotationSpeed.boolValue)
                {
                    EditorGUILayout.PropertyField(propMaxRotationSpeed, new GUIContent("Maximum rotation speed?"));
                }
            }

            EditorGUILayout.PropertyField(propDescription, new GUIContent("Description"));

            EditorGUI.EndChangeCheck();

            serializedObject.ApplyModifiedProperties();
            (target as OkapiElement).UpdateExplanation();

            StdEditor(false);
        }
    }

}
