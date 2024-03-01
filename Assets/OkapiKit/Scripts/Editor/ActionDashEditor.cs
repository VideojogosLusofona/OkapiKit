using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static OkapiKit.ActionDash;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ActionDash))]
    public class ActionDashEditor : ActionEditor
    {
        SerializedProperty propTarget;
        SerializedProperty propSpeed;
        SerializedProperty propDirection;
        SerializedProperty propAngle;
        SerializedProperty propDuration;

        protected override void OnEnable()
        {
            base.OnEnable();

            propTarget = serializedObject.FindProperty("target"); ;
            propSpeed = serializedObject.FindProperty("speed"); ;
            propDirection = serializedObject.FindProperty("direction"); ;
            propAngle = serializedObject.FindProperty("angle"); ;
            propDuration = serializedObject.FindProperty("duration"); ;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                StdEditor(false);

                var action = (target as ActionDash);
                if (action == null) return;

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(propTarget, new GUIContent("Target", "Which object transform will be moved.\nDashing is just changing the position of an object in a certain way."));
                EditorGUILayout.PropertyField(propSpeed, new GUIContent("Speed Range", "How fast should the object move, in units/second?\nThis is a range, everytime this action runs, a random number will be selected between these two and used as the speed."));
                EditorGUILayout.PropertyField(propDirection, new GUIContent("Direction", "In which direction should the object move? This is the basis from where the direction itself is calculated, using the next property (angle)"));
                EditorGUILayout.PropertyField(propAngle, new GUIContent("Angle", "What's the angle of movement, measured from the axis above."));
                EditorGUILayout.PropertyField(propDuration, new GUIContent("Duration", "For how long should the movement happen? If this action is triggered again, it will cancel previous dashes."));

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    (target as Action).UpdateExplanation();
                }
            }
        }
    }
}