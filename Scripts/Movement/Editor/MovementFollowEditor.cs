using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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

            EditorGUILayout.PropertyField(propSpeed, new GUIContent("Speed"));
            EditorGUILayout.PropertyField(propTargetType, new GUIContent("Target Type"));

            MovementFollow.TargetType type = (MovementFollow.TargetType)propTargetType.enumValueIndex;
            switch (type)
            {
                case MovementFollow.TargetType.Tag:
                    EditorGUILayout.PropertyField(propTargetTag, new GUIContent("Target Tag"));
                    break;
                case MovementFollow.TargetType.Object:
                    EditorGUILayout.PropertyField(propTargetObject, new GUIContent("Target Object"));
                    break;
                case MovementFollow.TargetType.Mouse:
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

            if (type != MovementFollow.TargetType.Mouse)
            {
                EditorGUILayout.PropertyField(propRelativeMovement, new GUIContent("Relative Movement"));
            }
            if (((type == MovementFollow.TargetType.Mouse) || (!propRelativeMovement.boolValue)) &&
                (propSpeed.floatValue > 0.0f))
            {
                EditorGUILayout.PropertyField(propStoppingDistance, new GUIContent("Stopping Distance"));
            }

            EditorGUILayout.PropertyField(propRotateTowardsDirection, new GUIContent("Align With Direction"));
            if (propRotateTowardsDirection.boolValue)
            {
                EditorGUILayout.PropertyField(propAlignAxis, new GUIContent("Alignment Axis"));
                EditorGUILayout.PropertyField(propMaxRotationSpeed, new GUIContent("Max Rotation Speed"));
            }

            EditorGUILayout.PropertyField(propDescription, new GUIContent("Description"));

            EditorGUI.EndChangeCheck();

            serializedObject.ApplyModifiedProperties();
            (target as OkapiElement).UpdateExplanation();

            StdEditor(false);
        }
    }

}
