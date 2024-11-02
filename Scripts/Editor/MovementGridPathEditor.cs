using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(MovementGridPath))]
    public class MovementPathGridEditor : MovementEditor
    {
        SerializedProperty propSpeed;
        SerializedProperty propPath;
        SerializedProperty propTaggedPath;
        SerializedProperty propLoop;
        SerializedProperty propRelativePath;
        SerializedProperty propRotationBehaviour;
        SerializedProperty propUseFlip;
        SerializedProperty propMaxRotationSpeed;
        SerializedProperty propCooldown;
        SerializedProperty propPushStrength;
        SerializedProperty propDisplayPath;
        SerializedProperty propUseAnimator;
        SerializedProperty propAnimator;
        SerializedProperty propHorizontalVelocityParameter;
        SerializedProperty propAbsoluteHorizontalVelocityParameter;
        SerializedProperty propVerticalVelocityParameter;
        SerializedProperty propAbsoluteVerticalVelocityParameter;

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
            propMaxRotationSpeed = serializedObject.FindProperty("maxRotationSpeed");
            propCooldown = serializedObject.FindProperty("cooldown");
            propPushStrength = serializedObject.FindProperty("pushStrength");
            propDisplayPath = serializedObject.FindProperty("displayPath");
            propUseAnimator = serializedObject.FindProperty("useAnimator");
            propAnimator = serializedObject.FindProperty("animator");
            propHorizontalVelocityParameter = serializedObject.FindProperty("horizontalVelocityParameter");
            propAbsoluteHorizontalVelocityParameter = serializedObject.FindProperty("absoluteHorizontalVelocityParameter");
            propVerticalVelocityParameter = serializedObject.FindProperty("verticalVelocityParameter");
            propAbsoluteVerticalVelocityParameter = serializedObject.FindProperty("absoluteVerticalVelocityParameter");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                EditorGUI.BeginChangeCheck();

                DefaultMovementEditor();

                EditorGUILayout.PropertyField(propSpeed, new GUIContent("Speed", "Movement speed (in pixels/second)."));
                EditorGUILayout.PropertyField(propCooldown, new GUIContent("Cooldown", "Time between grid movements"));
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
                EditorGUILayout.PropertyField(propDisplayPath, new GUIContent("Display Grid Path", "If on, the grid path will be displayed in the scene view."));

                EditorGUILayout.PropertyField(propRelativePath, new GUIContent("Is Path Relative?", "If on, the relative position of the object to the path's will be accounted for.\nThis means the object will follow the path as if the path's start was at the initial position of this object."));

                EditorGUILayout.PropertyField(propPushStrength, new GUIContent("Push Strength", "This is the strength which is used to push other objects.\nThis is matched with the mass of the objects, and can be accumulated (i.e. you need a strength of 2 to push two objects with 1 mass at the same time).\nA strenght of zero disables the push system."));

                // Separator
                Rect separatorRect = GUILayoutUtility.GetLastRect();
                separatorRect.yMin = separatorRect.yMax + 5;
                separatorRect.height = 5.0f;
                EditorGUI.DrawRect(separatorRect, GUIUtils.ColorFromHex("#ff6060"));
                EditorGUILayout.Space(separatorRect.height + 5);

                EditorGUILayout.PropertyField(propRotationBehaviour, new GUIContent("Rotation Behaviour", "What happens in terms of rotation?\nNone: No rotation applied\nAxis X: the right (X) axis will be aligned with the movement direction\nAxis Y: the up (Y) axis will be aligned with the movement direction."));
                if (propRotationBehaviour.intValue != (int)MovementPath.RotationBehaviour.None)
                {
                    EditorGUILayout.PropertyField(propUseFlip, new GUIContent("Use flip?", "If on, the object flips if the movement direction is to the left/down.\nFliping means scaling by negative coeficient."));
                    EditorGUILayout.PropertyField(propMaxRotationSpeed, new GUIContent("Maximum rotation speed?", "Maximum rotation speed (degrees per second)"));
                }

                EditorGUILayout.PropertyField(propUseAnimator, new GUIContent("Use Animator", "Should we drive an animator with this movement controller?"));
                if (propUseAnimator.boolValue)
                {
                    EditorGUILayout.PropertyField(propAnimator, new GUIContent("Animator", "What animator to use?"));
                    EditorGUILayout.PropertyField(propHorizontalVelocityParameter, new GUIContent("Horizontal Velocity Parameter", "What is the parameter to set to the horizontal velocity?"));
                    EditorGUILayout.PropertyField(propAbsoluteHorizontalVelocityParameter, new GUIContent("Absolute Horizontal Velocity Parameter", "What is the parameter to set to the absolute horizontal velocity?"));
                    EditorGUILayout.PropertyField(propVerticalVelocityParameter, new GUIContent("Vertical Velocity Parameter", "What is the parameter to set to the vertical velocity?"));
                    EditorGUILayout.PropertyField(propAbsoluteVerticalVelocityParameter, new GUIContent("Absolute Vertical Velocity Parameter", "What is the parameter to set to the absolute horizontal velocity?"));
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