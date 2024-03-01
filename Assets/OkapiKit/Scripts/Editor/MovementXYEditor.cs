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

                DefaultMovementEditor();

                EditorGUILayout.PropertyField(propSpeed, new GUIContent("Speed", "Maximum movement speed in world units (pixels)/second"));
                EditorGUILayout.PropertyField(propLimitSpeed, new GUIContent("Limit Speed?", "Should the speed be limited on diagonals?"));
                if (propLimitSpeed.boolValue)
                {
                    EditorGUILayout.PropertyField(propSpeedLimit, new GUIContent("Maximum Speed", "Maximum absolute speed in world units (pixels)/second"));
                }

                EditorGUILayout.PropertyField(propUseRotation, new GUIContent("Use Rotation?", "If true, the X and Y speed is relative to the rotation of the object.\nThis means that if the object is turned they refer to the right and up of the object, and not the absolute screen coordinates."));
                if (!propUseRotation.boolValue)
                {
                    EditorGUILayout.PropertyField(propTurnToDirection, new GUIContent("Turn To Movement Direction?", "If active, the object will turn towards the movement direction."));
                    if (propTurnToDirection.boolValue)
                    {
                        EditorGUILayout.PropertyField(propAxisToAlign, new GUIContent("Axis to align", "Is the object pointing right or up?"));
                        EditorGUILayout.PropertyField(propMaxTurnSpeed, new GUIContent("Max turn speed", "What's the maximum rotation speed (degrees/second)?"));
                    }
                }

                EditorGUILayout.PropertyField(propInputEnabled, new GUIContent("Use Input?", "Is the object controlled by the player?"));
                if (propInputEnabled.boolValue)
                {
                    EditorGUILayout.PropertyField(propInertiaEnable, new GUIContent("Use Inertia?", "If true, the object will have inertia and take a bit to stop completely when input is released."));
                    if (propInertiaEnable.boolValue)
                    {
                        EditorGUILayout.PropertyField(propInertiaStopTime, new GUIContent("Stop Time", "How long does the object take stopping?"));
                    }

                    EditorGUILayout.PropertyField(propInputType, new GUIContent("Input Type", "What's the input type?\nAxis: Use two axis to move\nButton: Use four keys to move\nKey: Use four keys to move"));

                    var inputType = (MovementXY.InputType)propInputType.enumValueIndex;

                    switch (inputType)
                    {
                        case MovementXY.InputType.Axis:
                            EditorGUILayout.PropertyField(propHorizontalAxis, new GUIContent("Horizontal Axis", "Horizontal axis"));
                            EditorGUILayout.PropertyField(propVerticalAxis, new GUIContent("Vertical Axis", "Vertical axis"));
                            break;
                        case MovementXY.InputType.Button:
                            EditorGUILayout.PropertyField(propHorizontalButtonPositive, new GUIContent("Horizontal Positive Button", "Right button"));
                            EditorGUILayout.PropertyField(propHorizontalButtonNegative, new GUIContent("Horizontal Negative Button", "Left button"));
                            EditorGUILayout.PropertyField(propVerticalButtonPositive, new GUIContent("Vertical Positive Button", "Up button"));
                            EditorGUILayout.PropertyField(propVerticalButtonNegative, new GUIContent("Vertical Negative Button", "Down button"));
                            break;
                        case MovementXY.InputType.Key:
                            EditorGUILayout.PropertyField(propHorizontalKeyPositive, new GUIContent("Horizontal Positive Key", "Right key"));
                            EditorGUILayout.PropertyField(propHorizontalKeyNegative, new GUIContent("Horizontal Negative Key", "Left key"));
                            EditorGUILayout.PropertyField(propVerticalKeyPositive, new GUIContent("Vertical Positive Key", "Up key"));
                            EditorGUILayout.PropertyField(propVerticalKeyNegative, new GUIContent("Vertical Negative Key", "Down key"));
                            break;
                        default:
                            break;
                    }
                }
                EditorGUILayout.PropertyField(propDescription, new GUIContent("Description", "This is for you to leave a comment for yourself or others."));

                EditorGUI.EndChangeCheck();

                serializedObject.ApplyModifiedProperties();
                (target as OkapiElement).UpdateExplanation();

                StdEditor(false);
            }
        }

    }
}