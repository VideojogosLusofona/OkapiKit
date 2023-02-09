using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MovementRotate))]
public class MovementRotateEditor : MovementEditor
{
    SerializedProperty propSpeed;
    SerializedProperty propMode;
    SerializedProperty propInputType;
    SerializedProperty propRotationAxis;
    SerializedProperty propRotationAxisX;
    SerializedProperty propRotationAxisY;
    SerializedProperty propRotationButtonPositive;
    SerializedProperty propRotationButtonNegative;
    SerializedProperty propRotationButtonPositiveX;
    SerializedProperty propRotationButtonNegativeX;
    SerializedProperty propRotationButtonPositiveY;
    SerializedProperty propRotationButtonNegativeY;
    SerializedProperty propRotationKeyPositive;
    SerializedProperty propRotationKeyNegative;
    SerializedProperty propRotationKeyPositiveX;
    SerializedProperty propRotationKeyNegativeX;
    SerializedProperty propRotationKeyPositiveY;
    SerializedProperty propRotationKeyNegativeY;
    SerializedProperty propAxisToAlign;
    SerializedProperty propTargetTag;
    SerializedProperty propTargetObject;

    protected override void OnEnable()
    {
        base.OnEnable();

        propSpeed = serializedObject.FindProperty("speed");
        propMode = serializedObject.FindProperty("mode");
        propInputType = serializedObject.FindProperty("inputType");
        propRotationAxis = serializedObject.FindProperty("rotationAxis");
        propRotationAxisX = serializedObject.FindProperty("rotationAxisX");
        propRotationAxisY = serializedObject.FindProperty("rotationAxisY");
        propRotationButtonPositive = serializedObject.FindProperty("rotationButtonPositive");
        propRotationButtonNegative = serializedObject.FindProperty("rotationButtonNegative");
        propRotationButtonPositiveX = serializedObject.FindProperty("rotationButtonPositiveX");
        propRotationButtonNegativeX = serializedObject.FindProperty("rotationButtonNegativeX");
        propRotationButtonPositiveY = serializedObject.FindProperty("rotationButtonPositiveY");
        propRotationButtonNegativeY = serializedObject.FindProperty("rotationButtonNegativeY");
        propRotationKeyPositive = serializedObject.FindProperty("rotationKeyPositive");
        propRotationKeyNegative = serializedObject.FindProperty("rotationKeyNegative");
        propRotationKeyPositiveX = serializedObject.FindProperty("rotationKeyPositiveX");
        propRotationKeyNegativeX = serializedObject.FindProperty("rotationKeyNegativeX");
        propRotationKeyPositiveY = serializedObject.FindProperty("rotationKeyPositiveY");
        propRotationKeyNegativeY = serializedObject.FindProperty("rotationKeyNegativeY");
        propAxisToAlign = serializedObject.FindProperty("axisToAlign");
        propTargetTag = serializedObject.FindProperty("targetTag"); ;
        propTargetObject = serializedObject.FindProperty("targetObject"); ;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (WriteTitle())
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(propSpeed, new GUIContent("Speed"));
            EditorGUILayout.PropertyField(propMode, new GUIContent("Mode"));
            if (propMode.enumValueIndex == (int)MovementRotate.RotateMode.InputSet)
            {
                EditorGUILayout.PropertyField(propAxisToAlign, new GUIContent("Axis To Align"));
                EditorGUILayout.PropertyField(propInputType, new GUIContent("Input Type"));

                var inputType = (MovementXY.InputType)propInputType.enumValueIndex;

                switch (inputType)
                {
                    case MovementXY.InputType.Axis:
                        EditorGUILayout.PropertyField(propRotationAxisX, new GUIContent("Axis X"));
                        EditorGUILayout.PropertyField(propRotationAxisY, new GUIContent("Axis Y"));
                        break;
                    case MovementXY.InputType.Button:
                        EditorGUILayout.PropertyField(propRotationButtonPositiveX, new GUIContent("Positive X Button"));
                        EditorGUILayout.PropertyField(propRotationButtonNegativeX, new GUIContent("Negative X Button"));
                        EditorGUILayout.PropertyField(propRotationButtonPositiveY, new GUIContent("Positive Y Button"));
                        EditorGUILayout.PropertyField(propRotationButtonNegativeY, new GUIContent("Negative Y Button"));
                        break;
                    case MovementXY.InputType.Key:
                        EditorGUILayout.PropertyField(propRotationKeyPositiveX, new GUIContent("Positive X Key"));
                        EditorGUILayout.PropertyField(propRotationKeyNegativeX, new GUIContent("Negative X Key"));
                        EditorGUILayout.PropertyField(propRotationKeyPositiveY, new GUIContent("Positive Y Key"));
                        EditorGUILayout.PropertyField(propRotationKeyNegativeY, new GUIContent("Negative Y Key"));
                        break;
                    default:
                        break;
                }
            }
            else if (propMode.enumValueIndex == (int)MovementRotate.RotateMode.InputDelta)
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
            else if (propMode.enumValueIndex == (int)MovementRotate.RotateMode.Target)
            {
                EditorGUILayout.PropertyField(propAxisToAlign, new GUIContent("Axis To Align"));
                if (propTargetTag.objectReferenceValue == null)
                {
                    if (propTargetObject.objectReferenceValue == null)
                    {
                        EditorGUILayout.PropertyField(propTargetTag, new GUIContent("Target Tag"));
                        EditorGUILayout.PropertyField(propTargetObject, new GUIContent("Target Object"));
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(propTargetObject, new GUIContent("Target Object"));
                    }
                }
                else
                {
                    EditorGUILayout.PropertyField(propTargetTag, new GUIContent("Target Tag"));
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
