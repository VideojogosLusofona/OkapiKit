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

                EditorGUILayout.PropertyField(propSpeed, new GUIContent("Speed", "Maximum rotation speed (degrees/second)"));
                EditorGUILayout.PropertyField(propMode, new GUIContent("Mode", "Rotation mode.\nAuto: Object rotates automatically\nInput Set: Object will turn to the direction given by the input (point to)\nInput Delta: Controls make the object turn right/left (tank controls)\nTarget: Object rotates towards a target\nMovement: Object rotates towards the movement direction"));
                if (propMode.enumValueIndex == (int)MovementRotate.RotateMode.InputSet)
                {
                    EditorGUILayout.PropertyField(propAxisToAlign, new GUIContent("Axis To Align", "Direction to align.\nIs the object's 'front' pointing right or up?"));
                    EditorGUILayout.PropertyField(propInputType, new GUIContent("Input Type", "What's the input type?\nAxis: Use two axis to select a direction\nButton: Use four keys to select a direction\nKey: Use four keys to select a direction\nMouse: Use the mouse position relative to the object to select a direction."));

                    var inputType = (MovementRotate.InputType)propInputType.enumValueIndex;

                    switch (inputType)
                    {
                        case MovementRotate.InputType.Axis:
                            EditorGUILayout.PropertyField(propRotationAxisX, new GUIContent("Axis X", "Horizontal axis"));
                            EditorGUILayout.PropertyField(propRotationAxisY, new GUIContent("Axis Y", "Vertical axis"));
                            break;
                        case MovementRotate.InputType.Button:
                            EditorGUILayout.PropertyField(propRotationButtonPositiveX, new GUIContent("Positive X Button", "Right button"));
                            EditorGUILayout.PropertyField(propRotationButtonNegativeX, new GUIContent("Negative X Button", "Left button"));
                            EditorGUILayout.PropertyField(propRotationButtonPositiveY, new GUIContent("Positive Y Button", "Up button"));
                            EditorGUILayout.PropertyField(propRotationButtonNegativeY, new GUIContent("Negative Y Button", "Down button"));
                            break;
                        case MovementRotate.InputType.Key:
                            EditorGUILayout.PropertyField(propRotationKeyPositiveX, new GUIContent("Positive X Key", "Right key"));
                            EditorGUILayout.PropertyField(propRotationKeyNegativeX, new GUIContent("Negative X Key", "Left key"));
                            EditorGUILayout.PropertyField(propRotationKeyPositiveY, new GUIContent("Positive Y Key", "Up key"));
                            EditorGUILayout.PropertyField(propRotationKeyNegativeY, new GUIContent("Negative Y Key", "Down key"));
                            break;
                        case MovementRotate.InputType.Mouse:
                            if (propCameraTag.objectReferenceValue == null)
                            {
                                if (propCameraObject.objectReferenceValue == null)
                                {
                                    EditorGUILayout.PropertyField(propCameraTag, new GUIContent("Camera Tag", "Tag on the camera.\nYou can select the camera either by tag or by linking it, but not both at the same time.\nBy tag is the prefered method."));
                                    EditorGUILayout.PropertyField(propCameraObject, new GUIContent("Camera Object", "Camera object.\nYou can select the camera either by tag or by linking it, but not both at the same time.\nBy tag is the prefered method."));
                                }
                                else
                                {
                                    EditorGUILayout.PropertyField(propCameraObject, new GUIContent("Camera Object", "Camera object.\nYou can select the camera either by tag or by linking it, but not both at the same time.\nBy tag is the prefered method."));
                                }
                            }
                            else
                            {
                                EditorGUILayout.PropertyField(propCameraTag, new GUIContent("Camera Tag", "Tag on the camera.\nYou can select the camera either by tag or by linking it, but not both at the same time.\nBy tag is the prefered method."));
                            }
                            break;
                        default:
                            break;
                    }
                }
                else if (propMode.enumValueIndex == (int)MovementRotate.RotateMode.InputDelta)
                {
                    EditorGUILayout.PropertyField(propInputType, new GUIContent("Input Type", "What's the input type?\nAxis: Use one axis to rotate\nButton: Use two keys to rotate\nKey: Use two keys to rotate\nMouse: Use the mouse position relative to the object to rotate."));

                    var inputType = (MovementRotate.InputType)propInputType.enumValueIndex;

                    switch (inputType)
                    {
                        case MovementRotate.InputType.Axis:
                            EditorGUILayout.PropertyField(propRotationAxis, new GUIContent("Rotation Axis", "Rotation axis"));
                            break;
                        case MovementRotate.InputType.Button:
                            EditorGUILayout.PropertyField(propRotationButtonPositive, new GUIContent("Counter-Clockwise Button", "Counter-clockwise button"));
                            EditorGUILayout.PropertyField(propRotationButtonNegative, new GUIContent("Clockwise Button", "Clockwise button"));
                            break;
                        case MovementRotate.InputType.Key:
                            EditorGUILayout.PropertyField(propRotationKeyPositive, new GUIContent("Counter-Clockwise Key", "Counter-clockwise key"));
                            EditorGUILayout.PropertyField(propRotationKeyNegative, new GUIContent("Clockwise Key", "Clockwise key"));
                            break;
                        case MovementRotate.InputType.Mouse:
                            if (propCameraTag.objectReferenceValue == null)
                            {
                                if (propCameraObject.objectReferenceValue == null)
                                {
                                    EditorGUILayout.PropertyField(propCameraTag, new GUIContent("Camera Tag", "Tag on the camera.\nYou can select the camera either by tag or by linking it, but not both at the same time.\nBy tag is the prefered method."));
                                    EditorGUILayout.PropertyField(propCameraObject, new GUIContent("Camera Object", "Camera object.\nYou can select the camera either by tag or by linking it, but not both at the same time.\nBy tag is the prefered method."));
                                }
                                else
                                {
                                    EditorGUILayout.PropertyField(propCameraObject, new GUIContent("Camera Object", "Camera object.\nYou can select the camera either by tag or by linking it, but not both at the same time.\nBy tag is the prefered method."));
                                }
                            }
                            else
                            {
                                EditorGUILayout.PropertyField(propCameraTag, new GUIContent("Camera Tag", "Tag on the camera.\nYou can select the camera either by tag or by linking it, but not both at the same time.\nBy tag is the prefered method."));
                            }
                            break;
                        default:
                            break;
                    }
                }
                else if (propMode.enumValueIndex == (int)MovementRotate.RotateMode.Target)
                {
                    EditorGUILayout.PropertyField(propAxisToAlign, new GUIContent("Axis To Align", "Is the object pointing right or up?"));
                    if (propTargetTag.objectReferenceValue == null)
                    {
                        if (propTargetObject.objectReferenceValue == null)
                        {
                            EditorGUILayout.PropertyField(propTargetTag, new GUIContent("Target Tag", "What's the target's tag?\nYou can specify either a tag for the target, or link the target itself, but not both at the same time."));
                            EditorGUILayout.PropertyField(propTargetObject, new GUIContent("Target Object", "What's the target object?\nYou can specify either a tag for the target, or link the target itself, but not both at the same time."));
                        }
                        else
                        {
                            EditorGUILayout.PropertyField(propTargetObject, new GUIContent("Target Object", "What's the target object?\nYou can specify either a tag for the target, or link the target itself, but not both at the same time."));
                        }
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(propTargetTag, new GUIContent("Target Tag", "What's the target's tag?\nYou can specify either a tag for the target, or link the target itself, but not both at the same time."));
                    }
                }
                else if (propMode.enumValueIndex == (int)MovementRotate.RotateMode.Movement)
                {
                    EditorGUILayout.PropertyField(propAxisToAlign, new GUIContent("Axis To Align", "Is the object pointing right or up?"));
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