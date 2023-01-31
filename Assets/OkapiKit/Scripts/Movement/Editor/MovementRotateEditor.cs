using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MovementRotate))]
public class MovementRotateEditor : MovementEditor
{
    SerializedProperty propSpeed;
    SerializedProperty propInputEnabled;
    SerializedProperty propInputType;
    SerializedProperty propRotationAxis;
    SerializedProperty propRotationButtonPositive;
    SerializedProperty propRotationButtonNegative;
    SerializedProperty propRotationKeyPositive;
    SerializedProperty propRotationKeyNegative;

    protected override void OnEnable()
    {
        base.OnEnable();

        propSpeed = serializedObject.FindProperty("speed");
        propInputEnabled = serializedObject.FindProperty("inputEnabled");
        propInputType = serializedObject.FindProperty("inputType");
        propRotationAxis = serializedObject.FindProperty("rotationAxis");
        propRotationButtonPositive = serializedObject.FindProperty("rotationButtonPositive");
        propRotationButtonNegative = serializedObject.FindProperty("rotationButtonNegative");
        propRotationKeyPositive = serializedObject.FindProperty("rotationKeyPositive");
        propRotationKeyNegative = serializedObject.FindProperty("rotationKeyNegative");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (WriteTitle())
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(propSpeed, new GUIContent("Speed"));
            EditorGUILayout.PropertyField(propInputEnabled, new GUIContent("Use Input?"));
            if (propInputEnabled.boolValue)
            {
                EditorGUILayout.PropertyField(propInputType, new GUIContent("Input Type"));

                var inputType = (MovementXY.InputType)propInputType.enumValueIndex;

                switch (inputType)
                {
                    case MovementXY.InputType.Axis:
                        EditorGUILayout.PropertyField(propRotationAxis, new GUIContent("Rotation Axis"));
                        break;
                    case MovementXY.InputType.Button:
                        EditorGUILayout.PropertyField(propRotationButtonPositive, new GUIContent("Counter-Clockwise Button"));
                        EditorGUILayout.PropertyField(propRotationButtonNegative, new GUIContent("Clockwise Button"));
                        break;
                    case MovementXY.InputType.Key:
                        EditorGUILayout.PropertyField(propRotationKeyPositive, new GUIContent("Counter-Clockwise Key"));
                        EditorGUILayout.PropertyField(propRotationKeyNegative, new GUIContent("Clockwise Key"));
                        break;
                    default:
                        break;
                }
            }
            EditorGUILayout.PropertyField(propDescription, new GUIContent("Description"));

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }

            StdEditor(false);
        }
    }

}
