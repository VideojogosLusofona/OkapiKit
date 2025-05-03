using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ActionBlink))]
    public class ActionBlinkEditor : ActionEditor
    {
        SerializedProperty propTarget;
        SerializedProperty propIncludeChildren;
        SerializedProperty propBlinkTimeOn;
        SerializedProperty propBlinkTimeOff;
        SerializedProperty propDuration;

        protected override void OnEnable()
        {
            base.OnEnable();

            propTarget = serializedObject.FindProperty("target");
            propIncludeChildren = serializedObject.FindProperty("includeChildren");
            propBlinkTimeOn = serializedObject.FindProperty("blinkTimeOn");
            propBlinkTimeOff = serializedObject.FindProperty("blinkTimeOff");
            propDuration = serializedObject.FindProperty("duration");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                StdEditor(false);

                var action = (target as ActionBlink);
                if (action == null) return;

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(propTarget, new GUIContent("Target", "Which renderer will blink.\nBlinking is just turning it on/off at the given speed, so it works with all renderer types."));
                EditorGUILayout.PropertyField(propIncludeChildren, new GUIContent("Include Children?", "Should all the renderers in children object of the target object also blink?"));
                EditorGUILayout.PropertyField(propBlinkTimeOn, new GUIContent("On Duration", "How long should the renderer be on, in seconds."));
                EditorGUILayout.PropertyField(propBlinkTimeOff, new GUIContent("Off Duration", "How long should the renderer be off, in seconds."));
                EditorGUILayout.PropertyField(propDuration, new GUIContent("Duration", "How long should the renderers be blinking?"));

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    action.UpdateExplanation();
                }
            }
        }
    }
}