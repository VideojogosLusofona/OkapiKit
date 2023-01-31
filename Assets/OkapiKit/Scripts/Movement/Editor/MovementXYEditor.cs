using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MovementXY))]
public class MovementXYEditor : MovementEditor
{
    SerializedProperty propSpeed;
    SerializedProperty propUseRotation;
    SerializedProperty propInputEnabled;
    SerializedProperty propInputType;
    SerializedProperty propHorizontalAxis;
    SerializedProperty propVerticalAxis;
    SerializedProperty propHorizontalButtonPositive;
    SerializedProperty propHorizontalButtonNegative;
    SerializedProperty propVerticalButtonPositive;
    SerializedProperty propVerticalButtonNegative;
    SerializedProperty propHorizontalKeyPositive;
    SerializedProperty propHorizontalKeyNegative;
    SerializedProperty propVerticalKeyPositive;
    SerializedProperty propVerticalKeyNegative;

    protected override void OnEnable()
    {
        base.OnEnable();

        propSpeed = serializedObject.FindProperty("speed");
        propUseRotation = serializedObject.FindProperty("useRotation");
        propInputEnabled = serializedObject.FindProperty("inputEnabled");
        propInputType = serializedObject.FindProperty("inputType");
        propHorizontalAxis = serializedObject.FindProperty("horizontalAxis");
        propVerticalAxis = serializedObject.FindProperty("verticalAxis");
        propHorizontalButtonPositive = serializedObject.FindProperty("horizontalButtonPositive");
        propHorizontalButtonNegative = serializedObject.FindProperty("horizontalButtonNegative");
        propVerticalButtonPositive = serializedObject.FindProperty("verticalButtonPositive");
        propVerticalButtonNegative = serializedObject.FindProperty("verticalButtonNegative");
        propHorizontalKeyPositive = serializedObject.FindProperty("horizontalKeyPositive");
        propHorizontalKeyNegative = serializedObject.FindProperty("horizontalKeyNegative");
        propVerticalKeyPositive = serializedObject.FindProperty("verticalKeyPositive");
        propVerticalKeyNegative = serializedObject.FindProperty("verticalKeyNegative");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (WriteTitle())
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(propSpeed, new GUIContent("Speed"));
            EditorGUILayout.PropertyField(propUseRotation, new GUIContent("Use Rotation?"));
            EditorGUILayout.PropertyField(propInputEnabled, new GUIContent("Use Input?"));
            if (propInputEnabled.boolValue)
            {
                EditorGUILayout.PropertyField(propInputType, new GUIContent("Input Type"));

                var inputType = (MovementXY.InputType)propInputType.enumValueIndex;

                switch (inputType)
                {
                    case MovementXY.InputType.Axis:
                        EditorGUILayout.PropertyField(propHorizontalAxis, new GUIContent("Horizontal Axis"));
                        EditorGUILayout.PropertyField(propVerticalAxis, new GUIContent("Vertical Axis"));
                        break;
                    case MovementXY.InputType.Button:
                        EditorGUILayout.PropertyField(propHorizontalButtonPositive, new GUIContent("Horizontal Positive Button"));
                        EditorGUILayout.PropertyField(propHorizontalButtonNegative, new GUIContent("Horizontal Negative Button"));
                        EditorGUILayout.PropertyField(propVerticalButtonPositive, new GUIContent("Vertical Positive Button"));
                        EditorGUILayout.PropertyField(propVerticalButtonNegative, new GUIContent("Vertical Negative Button"));
                        break;
                    case MovementXY.InputType.Key:
                        EditorGUILayout.PropertyField(propHorizontalKeyPositive, new GUIContent("Horizontal Positive Key"));
                        EditorGUILayout.PropertyField(propHorizontalKeyNegative, new GUIContent("Horizontal Negative Key"));
                        EditorGUILayout.PropertyField(propVerticalKeyPositive, new GUIContent("Vertical Positive Key"));
                        EditorGUILayout.PropertyField(propVerticalKeyNegative, new GUIContent("Vertical Negative Key"));
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
