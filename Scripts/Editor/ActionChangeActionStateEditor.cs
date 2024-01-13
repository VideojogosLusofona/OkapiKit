using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ActionChangeActionState))]
    public class ActionChangeActionStateEditor : ActionEditor
    {
        SerializedProperty propTarget;
        SerializedProperty propState;

        protected override void OnEnable()
        {
            base.OnEnable();

            propTarget = serializedObject.FindProperty("target");
            propState = serializedObject.FindProperty("state");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                StdEditor(false);

                var action = (target as ActionChangeActionState);
                if (action == null) return;

                EditorGUI.BeginChangeCheck();

                EditorGUILayout.PropertyField(propTarget, new GUIContent("Target", "What's the action to turn on/off/toggle?"));
                EditorGUILayout.PropertyField(propState, new GUIContent("State", "What is the state of the target action after this action runs?\nOn - Activates the action\nOff - Disables the action\nToggle - If it is on, turn off. If it is off, turn it on."));

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    (target as Action).UpdateExplanation();
                }
            }
        }
    }
}