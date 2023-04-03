using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(MovementXY))]
    public class MovementXYEditor : MovementEditor
    {
        SerializedProperty propSpeed;
        SerializedProperty propLimitSpeed;
        SerializedProperty propSpeedLimit;
        SerializedProperty propUseRotation;
        SerializedProperty propTurnToDirection;
        SerializedProperty propMaxTurnSpeed;
        SerializedProperty propAxisToAlign;
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
        SerializedProperty propInertiaEnable;
        SerializedProperty propInertiaStopTime;

        protected override void OnEnable()
        {
            base.OnEnable();

            propSpeed = serializedObject.FindProperty("speed");
            propLimitSpeed = serializedObject.FindProperty("limitSpeed");
            propSpeedLimit = serializedObject.FindProperty("speedLimit");
            propUseRotation = serializedObject.FindProperty("useRotation");
            propTurnToDirection = serializedObject.FindProperty("turnToDirection");
            propMaxTurnSpeed = serializedObject.FindProperty("maxTurnSpeed");
            propAxisToAlign = serializedObject.FindProperty("axisToAlign");
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
            propInertiaEnable = serializedObject.FindProperty("inertiaEnable");
            propInertiaStopTime = serializedObject.FindProperty("inertiaStopTime");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                EditorGUI.BeginChangeCheck();

                EditorGUILayout.PropertyField(propSpeed, new GUIContent("Speed"));
                EditorGUILayout.PropertyField(propLimitSpeed, new GUIContent("Limit Speed?"));
                if (propLimitSpeed.boolValue)
                {
                    EditorGUILayout.PropertyField(propSpeedLimit, new GUIContent("Maximum Speed"));
                }

                EditorGUILayout.PropertyField(propUseRotation, new GUIContent("Use Rotation?"));
                if (!propUseRotation.boolValue)
                {
                    EditorGUILayout.PropertyField(propTurnToDirection, new GUIContent("Turn To Movement Direction?"));
                    if (propTurnToDirection.boolValue)
                    {
                        EditorGUILayout.PropertyField(propAxisToAlign, new GUIContent("Axis to align"));
                        EditorGUILayout.PropertyField(propMaxTurnSpeed, new GUIContent("Max turn speed"));
                    }
                }

                EditorGUILayout.PropertyField(propInertiaEnable, new GUIContent("Use Inertia?"));
                if (propInertiaEnable.boolValue)
                {
                    EditorGUILayout.PropertyField(propInertiaStopTime, new GUIContent("Stop Time"));
                }

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

                EditorGUI.EndChangeCheck();

                serializedObject.ApplyModifiedProperties();
                (target as OkapiElement).UpdateExplanation();

                StdEditor(false);
            }
        }

    }
}