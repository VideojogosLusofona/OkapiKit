using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(MovementForward))]
    public class MovementForwardEditor : MovementEditor
    {
        SerializedProperty propSpeed;
        SerializedProperty propAxis;

        protected override void OnEnable()
        {
            base.OnEnable();

            propSpeed = serializedObject.FindProperty("speed");
            propAxis = serializedObject.FindProperty("axis");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                EditorGUI.BeginChangeCheck();

                EditorGUILayout.PropertyField(propSpeed, new GUIContent("Speed", "What's the movement speed (pixels/second)?"));
                EditorGUILayout.PropertyField(propAxis, new GUIContent("Axis", "What is forward (i.e. is the object point to the right, or pointing up?)"));

                EditorGUILayout.PropertyField(propDescription, new GUIContent("Description", "This is for you to leave a comment for yourself or others."));

                EditorGUI.EndChangeCheck();

                serializedObject.ApplyModifiedProperties();
                (target as OkapiElement).UpdateExplanation();

                StdEditor(false);
            }
        }

    }
}