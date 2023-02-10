using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MovementFollow))]
public class MovementFollowEditor : MovementEditor
{
    SerializedProperty propSpeed;
    SerializedProperty propTargetTag;
    SerializedProperty propTargetObject;
    SerializedProperty propRelativeMovement;
    SerializedProperty propRotateTowardsDirection;
    SerializedProperty propAlignAxis;
    SerializedProperty propMaxRotationSpeed;

    protected override void OnEnable()
    {
        base.OnEnable();

        propSpeed = serializedObject.FindProperty("speed");
        propTargetTag = serializedObject.FindProperty("targetTag"); ;
        propTargetObject = serializedObject.FindProperty("targetObject"); ;
        propRelativeMovement = serializedObject.FindProperty("relativeMovement"); ;
        propRotateTowardsDirection = serializedObject.FindProperty("rotateTowardsDirection"); ;
        propAlignAxis = serializedObject.FindProperty("alignAxis"); ;
        propMaxRotationSpeed = serializedObject.FindProperty("maxRotationSpeed"); ;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (WriteTitle())
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(propSpeed, new GUIContent("Speed"));
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
            EditorGUILayout.PropertyField(propRelativeMovement, new GUIContent("Relative Movement"));
            EditorGUILayout.PropertyField(propRotateTowardsDirection, new GUIContent("Align With Direction"));
            if (propRotateTowardsDirection.boolValue)
            {
                EditorGUILayout.PropertyField(propAlignAxis, new GUIContent("Alignment Axis"));
                EditorGUILayout.PropertyField(propMaxRotationSpeed, new GUIContent("Max Rotation Speed"));
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
