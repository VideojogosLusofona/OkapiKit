using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
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
        SerializedProperty propCameraTag;
        SerializedProperty propCameraObject;

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
            propCameraTag = serializedObject.FindProperty("cameraTag"); ;
            propCameraObject = serializedObject.FindProperty("cameraObject"); ;
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

                    var inputType = (MovementRotate.InputType)propInputType.enumValueIndex;

                    switch (inputType)
                    {
                        case MovementRotate.InputType.Axis:
                            EditorGUILayout.PropertyField(propRotationAxisX, new GUIContent("Axis X"));
                            EditorGUILayout.PropertyField(propRotationAxisY, new GUIContent("Axis Y"));
                            break;
                        case MovementRotate.InputType.Button:
                            EditorGUILayout.PropertyField(propRotationButtonPositiveX, new GUIContent("Positive X Button"));
                            EditorGUILayout.PropertyField(propRotationButtonNegativeX, new GUIContent("Negative X Button"));
                            EditorGUILayout.PropertyField(propRotationButtonPositiveY, new GUIContent("Positive Y Button"));
                            EditorGUILayout.PropertyField(propRotationButtonNegativeY, new GUIContent("Negative Y Button"));
                            break;
                        case MovementRotate.InputType.Key:
                            EditorGUILayout.PropertyField(propRotationKeyPositiveX, new GUIContent("Positive X Key"));
                            EditorGUILayout.PropertyField(propRotationKeyNegativeX, new GUIContent("Negative X Key"));
                            EditorGUILayout.PropertyField(propRotationKeyPositiveY, new GUIContent("Positive Y Key"));
                            EditorGUILayout.PropertyField(propRotationKeyNegativeY, new GUIContent("Negative Y Key"));
                            break;
                        case MovementRotate.InputType.Mouse:
                            if (propCameraTag.objectReferenceValue == null)
                            {
                                if (propCameraObject.objectReferenceValue == null)
                                {
                                    EditorGUILayout.PropertyField(propCameraTag, new GUIContent("Camera Tag"));
                                    EditorGUILayout.PropertyField(propCameraObject, new GUIContent("Camera Object"));
                                }
                                else
                                {
                                    EditorGUILayout.PropertyField(propCameraObject, new GUIContent("Camera Object"));
                                }
                            }
                            else
                            {
                                EditorGUILayout.PropertyField(propCameraTag, new GUIContent("Camera Tag"));
                            }
                            break;
                        default:
                            break;
                    }
                }
                else if (propMode.enumValueIndex == (int)MovementRotate.RotateMode.InputDelta)
                {
                    EditorGUILayout.PropertyField(propInputType, new GUIContent("Input Type"));

                    var inputType = (MovementRotate.InputType)propInputType.enumValueIndex;

                    switch (inputType)
                    {
                        case MovementRotate.InputType.Axis:
                            EditorGUILayout.PropertyField(propRotationAxis, new GUIContent("Rotation Axis"));
                            break;
                        case MovementRotate.InputType.Button:
                            EditorGUILayout.PropertyField(propRotationButtonPositive, new GUIContent("Counter-Clockwise Button"));
                            EditorGUILayout.PropertyField(propRotationButtonNegative, new GUIContent("Clockwise Button"));
                            break;
                        case MovementRotate.InputType.Key:
                            EditorGUILayout.PropertyField(propRotationKeyPositive, new GUIContent("Counter-Clockwise Key"));
                            EditorGUILayout.PropertyField(propRotationKeyNegative, new GUIContent("Clockwise Key"));
                            break;
                        case MovementRotate.InputType.Mouse:
                            if (propCameraTag.objectReferenceValue == null)
                            {
                                if (propCameraObject.objectReferenceValue == null)
                                {
                                    EditorGUILayout.PropertyField(propCameraTag, new GUIContent("Camera Tag"));
                                    EditorGUILayout.PropertyField(propCameraObject, new GUIContent("Camera Object"));
                                }
                                else
                                {
                                    EditorGUILayout.PropertyField(propCameraObject, new GUIContent("Camera Object"));
                                }
                            }
                            else
                            {
                                EditorGUILayout.PropertyField(propCameraTag, new GUIContent("Camera Tag"));
                            }
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
                else if (propMode.enumValueIndex == (int)MovementRotate.RotateMode.Movement)
                {
                    EditorGUILayout.PropertyField(propAxisToAlign, new GUIContent("Axis To Align"));
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