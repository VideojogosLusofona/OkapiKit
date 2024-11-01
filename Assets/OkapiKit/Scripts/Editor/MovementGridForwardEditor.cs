using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(MovementGridForward))]
    public class MovementGridForwardEditor : MovementEditor
    {
        SerializedProperty propSpeed;
        SerializedProperty propAxis;
        SerializedProperty propCooldown;
        SerializedProperty propPushStrength;

        protected override void OnEnable()
        {
            base.OnEnable();

            propSpeed = serializedObject.FindProperty("speed");
            propAxis = serializedObject.FindProperty("axis");
            propCooldown = serializedObject.FindProperty("cooldown");
            propPushStrength = serializedObject.FindProperty("pushStrength");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                EditorGUI.BeginChangeCheck();

                DefaultMovementEditor();

                EditorGUILayout.PropertyField(propSpeed, new GUIContent("Speed", "What's the movement speed (pixels/second)?"));
                EditorGUILayout.PropertyField(propAxis, new GUIContent("Axis", "What is forward (i.e. is the object point to the right, or pointing up?)"));

                EditorGUILayout.PropertyField(propCooldown, new GUIContent("Cooldown", "Time between grid movements"));

                EditorGUILayout.PropertyField(propPushStrength, new GUIContent("Push Strength", "This is the strength which is used to push other objects.\nThis is matched with the mass of the objects, and can be accumulated (i.e. you need a strength of 2 to push two objects with 1 mass at the same time).\nA strenght of zero disables the push system."));

                EditorGUILayout.PropertyField(propDescription, new GUIContent("Description", "This is for you to leave a comment for yourself or others."));

                EditorGUI.EndChangeCheck();

                serializedObject.ApplyModifiedProperties();
                (target as OkapiElement).UpdateExplanation();

                StdEditor(false);
            }
        }

    }
}