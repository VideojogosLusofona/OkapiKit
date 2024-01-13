using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(MovementPath))]
    public class MovementPathEditor : MovementEditor
    {
        SerializedProperty propSpeed;
        SerializedProperty propPath;
        SerializedProperty propTaggedPath;
        SerializedProperty propLoop;
        SerializedProperty propRelativePath;
        SerializedProperty propRotationBehaviour;
        SerializedProperty propUseFlip;
        SerializedProperty propHasMaxRotationSpeed;
        SerializedProperty propMaxRotationSpeed;

        protected override void OnEnable()
        {
            base.OnEnable();

            propSpeed = serializedObject.FindProperty("speed");
            propPath = serializedObject.FindProperty("path");
            propTaggedPath = serializedObject.FindProperty("taggedPath");
            propLoop = serializedObject.FindProperty("loop");
            propRelativePath = serializedObject.FindProperty("relativePath");
            propRotationBehaviour = serializedObject.FindProperty("rotationBehaviour");
            propUseFlip = serializedObject.FindProperty("useFlip");
            propHasMaxRotationSpeed = serializedObject.FindProperty("hasMaxRotationSpeed");
            propMaxRotationSpeed = serializedObject.FindProperty("maxRotationSpeed");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                EditorGUI.BeginChangeCheck();

                EditorGUILayout.PropertyField(propSpeed, new GUIContent("Speed", "Movement speed (in pixels/second)."));
                if (propPath.objectReferenceValue == null)
                {
                    if (propTaggedPath.objectReferenceValue == null)
                    {
                        EditorGUILayout.PropertyField(propPath, new GUIContent("Path", "What path should this object follow?\nYou can either link a path, or reference it by tag, but not both at the same time.\nIf you use a tag to define the path, if there's multiple paths with the same tag, a random one will be selected."));
                        EditorGUILayout.PropertyField(propTaggedPath, new GUIContent("Path Tag", "Find a path with this tag.\nYou can either link a path, or reference it by tag, but not both at the same time.\nIf you use a tag to define the path, if there's multiple paths with the same tag, a random one will be selected."));
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(propTaggedPath, new GUIContent("Path Tag", "Find a path with this tag.\nYou can either link a path, or reference it by tag, but not both at the same time.\nIf you use a tag to define the path, if there's multiple paths with the same tag, a random one will be selected."));
                    }
                }
                else
                {
                    EditorGUILayout.PropertyField(propPath, new GUIContent("Path", "What path should this object follow?\nYou can either link a path, or reference it by tag, but not both at the same time.\nIf you use a tag to define the path, if there's multiple paths with the same tag, a random one will be selected."));
                }
                EditorGUILayout.PropertyField(propLoop, new GUIContent("Loop", "If on, the object will loop around the path."));
                EditorGUILayout.PropertyField(propRelativePath, new GUIContent("Is Path Relative?", "If on, the relative position of the object to the path's will be accounted for.\nThis means the object will follow the path as if the path's start was at the initial position of this object."));
                EditorGUILayout.PropertyField(propRotationBehaviour, new GUIContent("Rotation Behaviour", "What happens in terms of rotation?\nNone: No rotation applied\nAxis X: the right (X) axis will be aligned with the movement direction\nAxis Y: the up (Y) axis will be aligned with the movement direction."));
                if (propRotationBehaviour.enumValueIndex != (int)MovementPath.RotationBehaviour.None)
                {
                    EditorGUILayout.PropertyField(propUseFlip, new GUIContent("Use flip?", "If on, the object flips if the movement direction is to the left/down.\nFliping means scaling by negative coeficient."));
                    EditorGUILayout.PropertyField(propHasMaxRotationSpeed, new GUIContent("Has maximum rotation speed?", "If on, the object has a maximum rotation speed."));
                    if (propHasMaxRotationSpeed.boolValue)
                    {
                        EditorGUILayout.PropertyField(propMaxRotationSpeed, new GUIContent("Maximum rotation speed?", "Maximum rotation speed (degrees per second)"));
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