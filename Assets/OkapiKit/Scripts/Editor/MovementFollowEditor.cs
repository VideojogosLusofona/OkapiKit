using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(MovementFollow))]
    public class MovementFollowEditor : MovementEditor
    {
        SerializedProperty propSpeed;
        SerializedProperty propTargetType;
        SerializedProperty propTargetTag;
        SerializedProperty propTargetObject;
        SerializedProperty propRelativeMovement;
        SerializedProperty propRotateTowardsDirection;
        SerializedProperty propAlignAxis;
        SerializedProperty propMaxRotationSpeed;
        SerializedProperty propCameraObject;
        SerializedProperty propCameraTag;
        SerializedProperty propStoppingDistance;

        protected override void OnEnable()
        {
            base.OnEnable();

            propSpeed = serializedObject.FindProperty("speed");
            propTargetType = serializedObject.FindProperty("targetType");
            propTargetTag = serializedObject.FindProperty("targetTag");
            propTargetObject = serializedObject.FindProperty("targetObject");
            propRelativeMovement = serializedObject.FindProperty("relativeMovement");
            propRotateTowardsDirection = serializedObject.FindProperty("rotateTowardsDirection");
            propAlignAxis = serializedObject.FindProperty("alignAxis");
            propMaxRotationSpeed = serializedObject.FindProperty("maxRotationSpeed");
            propCameraObject = serializedObject.FindProperty("cameraObject");
            propCameraTag = serializedObject.FindProperty("cameraTag");
            propStoppingDistance = serializedObject.FindProperty("stoppingDistance");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                EditorGUI.BeginChangeCheck();

                EditorGUILayout.PropertyField(propSpeed, new GUIContent("Speed", "Maximum follow speed"));
                EditorGUILayout.PropertyField(propTargetType, new GUIContent("Target Type", "Type of target to follow.\nTag: Find the closest object tagged with the given tag, and follow it\nObject: Follow the linked object\nMouse: Follow the mouse cursor"));

                MovementFollow.TargetType type = (MovementFollow.TargetType)propTargetType.enumValueIndex;
                switch (type)
                {
                    case MovementFollow.TargetType.Tag:
                        EditorGUILayout.PropertyField(propTargetTag, new GUIContent("Target Tag", "Find objects with this tag, and follow the closest one."));
                        break;
                    case MovementFollow.TargetType.Object:
                        EditorGUILayout.PropertyField(propTargetObject, new GUIContent("Target Object", "Follow this object."));
                        break;
                    case MovementFollow.TargetType.Mouse:
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

                if (type != MovementFollow.TargetType.Mouse)
                {
                    EditorGUILayout.PropertyField(propRelativeMovement, new GUIContent("Relative Movement", "Is the follow movement relative?\nIf on, the object will preserve the relative position to the object being followed."));
                }
                if (((type == MovementFollow.TargetType.Mouse) || (!propRelativeMovement.boolValue)) &&
                    (propSpeed.floatValue > 0.0f))
                {
                    EditorGUILayout.PropertyField(propStoppingDistance, new GUIContent("Stopping Distance", "At what distance should the object stop from the target object?"));
                }

                EditorGUILayout.PropertyField(propRotateTowardsDirection, new GUIContent("Align With Direction", "If on, the object will be turned towards the target object."));
                if (propRotateTowardsDirection.boolValue)
                {
                    EditorGUILayout.PropertyField(propAlignAxis, new GUIContent("Alignment Axis", "Alignment axis (i.e. is the object pointing to the right or up?)"));
                    EditorGUILayout.PropertyField(propMaxRotationSpeed, new GUIContent("Max Rotation Speed", "What's the maximum rotation speed (degrees per second)?"));
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